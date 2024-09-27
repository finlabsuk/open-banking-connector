// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Web;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.
    DomesticPaymentConsent;

public class DomesticPaymentConsentSubtest(
    PaymentInitiationApiClient paymentInitiationApiClient,
    AuthContextsApiClient authContextsApiClient)
{
    public static ISet<DomesticPaymentSubtestEnum> DomesticPaymentFunctionalSubtestsSupported(
        BankProfile bankProfile) =>
        DomesticPaymentSubtestHelper.AllDomesticPaymentSubtests;

    public async Task RunTest(
        DomesticPaymentSubtestEnum subtestEnum,
        BankProfile bankProfile,
        Guid bankRegistrationId,
        OAuth2ResponseMode defaultResponseMode,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        ConsentAuth consentAuth,
        string authUrlLeftPart,
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

        var testDomesticPaymentConsentAuth = true;

        // Create DomesticPaymentConsent
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
        DomesticPaymentConsentCreateResponse domesticPaymentConsentCreateResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentCreate(domesticPaymentConsentRequest);
        Guid domesticPaymentConsentId = domesticPaymentConsentCreateResponse.Id;

        // Read DomesticPaymentConsent
        DomesticPaymentConsentCreateResponse domesticPaymentConsentReadResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentRead(
                new ConsentReadParams
                {
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExcludeExternalApiOperation = false
                });

        // Consent authorisation
        if (testDomesticPaymentConsentAuth)
        {
            // Create redirect observer which will "catch" redirect
            async Task<AuthContextUpdateAuthResultResponse> ProcessRedirectFcn(TestingAuthResult result)
            {
                return await authContextsApiClient.RedirectDelegate(result.RedirectParameters);
            }

            async Task<DomesticPaymentConsentAuthContextCreateResponse>
                DomesticPaymentConsentAuthContextCreateResponse()
            {
                // Create AuthContext
                var authContextRequest = new DomesticPaymentConsentAuthContext
                {
                    DomesticPaymentConsentId = domesticPaymentConsentId,
                    Reference = testNameUnique + "_DomesticPaymentConsent",
                    CreatedBy = modifiedBy
                };
                DomesticPaymentConsentAuthContextCreateResponse authContextCreateResponse =
                    await paymentInitiationApiClient.DomesticPaymentConsentAuthContextCreate(authContextRequest);

                // Read AuthContext
                DomesticPaymentConsentAuthContextReadResponse authContextReadResponse =
                    await paymentInitiationApiClient.DomesticPaymentConsentAuthContextRead(
                        new LocalReadParams
                        {
                            Id = authContextCreateResponse.Id,
                            ModifiedBy = null
                        });

                return authContextCreateResponse;
            }

            var redirectObserver = new RedirectObserver
            {
                ConsentId = domesticPaymentConsentId,
                ConsentType = ConsentType.DomesticPaymentConsent,
                ProcessRedirectFcn = ProcessRedirectFcn,
                DomesticPaymentConsentAuthContextCreateFcn = DomesticPaymentConsentAuthContextCreateResponse
            };

            // Determine auth URL
            string authUrl;
            if (bankProfile.SupportsSca)
            {
                DomesticPaymentConsentAuthContextCreateResponse authContext =
                    await DomesticPaymentConsentAuthContextCreateResponse();
                authUrl = authContext.AuthUrl;
                redirectObserver.AssociatedStates.Add(authContext.State);
            }
            else
            {
                authUrl = $"{authUrlLeftPart}/dev1/pisp/domestic-payment-consents/{domesticPaymentConsentId}/auth";
            }

            // Perform auth
            AuthContextUpdateAuthResultResponse authResultResponse = await consentAuth.PerformAuth(
                redirectObserver,
                authUrl,
                bankProfile.SupportsSca,
                defaultResponseMode,
                bankUser,
                bankProfile.BankProfileEnum,
                ConsentVariety.DomesticPaymentConsent);

            // Read DomesticPaymentConsent FundsConfirmation
            DomesticPaymentConsentFundsConfirmationResponse domesticPaymentConsentFundsConfirmationResponse =
                await paymentInitiationApiClient.DomesticPaymentConsentReadFundsConfirmation(
                    new ConsentBaseReadParams
                    {
                        ExtraHeaders = null,
                        PublicRequestUrlWithoutQuery = null,
                        Id = domesticPaymentConsentId,
                        ModifiedBy = null
                    });

            // Create DomesticPayment
            DomesticPaymentRequest domesticPaymentRequest = await GetDomesticPaymentRequest(
                bankProfile,
                modifiedBy,
                pispFluentRequestLogging,
                domesticPaymentTemplateRequest,
                initiationInstructionIdentification,
                initiationEndToEndIdentification,
                domesticPaymentConsentId);
            DomesticPaymentResponse domesticPaymentResp =
                await paymentInitiationApiClient.DomesticPaymentCreate(
                    domesticPaymentRequest,
                    new ConsentExternalCreateParams
                    {
                        ExtraHeaders = null,
                        PublicRequestUrlWithoutQuery = null
                    });
            string domesticPaymentExternalId = domesticPaymentResp.ExternalApiResponse.Data.DomesticPaymentId;

            // Read DomesticPayment
            DomesticPaymentResponse domesticPaymentReadResponse = await paymentInitiationApiClient.DomesticPaymentRead(
                new ConsentExternalEntityReadParams
                {
                    ConsentId = domesticPaymentConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExternalApiId = domesticPaymentExternalId
                });

            // Read DomesticPayment PaymentDetails
            if (bankProfile.PaymentInitiationApiSettings.UseDomesticPaymentGetPaymentDetailsEndpoint)
            {
                DomesticPaymentPaymentDetailsResponse domesticPaymentReadPaymentDetailsResponse =
                    await paymentInitiationApiClient.DomesticPaymentReadPaymentDetails(
                        new ConsentExternalEntityReadParams
                        {
                            ConsentId = domesticPaymentConsentId,
                            ModifiedBy = null,
                            ExtraHeaders = null,
                            PublicRequestUrlWithoutQuery = null,
                            ExternalApiId = domesticPaymentExternalId
                        });
            }
        }

        // Delete DomesticPaymentConsent
        BaseResponse domesticPaymentConsentDeleteResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentDelete(
                new LocalDeleteParams
                {
                    Id = domesticPaymentConsentId,
                    ModifiedBy = null
                });
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
