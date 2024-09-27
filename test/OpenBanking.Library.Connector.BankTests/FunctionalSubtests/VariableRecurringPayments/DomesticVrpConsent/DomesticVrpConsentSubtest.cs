// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Web;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrpConsent;

public class DomesticVrpConsentSubtest(
    VariableRecurringPaymentsApiClient variableRecurringPaymentsApiClient,
    AuthContextsApiClient authContextsApiClient)
{
    public static ISet<DomesticVrpSubtestEnum> DomesticVrpFunctionalSubtestsSupported(BankProfile bankProfile) =>
        DomesticVrpSubtestHelper.AllDomesticVrpSubtests;

    public async Task RunTest(
        DomesticVrpSubtestEnum subtestEnum,
        BankProfile bankProfile,
        Guid bankRegistrationId,
        OAuth2ResponseMode defaultResponseMode,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        ConsentAuth consentAuth,
        string authUrlLeftPart,
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

        var testDomesticVrpConsentAuth = true;

        // Create DomesticVrpConsent
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
        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        DateTimeOffset controlParametersValidFromDateTime = currentTime;
        DateTimeOffset controlParametersValidToDateTime = currentTime.AddYears(3);
        DomesticVrpConsentRequest domesticVrpConsentRequest = await GetDomesticVrpConsentRequest(
            bankProfile,
            bankRegistrationId,
            testNameUnique,
            modifiedBy,
            vrpFluentRequestLogging,
            domesticVrpTemplateRequest,
            controlParametersValidFromDateTime,
            controlParametersValidToDateTime);
        DomesticVrpConsentCreateResponse domesticVrpConsentCreateResponse =
            await variableRecurringPaymentsApiClient.DomesticVrpConsentCreate(domesticVrpConsentRequest);
        Guid domesticVrpConsentId = domesticVrpConsentCreateResponse.Id;

        // Read DomesticVrpConsent
        DomesticVrpConsentCreateResponse domesticVrpConsentReadResponse =
            await variableRecurringPaymentsApiClient.DomesticVrpConsentRead(
                new ConsentReadParams
                {
                    Id = domesticVrpConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExcludeExternalApiOperation = false
                });

        // Consent authorisation
        if (testDomesticVrpConsentAuth)
        {
            // Create redirect observer which will "catch" redirect
            async Task<AuthContextUpdateAuthResultResponse> ProcessRedirectFcn(TestingAuthResult result)
            {
                return await authContextsApiClient.RedirectDelegate(result.RedirectParameters);
            }

            async Task<DomesticVrpConsentAuthContextCreateResponse>
                DomesticVrpConsentAuthContextCreateResponse()
            {
                // Create AuthContext
                var authContextRequest = new DomesticVrpConsentAuthContext
                {
                    DomesticVrpConsentId = domesticVrpConsentId,
                    Reference = testNameUnique + "_DomesticVrpConsent",
                    CreatedBy = modifiedBy
                };
                DomesticVrpConsentAuthContextCreateResponse authContextCreateResponse =
                    await variableRecurringPaymentsApiClient.DomesticVrpConsentAuthContextCreate(authContextRequest);

                // Read AuthContext
                DomesticVrpConsentAuthContextReadResponse authContextReadResponse =
                    await variableRecurringPaymentsApiClient.DomesticVrpConsentAuthContextRead(
                        new LocalReadParams
                        {
                            Id = authContextCreateResponse.Id,
                            ModifiedBy = null
                        });

                return authContextCreateResponse;
            }

            var redirectObserver = new RedirectObserver
            {
                ConsentId = domesticVrpConsentId,
                ConsentType = ConsentType.DomesticVrpConsent,
                ProcessRedirectFcn = ProcessRedirectFcn,
                DomesticVrpConsentAuthContextCreateFcn = DomesticVrpConsentAuthContextCreateResponse
            };

            // Determine auth URL
            string authUrl;
            if (bankProfile.SupportsSca)
            {
                DomesticVrpConsentAuthContextCreateResponse authContext =
                    await DomesticVrpConsentAuthContextCreateResponse();
                authUrl = authContext.AuthUrl;
                redirectObserver.AssociatedStates.Add(authContext.State);
            }
            else
            {
                authUrl = $"{authUrlLeftPart}/dev1/vrp/domestic-vrp-consents/{domesticVrpConsentId}/auth";
            }

            // Perform auth
            AuthContextUpdateAuthResultResponse authResultResponse = await consentAuth.PerformAuth(
                redirectObserver,
                authUrl,
                bankProfile.SupportsSca,
                defaultResponseMode,
                bankUser,
                bankProfile.BankProfileEnum,
                ConsentVariety.DomesticVrpConsent);

            // Create DomesticVrp FundsConfirmation
            DomesticVrpConsentFundsConfirmationRequest domesticVrpConsentFundsConfirmationRequest =
                await GetDomesticVrpConsentFundsConfirmationRequest(
                    bankProfile,
                    modifiedBy,
                    vrpFluentRequestLogging,
                    domesticVrpTemplateRequest);
            DomesticVrpConsentFundsConfirmationResponse domesticPaymentConsentResp4 =
                await variableRecurringPaymentsApiClient.DomesticVrpConsentCreateFundsConfirmation(
                    new VrpConsentFundsConfirmationCreateParams
                    {
                        PublicRequestUrlWithoutQuery = null,
                        ExtraHeaders = null,
                        ConsentId = domesticVrpConsentId,
                        Request = domesticVrpConsentFundsConfirmationRequest
                    });

            // Create DomesticVrp
            var instructionInstructionIdentification = Guid.NewGuid().ToString("N");
            var instructionEndToEndIdentification = Guid.NewGuid().ToString("N");
            DomesticVrpRequest domesticVrpRequest = await GetDomesticVrpRequest(
                bankProfile,
                modifiedBy,
                vrpFluentRequestLogging,
                domesticVrpTemplateRequest,
                domesticVrpConsentId,
                instructionInstructionIdentification,
                instructionEndToEndIdentification);
            DomesticVrpResponse domesticVrpResp =
                await variableRecurringPaymentsApiClient.DomesticVrpCreate(
                    domesticVrpRequest,
                    new ConsentExternalCreateParams
                    {
                        ExtraHeaders = null,
                        PublicRequestUrlWithoutQuery = null
                    });
            string domesticVrpExternalId = domesticVrpResp.ExternalApiResponse.Data.DomesticVRPId;

            // Read DomesticVrp
            DomesticVrpResponse domesticVrpResp2 =
                await variableRecurringPaymentsApiClient.DomesticVrpRead(
                    new ConsentExternalEntityReadParams
                    {
                        ConsentId = domesticVrpConsentId,
                        ModifiedBy = null,
                        ExtraHeaders = null,
                        PublicRequestUrlWithoutQuery = null,
                        ExternalApiId = domesticVrpExternalId
                    });

            // Read DomesticVrp PaymentDetails
            if (bankProfile.VariableRecurringPaymentsApiSettings.UseDomesticVrpGetPaymentDetailsEndpoint)
            {
                DomesticVrpPaymentDetailsResponse paymentDetailsResponse =
                    await variableRecurringPaymentsApiClient.DomesticVrpReadPaymentDetails(
                        new ConsentExternalEntityReadParams
                        {
                            ConsentId = domesticVrpConsentId,
                            ModifiedBy = null,
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null,
                            ExternalApiId = domesticVrpExternalId
                        });
            }
        }

        // Delete DomesticVrpConsent
        var excludeExternalApiOperation = false;
        BaseResponse domesticVrpConsentResp3 = await variableRecurringPaymentsApiClient.DomesticVrpConsentDelete(
            new ConsentDeleteParams
            {
                Id = domesticVrpConsentId,
                ModifiedBy = null,
                ExtraHeaders = null,
                ExcludeExternalApiOperation = excludeExternalApiOperation
            });
    }

    private static async Task<DomesticVrpConsentFundsConfirmationRequest> GetDomesticVrpConsentFundsConfirmationRequest(
        BankProfile bankProfile,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        DomesticVrpTemplateRequest domesticVrpTemplateRequest)
    {
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

        domesticVrpConsentFundsConfirmationRequest.ModifiedBy = modifiedBy;

        return domesticVrpConsentFundsConfirmationRequest;
    }

    private static async Task<DomesticVrpRequest> GetDomesticVrpRequest(
        BankProfile bankProfile,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        DomesticVrpTemplateRequest domesticVrpTemplateRequest,
        Guid domesticVrpConsentId,
        string instructionInstructionIdentification,
        string instructionEndToEndIdentification)
    {
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

        domesticVrpRequest.ExternalApiRequest.Data.Instruction.InstructionIdentification =
            instructionInstructionIdentification; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Instruction.EndToEndIdentification =
            instructionEndToEndIdentification; // replace logging placeholder
        domesticVrpRequest.DomesticVrpConsentId = domesticVrpConsentId;
        domesticVrpRequest.ModifiedBy = modifiedBy;

        return domesticVrpRequest;
    }

    private static async Task<DomesticVrpConsentRequest> GetDomesticVrpConsentRequest(
        BankProfile bankProfile,
        Guid bankRegistrationId,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        DomesticVrpTemplateRequest domesticVrpTemplateRequest,
        DateTimeOffset controlParametersValidFromDateTime,
        DateTimeOffset controlParametersValidToDateTime)
    {
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

        domesticVrpConsentRequest.ExternalApiRequest.Data.ControlParameters.ValidFromDateTime =
            controlParametersValidFromDateTime; // replace logging placeholder
        domesticVrpConsentRequest.ExternalApiRequest.Data.ControlParameters.ValidToDateTime =
            controlParametersValidToDateTime; // replace logging placeholder
        domesticVrpConsentRequest.BankRegistrationId = bankRegistrationId; // replace logging placeholder
        domesticVrpConsentRequest.Reference = testNameUnique;
        domesticVrpConsentRequest.CreatedBy = modifiedBy;

        return domesticVrpConsentRequest;
    }
}
