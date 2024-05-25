// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrpConsent;

public class DomesticVrpConsentSubtest(VariableRecurringPaymentsApiClient variableRecurringPaymentsApiClient)
{
    public static ISet<DomesticVrpSubtestEnum> DomesticVrpFunctionalSubtestsSupported(BankProfile bankProfile) =>
        bankProfile.VariableRecurringPaymentsApi is null
            ? new HashSet<DomesticVrpSubtestEnum>() // empty set
            : DomesticVrpSubtestHelper.AllDomesticVrpSubtests;

    public async Task RunTest(
        DomesticVrpSubtestEnum subtestEnum,
        BankProfile bankProfile,
        Guid bankRegistrationId,
        OAuth2ResponseMode defaultResponseMode,
        Func<IServiceScopeContainer> serviceScopeGenerator,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        ConsentAuth? consentAuth,
        BankUser? bankUser)
    {
        bool subtestSkipped = subtestEnum switch
        {
            DomesticVrpSubtestEnum.SweepingVrp => false,
            _ => throw new ArgumentException(
                $"{nameof(subtestEnum)} is not valid {nameof(DomesticVrpSubtestEnum)} or needs to be added to this switch statement.")
        };
        if (subtestSkipped)
        {
            return;
        }

        // Get request builder
        using IServiceScopeContainer serviceScopeContainer = serviceScopeGenerator();
        IRequestBuilder requestBuilder = serviceScopeContainer.RequestBuilder;

        // Create DomesticVrpConsent request
        var domesticVrpTemplateRequest = new DomesticVrpTemplateRequest
        {
            Type = DomesticVrpSubtestHelper.GetDomesticVrpConsentTemplateType(subtestEnum),
            Parameters = new DomesticVrpTemplateParameters
            {
                InstructionIdentification = "placeholder", // logging placeholder
                EndToEndIdentification = "placeholder", // logging placeholder
                IncludeCreditorInInitiation = true,
                IncludeDebtorInInitiation = false,
                ValidFromDateTime = default, // logging placeholder
                ValidToDateTime = default // logging placeholder
            }
        };
        var domesticVrpConsentRequest = new DomesticVrpConsentRequest
        {
            BankRegistrationId = default, // logging placeholder
            ExternalApiRequest = DomesticVrpConsentPublicMethods.ResolveExternalApiRequest(
                null,
                domesticVrpTemplateRequest,
                bankProfile) // Resolve for fuller logging
        };

        await vrpFluentRequestLogging
            .AppendToPath("domesticVrpConsent")
            .AppendToPath("postRequest")
            .WriteFile(domesticVrpConsentRequest);

        // POST domestic payment consent
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        domesticVrpConsentRequest.ExternalApiRequest.Data.ControlParameters.ValidFromDateTime =
            currentTime; // replace logging placeholder
        domesticVrpConsentRequest.ExternalApiRequest.Data.ControlParameters.ValidToDateTime =
            currentTime.AddYears(3); // replace logging placeholder
        domesticVrpConsentRequest.BankRegistrationId = bankRegistrationId; // replace logging placeholder
        domesticVrpConsentRequest.Reference = testNameUnique;
        domesticVrpConsentRequest.CreatedBy = modifiedBy;
        DomesticVrpConsentCreateResponse domesticVrpConsentResp =
            await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                .CreateAsync(domesticVrpConsentRequest);

        // Checks
        domesticVrpConsentResp.Should().NotBeNull();
        domesticVrpConsentResp.Warnings.Should().BeNull();
        domesticVrpConsentResp.ExternalApiResponse.Should().NotBeNull();
        Guid domesticVrpConsentId = domesticVrpConsentResp.Id;

        // GET domestic VRP consent
        DomesticVrpConsentCreateResponse domesticVrpConsentResp2 =
            await requestBuilder.VariableRecurringPayments.DomesticVrpConsents.ReadAsync(
                new ConsentReadParams
                {
                    Id = domesticVrpConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExcludeExternalApiOperation = false
                });

        // Checks
        domesticVrpConsentResp2.Should().NotBeNull();
        domesticVrpConsentResp2.Warnings.Should().BeNull();
        domesticVrpConsentResp2.ExternalApiResponse.Should().NotBeNull();

        // POST auth context
        var authContextRequest = new DomesticVrpConsentAuthContext
        {
            DomesticVrpConsentId = domesticVrpConsentId,
            Reference = testNameUnique,
            CreatedBy = modifiedBy
        };
        DomesticVrpConsentAuthContextCreateResponse authContextResponse =
            await requestBuilder.VariableRecurringPayments
                .DomesticVrpConsents
                .AuthContexts
                .CreateLocalAsync(authContextRequest);

        // Checks
        authContextResponse.Should().NotBeNull();
        authContextResponse.Warnings.Should().BeNull();
        authContextResponse.AuthUrl.Should().NotBeNull();
        Guid authContextId = authContextResponse.Id;
        string authUrl = authContextResponse.AuthUrl;

        // GET auth context
        DomesticVrpConsentAuthContextReadResponse authContextResponse2 =
            await requestBuilder.VariableRecurringPayments.DomesticVrpConsents
                .AuthContexts
                .ReadLocalAsync(
                    new LocalReadParams
                    {
                        Id = authContextId,
                        ModifiedBy = null
                    });
        // Checks
        authContextResponse2.Should().NotBeNull();
        authContextResponse2.Warnings.Should().BeNull();

        // Consent authorisation
        if (consentAuth is not null)
        {
            async Task<bool> AuthIsComplete()
            {
                DomesticVrpConsentCreateResponse consentResponse =
                    await requestBuilder
                        .VariableRecurringPayments
                        .DomesticVrpConsents
                        .ReadAsync(
                            new ConsentReadParams
                            {
                                Id = domesticVrpConsentId,
                                ModifiedBy = null,
                                ExtraHeaders = null,
                                PublicRequestUrlWithoutQuery = null,
                                ExcludeExternalApiOperation = true
                            });
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
                ConsentVariety.DomesticVrpConsent,
                bankUser,
                defaultResponseMode,
                AuthIsComplete);

            // Refresh scope to ensure user token acquired following consent is available
            using IServiceScopeContainer scopedServiceScopeNew = serviceScopeGenerator();
            IRequestBuilder requestBuilderNew = scopedServiceScopeNew.RequestBuilder;

            // Create DomesticVrp FundsConfirmation request
            var domesticVrpConsentFundsConfirmationRequest = new DomesticVrpConsentFundsConfirmationRequest
            {
                ExternalApiRequest =
                    DomesticVrpConsentPublicMethods.ResolveExternalApiFundsConfirmationRequest(
                        null,
                        domesticVrpTemplateRequest,
                        string.Empty, // logging placeholder
                        bankProfile) // Resolve for fuller logging
            };
            await vrpFluentRequestLogging
                .AppendToPath("domesticVrpConsent")
                .AppendToPath("fundsConfirmation")
                .AppendToPath("postRequest")
                .WriteFile(domesticVrpConsentFundsConfirmationRequest);

            // POST consent funds confirmation
            domesticVrpConsentFundsConfirmationRequest.ModifiedBy = modifiedBy;
            DomesticVrpConsentFundsConfirmationResponse domesticPaymentConsentResp4 =
                await requestBuilderNew.VariableRecurringPayments.DomesticVrpConsents
                    .CreateFundsConfirmationAsync(
                        new VrpConsentFundsConfirmationCreateParams
                        {
                            PublicRequestUrlWithoutQuery = null,
                            ExtraHeaders = null,
                            ConsentId = domesticVrpConsentId,
                            Request = domesticVrpConsentFundsConfirmationRequest
                        });

            // Checks
            domesticPaymentConsentResp4.Should().NotBeNull();
            domesticPaymentConsentResp4.Warnings.Should().BeNull();
            domesticPaymentConsentResp4.ExternalApiResponse.Should().NotBeNull();

            // Create DomesticVrp request
            var domesticVrpRequest = new DomesticVrpRequest
            {
                DomesticVrpConsentId = default, // logging placeholder
                ExternalApiRequest = DomesticVrpPublicMethods.ResolveExternalApiRequest(
                    null,
                    domesticVrpTemplateRequest,
                    string.Empty, // logging placeholder
                    bankProfile) // resolve for fuller logging
            };
            await vrpFluentRequestLogging
                .AppendToPath("domesticVrp")
                .AppendToPath("postRequest")
                .WriteFile(domesticVrpRequest);

            // POST domestic VRP
            domesticVrpRequest.ExternalApiRequest.Data.Instruction.InstructionIdentification =
                Guid.NewGuid().ToString("N"); // replace logging placeholder
            domesticVrpRequest.ExternalApiRequest.Data.Instruction.EndToEndIdentification =
                Guid.NewGuid().ToString("N"); // replace logging placeholder
            domesticVrpRequest.DomesticVrpConsentId = domesticVrpConsentId;
            domesticVrpRequest.ModifiedBy = modifiedBy;
            DomesticVrpResponse domesticVrpResp =
                await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                    .CreateAsync(
                        domesticVrpRequest,
                        new ConsentExternalCreateParams
                        {
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null
                        });

            // Checks
            domesticVrpResp.Should().NotBeNull();
            domesticVrpResp.Warnings.Should().BeNull();
            domesticVrpResp.ExternalApiResponse.Should().NotBeNull();
            string domesticVrpExternalId = domesticVrpResp.ExternalApiResponse.Data.DomesticVRPId;

            // GET domestic payment
            DomesticVrpResponse domesticVrpResp2 =
                await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                    .ReadAsync(
                        new ConsentExternalEntityReadParams
                        {
                            ConsentId = domesticVrpConsentId,
                            ModifiedBy = null,
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null,
                            ExternalApiId = domesticVrpExternalId
                        });

            // Checks
            domesticVrpResp2.Should().NotBeNull();
            domesticVrpResp2.Warnings.Should().BeNull();
            domesticVrpResp2.ExternalApiResponse.Should().NotBeNull();

            // GET domestic VRP payment details
            if (bankProfile.VariableRecurringPaymentsApiSettings.UseDomesticVrpGetPaymentDetailsEndpoint)
            {
                DomesticVrpPaymentDetailsResponse paymentDetailsResponse =
                    await requestBuilderNew.VariableRecurringPayments.DomesticVrps
                        .ReadPaymentDetailsAsync(
                            new ConsentExternalEntityReadParams
                            {
                                ConsentId = domesticVrpConsentId,
                                ModifiedBy = null,
                                ExtraHeaders = null,
                                PublicRequestUrlWithoutQuery = null,
                                ExternalApiId = domesticVrpExternalId
                            });

                // Checks
                paymentDetailsResponse.Should().NotBeNull();
                paymentDetailsResponse.Warnings.Should().BeNull();
                paymentDetailsResponse.ExternalApiResponse.Should().NotBeNull();
            }

            // DELETE domestic payment consent
            var excludeExternalApiOperation = false;
            BaseResponse domesticVrpConsentResp3 = await requestBuilderNew.VariableRecurringPayments
                .DomesticVrpConsents
                .DeleteAsync(
                    new ConsentDeleteParams
                    {
                        Id = domesticVrpConsentId,
                        ModifiedBy = null,
                        ExtraHeaders = null,
                        ExcludeExternalApiOperation = excludeExternalApiOperation
                    });

            // Checks
            domesticVrpConsentResp3.Should().NotBeNull();
            domesticVrpConsentResp3.Warnings.Should().BeNull();
        }
    }
}
