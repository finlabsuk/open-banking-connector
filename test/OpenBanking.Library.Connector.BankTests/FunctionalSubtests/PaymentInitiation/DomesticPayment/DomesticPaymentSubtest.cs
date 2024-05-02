// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;
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

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;

public static class DomesticPaymentSubtest
{
    public static ISet<DomesticPaymentSubtestEnum> DomesticPaymentFunctionalSubtestsSupported(
        BankProfile bankProfile) =>
        bankProfile.PaymentInitiationApi is null
            ? new HashSet<DomesticPaymentSubtestEnum>()
            : DomesticPaymentSubtestHelper.AllDomesticPaymentSubtests;

    public static async Task RunTest(
        DomesticPaymentSubtestEnum subtestEnum,
        BankProfile bankProfile,
        Guid bankRegistrationId,
        OAuth2ResponseMode defaultResponseMode,
        Func<IServiceScopeContainer> serviceScopeGenerator,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        ConsentAuth? consentAuth,
        BankUser? bankUser)
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

        var initiationInstructionIdentification = Guid.NewGuid().ToString("N");
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification =
            initiationInstructionIdentification; // replace logging placeholder
        var initiationEndToEndIdentification = Guid.NewGuid().ToString("N");
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification =
            initiationEndToEndIdentification; // replace logging placeholder
        domesticPaymentConsentRequest.BankRegistrationId = bankRegistrationId; // replace logging placeholder
        domesticPaymentConsentRequest.Reference = testNameUnique;
        domesticPaymentConsentRequest.CreatedBy = modifiedBy;
        DomesticPaymentConsentCreateResponse domesticPaymentConsentResp =
            await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                .CreateAsync(domesticPaymentConsentRequest);

        // Checks
        domesticPaymentConsentResp.Should().NotBeNull();
        domesticPaymentConsentResp.Warnings.Should().BeNull();
        domesticPaymentConsentResp.ExternalApiResponse.Should().NotBeNull();
        Guid domesticPaymentConsentId = domesticPaymentConsentResp.Id;

        // GET domestic payment consent
        DomesticPaymentConsentCreateResponse domesticPaymentConsentResp2 =
            await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                .ReadAsync(domesticPaymentConsentId);

        // Checks
        domesticPaymentConsentResp2.Should().NotBeNull();
        domesticPaymentConsentResp2.Warnings.Should().BeNull();
        domesticPaymentConsentResp2.ExternalApiResponse.Should().NotBeNull();

        // POST auth context
        var authContextRequest = new DomesticPaymentConsentAuthContext
        {
            DomesticPaymentConsentId = domesticPaymentConsentId,
            Reference = testNameUnique,
            CreatedBy = modifiedBy
        };
        DomesticPaymentConsentAuthContextCreateResponse authContextResponse =
            await requestBuilder.PaymentInitiation
                .DomesticPaymentConsents
                .AuthContexts
                .CreateLocalAsync(authContextRequest);

        // Checks
        authContextResponse.Should().NotBeNull();
        authContextResponse.Warnings.Should().BeNull();
        authContextResponse.AuthUrl.Should().NotBeNull();
        Guid authContextId = authContextResponse.Id;
        string authUrl = authContextResponse.AuthUrl;

        // GET auth context
        DomesticPaymentConsentAuthContextReadResponse authContextResponse2 =
            await requestBuilder.PaymentInitiation.DomesticPaymentConsents
                .AuthContexts
                .ReadLocalAsync(authContextId);
        // Checks
        authContextResponse2.Should().NotBeNull();
        authContextResponse2.Warnings.Should().BeNull();

        // Consent authorisation
        if (consentAuth is not null)
        {
            async Task<bool> AuthIsComplete()
            {
                DomesticPaymentConsentCreateResponse consentResponse =
                    await requestBuilder
                        .PaymentInitiation
                        .DomesticPaymentConsents
                        .ReadAsync(
                            domesticPaymentConsentId,
                            modifiedBy,
                            null,
                            false);
                return consentResponse.Created < consentResponse.AuthContextModified;
            }

            if (bankUser is null)
            {
                throw new ArgumentException("No user specified for consent auth.");
            }

            // Authorise consent in UI via Playwright
            await consentAuth.AutomatedAuthAsync(
                authUrl,
                bankProfile,
                ConsentVariety.DomesticPaymentConsent,
                bankUser,
                defaultResponseMode,
                AuthIsComplete);

            // Refresh scope to ensure user token acquired following consent is available
            using IServiceScopeContainer scopedServiceScopeNew = serviceScopeGenerator();
            IRequestBuilder requestBuilderNew = scopedServiceScopeNew.RequestBuilder;

            // GET consent funds confirmation
            DomesticPaymentConsentFundsConfirmationResponse domesticPaymentConsentResp4 =
                await requestBuilderNew.PaymentInitiation.DomesticPaymentConsents
                    .ReadFundsConfirmationAsync(
                        new ConsentBaseReadParams
                        {
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null,
                            Id = domesticPaymentConsentId,
                            ModifiedBy = null
                        });

            // Checks
            domesticPaymentConsentResp4.Should().NotBeNull();
            domesticPaymentConsentResp4.Warnings.Should().BeNull();
            domesticPaymentConsentResp4.ExternalApiResponse.Should().NotBeNull();

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

        // DELETE domestic payment consent
        BaseResponse domesticPaymentConsentResp3 = await requestBuilder
            .PaymentInitiation
            .DomesticPaymentConsents
            .DeleteLocalAsync(domesticPaymentConsentId, modifiedBy);

        // Checks
        domesticPaymentConsentResp3.Should().NotBeNull();
        domesticPaymentConsentResp3.Warnings.Should().BeNull();
    }
}
