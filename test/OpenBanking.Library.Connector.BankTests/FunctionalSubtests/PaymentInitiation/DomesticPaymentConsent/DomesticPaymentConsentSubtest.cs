// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.
    DomesticPaymentConsent;

public class DomesticPaymentConsentSubtest(PaymentInitiationApiClient paymentInitiationApiClient)
{
    public static ISet<DomesticPaymentSubtestEnum> DomesticPaymentFunctionalSubtestsSupported(
        BankProfile bankProfile) =>
        DomesticPaymentSubtestHelper.AllDomesticPaymentSubtests;

    public async Task RunTest(
        DomesticPaymentSubtestEnum subtestEnum,
        BankProfile bankProfile,
        Guid bankRegistrationId,
        OAuth2ResponseMode defaultResponseMode,
        Func<IServiceScopeContainer> serviceScopeGenerator,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        ConsentAuth? consentAuth,
        string authUrlLeftPart,
        BankUser? bankUser,
        IMemoryCache memoryCache)
    {
        bool subtestSkipped = subtestEnum switch
        {
            DomesticPaymentSubtestEnum.PersonToPersonSubtest =>
                true,
            DomesticPaymentSubtestEnum.PersonToMerchantSubtest => false,
            _ => throw new ArgumentException(
                $"{nameof(subtestEnum)} is not valid {nameof(DomesticPaymentSubtestEnum)} or needs to be added to this switch statement.")
        };
        if (subtestSkipped)
        {
            return;
        }

        // Get request builder
        using IServiceScopeContainer serviceScopeContainer = serviceScopeGenerator();
        IRequestBuilder requestBuilder = serviceScopeContainer.RequestBuilder;

        // Create DomesticPaymentConsent request
        var initiationInstructionIdentification = Guid.NewGuid().ToString("N");
        var initiationEndToEndIdentification = Guid.NewGuid().ToString("N");
        var domesticPaymentTemplateRequest = new DomesticPaymentTemplateRequest
        {
            Type = DomesticPaymentSubtestHelper
                .GetDomesticPaymentTemplateType(subtestEnum),
            Parameters = new DomesticPaymentTemplateParameters
            {
                InstructionIdentification = "placeholder", // logging placeholder
                EndToEndIdentification = "placeholder" // logging placeholder
            }
        };
        DomesticPaymentConsentRequest domesticPaymentConsentRequest = await GetDomesticPaymentConsentRequest(
            bankProfile,
            bankRegistrationId,
            testNameUnique,
            modifiedBy,
            pispFluentRequestLogging,
            domesticPaymentTemplateRequest,
            initiationInstructionIdentification,
            initiationEndToEndIdentification);

        DomesticPaymentConsentCreateResponse domesticPaymentConsentResp =
            await DomesticPaymentConsentCreate(domesticPaymentConsentRequest);
        Guid domesticPaymentConsentId = domesticPaymentConsentResp.Id;

        // Consent authorisation
        if (consentAuth is not null)
        {
            // Method that checks for auth completion
            async Task<bool> AuthIsComplete()
            {
                DomesticPaymentConsentCreateResponse consentResponse =
                    await DomesticPaymentConsentRead(domesticPaymentConsentId, true);
                return consentResponse.Created < consentResponse.AuthContextModified;
            }

            // Perform auth
            if (bankProfile.SupportsSca)
            {
                // Create AuthContext
                var authContextRequest = new DomesticPaymentConsentAuthContext
                {
                    DomesticPaymentConsentId = domesticPaymentConsentId,
                    Reference = testNameUnique + "_DomesticPaymentConsent",
                    CreatedBy = modifiedBy
                };
                DomesticPaymentConsentAuthContextCreateResponse authContextResponse =
                    await DomesticPaymentConsentAuthContextCreate(authContextRequest);

                // Perform email auth
                await consentAuth.EmailAuthAsync(
                    authContextResponse.AuthUrl,
                    AuthIsComplete);
            }
            else
            {
                if (bankUser is null)
                {
                    throw new ArgumentException("No user specified for consent auth.");
                }

                // Perform automated auth
                var authUrl =
                    $"{authUrlLeftPart}/dev1/pisp/domestic-payment-consents/{domesticPaymentConsentId}/auth";
                await consentAuth.AutomatedAuthAsync(
                    authUrl,
                    bankProfile,
                    ConsentVariety.DomesticPaymentConsent,
                    bankUser,
                    defaultResponseMode,
                    AuthIsComplete);
            }

            // Refresh scope to ensure user token acquired following consent is available
            using IServiceScopeContainer scopedServiceScopeNew = serviceScopeGenerator();
            IRequestBuilder requestBuilderNew = scopedServiceScopeNew.RequestBuilder;

            // Read DomesticPaymentConsentFundsConfirmation
            await DomesticPaymentConsentReadFundsConfirmation(domesticPaymentConsentId);

            DomesticPaymentRequest domesticPaymentRequest = await GetDomesticPaymentRequest(
                bankProfile,
                modifiedBy,
                pispFluentRequestLogging,
                domesticPaymentTemplateRequest,
                initiationInstructionIdentification,
                initiationEndToEndIdentification,
                domesticPaymentConsentId);
            DomesticPaymentResponse domesticPaymentResp =
                await requestBuilderNew.PaymentInitiation.DomesticPayments
                    .CreateAsync(
                        domesticPaymentRequest,
                        new ConsentExternalCreateParams
                        {
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null
                        });

            // Checks
            domesticPaymentResp.Should().NotBeNull();
            domesticPaymentResp.Warnings.Should().BeNull();
            domesticPaymentResp.ExternalApiResponse.Should().NotBeNull();
            string domesticPaymentExternalId = domesticPaymentResp.ExternalApiResponse.Data.DomesticPaymentId;

            // GET domestic payment
            DomesticPaymentResponse domesticPaymentResp2 =
                await requestBuilderNew.PaymentInitiation.DomesticPayments
                    .ReadAsync(
                        new ConsentExternalEntityReadParams
                        {
                            ConsentId = domesticPaymentConsentId,
                            ModifiedBy = null,
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null,
                            ExternalApiId = domesticPaymentExternalId
                        });

            // Checks
            domesticPaymentResp2.Should().NotBeNull();
            domesticPaymentResp2.Warnings.Should().BeNull();
            domesticPaymentResp2.ExternalApiResponse.Should().NotBeNull();

            // GET domestic payment payment details
            if (bankProfile.PaymentInitiationApiSettings.UseDomesticPaymentGetPaymentDetailsEndpoint)
            {
                DomesticPaymentPaymentDetailsResponse paymentDetailsResponse =
                    await requestBuilderNew.PaymentInitiation.DomesticPayments
                        .ReadPaymentDetailsAsync(
                            new ConsentExternalEntityReadParams
                            {
                                ConsentId = domesticPaymentConsentId,
                                ModifiedBy = null,
                                ExtraHeaders = null,
                                PublicRequestUrlWithoutQuery = null,
                                ExternalApiId = domesticPaymentExternalId
                            });

                // Checks
                paymentDetailsResponse.Should().NotBeNull();
                paymentDetailsResponse.Warnings.Should().BeNull();
                paymentDetailsResponse.ExternalApiResponse.Should().NotBeNull();
            }
        }

        await DomesticPaymentConsentDelete(requestBuilder, domesticPaymentConsentId);
    }

    private static async Task<DomesticPaymentRequest> GetDomesticPaymentRequest(
        BankProfile bankProfile,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        DomesticPaymentTemplateRequest domesticPaymentTemplateRequest,
        string initiationInstructionIdentification,
        string initiationEndToEndIdentification,
        Guid domesticPaymentConsentId)
    {
        // Create DomesticPayment request
        var domesticPaymentRequest = new DomesticPaymentRequest
        {
            DomesticPaymentConsentId = default, // logging placeholder
            ExternalApiRequest =
                DomesticPaymentPublicMethods.ResolveExternalApiRequest(
                    null,
                    domesticPaymentTemplateRequest,
                    string.Empty, // logging placeholder
                    bankProfile) // resolve for fuller logging
        };
        await pispFluentRequestLogging
            .AppendToPath("domesticPayment")
            .AppendToPath("postRequest")
            .WriteFile(domesticPaymentRequest);

        // POST domestic payment
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification =
            initiationInstructionIdentification; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification =
            initiationEndToEndIdentification; // replace logging placeholder
        domesticPaymentRequest.DomesticPaymentConsentId = domesticPaymentConsentId;
        domesticPaymentRequest.ModifiedBy = modifiedBy;

        return domesticPaymentRequest;
    }

    private async Task<DomesticPaymentConsentFundsConfirmationResponse> DomesticPaymentConsentReadFundsConfirmation(
        Guid domesticPaymentConsentId)
    {
        // GET consent funds confirmation
        DomesticPaymentConsentFundsConfirmationResponse domesticPaymentConsentFundsConfirmationResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentReadFundsConfirmation(
                new ConsentBaseReadParams
                {
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null
                });

        // Checks
        domesticPaymentConsentFundsConfirmationResponse.ExternalApiResponse.Should().NotBeNull();
        return domesticPaymentConsentFundsConfirmationResponse;
    }

    private async Task<DomesticPaymentConsentAuthContextCreateResponse> DomesticPaymentConsentAuthContextCreate(
        DomesticPaymentConsentAuthContext authContextRequest)
    {
        DomesticPaymentConsentAuthContextCreateResponse authContextResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentAuthContextCreate(authContextRequest);

        // Checks
        authContextResponse.AuthUrl.Should().NotBeNull();

        // GET auth context
        await DomesticPaymentConsentAuthContextRead(authContextResponse.Id);

        return authContextResponse;
    }

    private async Task<DomesticPaymentConsentAuthContextReadResponse> DomesticPaymentConsentAuthContextRead(
        Guid authContextId)
    {
        DomesticPaymentConsentAuthContextReadResponse authContextResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentAuthContextRead(
                new LocalReadParams
                {
                    Id = authContextId,
                    ModifiedBy = null
                });

        return authContextResponse;
    }

    private async Task<BaseResponse> DomesticPaymentConsentDelete(
        IRequestBuilder requestBuilder,
        Guid domesticPaymentConsentId)
    {
        BaseResponse domesticPaymentConsentResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentDelete(
                new LocalDeleteParams
                {
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null
                });

        return domesticPaymentConsentResponse;
    }

    private async Task<DomesticPaymentConsentCreateResponse> DomesticPaymentConsentCreate(
        DomesticPaymentConsentRequest domesticPaymentConsentRequest)
    {
        // Create DomesticPaymentConsent
        DomesticPaymentConsentCreateResponse domesticPaymentConsentResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentCreate(domesticPaymentConsentRequest);

        // Checks
        if (domesticPaymentConsentRequest.ExternalApiObject is not null)
        {
            domesticPaymentConsentResponse.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            domesticPaymentConsentResponse.ExternalApiResponse.Should().NotBeNull();
        }

        // Read DomesticPaymentConsent
        await DomesticPaymentConsentRead(domesticPaymentConsentResponse.Id, false);

        return domesticPaymentConsentResponse;
    }

    private async Task<DomesticPaymentConsentCreateResponse> DomesticPaymentConsentRead(
        Guid domesticPaymentConsentId,
        bool excludeExternalApiOperation)
    {
        // GET domestic payment consent
        DomesticPaymentConsentCreateResponse domesticPaymentConsentCreateResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentRead(
                new ConsentReadParams
                {
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExcludeExternalApiOperation = excludeExternalApiOperation
                });

        // Checks
        if (excludeExternalApiOperation)
        {
            domesticPaymentConsentCreateResponse.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            domesticPaymentConsentCreateResponse.ExternalApiResponse.Should().NotBeNull();
        }

        return domesticPaymentConsentCreateResponse;
    }

    private static async Task<DomesticPaymentConsentRequest> GetDomesticPaymentConsentRequest(
        BankProfile bankProfile,
        Guid bankRegistrationId,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        DomesticPaymentTemplateRequest domesticPaymentTemplateRequest,
        string initiationInstructionIdentification,
        string initiationEndToEndIdentification)
    {
        var domesticPaymentConsentRequest = new DomesticPaymentConsentRequest
        {
            BankRegistrationId = default, // logging placeholder
            ExternalApiRequest = DomesticPaymentConsentPublicMethods.ResolveExternalApiRequest(
                null,
                domesticPaymentTemplateRequest,
                bankProfile) // Resolve for fuller logging
        };

        await pispFluentRequestLogging
            .AppendToPath("domesticPaymentConsent")
            .AppendToPath("postRequest")
            .WriteFile(domesticPaymentConsentRequest);

        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification =
            initiationInstructionIdentification; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification =
            initiationEndToEndIdentification; // replace logging placeholder
        domesticPaymentConsentRequest.BankRegistrationId = bankRegistrationId; // replace logging placeholder
        domesticPaymentConsentRequest.Reference = testNameUnique;
        domesticPaymentConsentRequest.CreatedBy = modifiedBy;
        return domesticPaymentConsentRequest;
    }
}
