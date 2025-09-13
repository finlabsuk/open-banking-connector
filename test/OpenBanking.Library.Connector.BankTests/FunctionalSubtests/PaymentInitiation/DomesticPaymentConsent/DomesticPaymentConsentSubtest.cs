// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
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
        bool pispUseV4,
        OAuth2ResponseMode defaultResponseMode,
        bool testAuth,
        string referenceName,
        PaymentsEnv paymentsEnv,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        ConsentAuth consentAuth,
        string authUrlLeftPart,
        BankUser? bankUser)
    {
        // Create DomesticPaymentConsent
        var instructionIdentification = Guid.NewGuid().ToString("N");
        string endToEndIdentification = Guid.NewGuid().ToString("N")[..31];
        double amountDouble = Random.Shared.Next(10, 300) / 100.0;
        var amount = amountDouble.ToString("F2");
        DomesticPaymentConsentRequest domesticPaymentConsentRequest = await GetDomesticPaymentConsentRequest(
            bankRegistrationId,
            pispUseV4,
            testNameUnique,
            modifiedBy,
            pispFluentRequestLogging,
            instructionIdentification,
            endToEndIdentification,
            amount,
            referenceName,
            paymentsEnv,
            bankProfile.PaymentInitiationApiSettings);
        DomesticPaymentConsentCreateResponse domesticPaymentConsentCreateResponse =
            await paymentInitiationApiClient.DomesticPaymentConsentCreate(
                domesticPaymentConsentRequest,
                bankProfile.CustomBehaviour?.DomesticPaymentConsent);
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

        // Perform testing which requires auth
        if (testAuth)
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
                    },
                    bankProfile.CustomBehaviour?.DomesticPaymentConsent);

            // Create DomesticPayment
            DomesticPaymentRequest domesticPaymentRequest = await GetDomesticPaymentRequest(
                domesticPaymentConsentId,
                pispUseV4,
                modifiedBy,
                pispFluentRequestLogging,
                instructionIdentification,
                endToEndIdentification,
                amount,
                referenceName,
                paymentsEnv,
                bankProfile.PaymentInitiationApiSettings);
            DomesticPaymentResponse domesticPaymentResp =
                await paymentInitiationApiClient.DomesticPaymentCreate(
                    domesticPaymentRequest,
                    new ConsentExternalCreateParams
                    {
                        ExtraHeaders = null,
                        PublicRequestUrlWithoutQuery = null
                    },
                    bankProfile.CustomBehaviour?.DomesticPayment);
            string domesticPaymentExternalId = domesticPaymentResp.ExternalApiResponse.Data.DomesticPaymentId;

            // Allow time for payment to be processed
            await Task.Delay(TimeSpan.FromSeconds(3));

            // Read DomesticPayment
            DomesticPaymentResponse domesticPaymentReadResponse = await paymentInitiationApiClient.DomesticPaymentRead(
                new ConsentExternalEntityReadParams
                {
                    ConsentId = domesticPaymentConsentId,
                    ModifiedBy = null,
                    ExtraHeaders = null,
                    PublicRequestUrlWithoutQuery = null,
                    ExternalApiId = domesticPaymentExternalId
                },
                bankProfile.CustomBehaviour?.DomesticPayment);

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
        Guid domesticPaymentConsentId,
        bool pispUseV4,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        string instructionIdentification,
        string endToEndIdentification,
        string amount,
        string referenceName,
        PaymentsEnv paymentsEnv,
        PaymentInitiationApiSettings paymentInitiationApiSettings)
    {
        // Create DomesticPayment request
        var externalApiRequest = new PaymentInitiationModelsPublic.OBWriteDomestic2
        {
            Data = new PaymentInitiationModelsPublic.Data
            {
                ConsentId = "",
                Initiation = new PaymentInitiationModelsPublic.Initiation
                {
                    InstructionIdentification = "placeholder", // logging placeholder
                    EndToEndIdentification = "placeholder", // logging placeholder
                    InstructedAmount = new PaymentInitiationModelsPublic.InstructedAmount
                    {
                        Amount = "placeholder", // logging placeholder
                        Currency = "GBP"
                    },
                    CreditorAccount = new PaymentInitiationModelsPublic.CreditorAccount
                    {
                        SchemeName = "placeholder", // logging placeholder
                        Identification = "placeholder", // logging placeholder
                        Name = "placeholder" // logging placeholder
                    },
                    RemittanceInformation =
                        new PaymentInitiationModelsPublic.OBRemittanceInformation2
                        {
                            Structured =
                            [
                                new PaymentInitiationModelsPublic.OBRemittanceInformationStructured
                                {
                                    CreditorReferenceInformation =
                                        new PaymentInitiationModelsPublic.CreditorReferenceInformation
                                        {
                                            Reference = "placeholder" // logging placeholder 
                                        }
                                }
                            ]
                        }
                }
            },
            Risk = new PaymentInitiationModelsPublic.OBRisk1
            {
                PaymentContextCode =
                    pispUseV4 ? PaymentInitiationModelsPublic.OBRisk1PaymentContextCode.TransferToSelf : null,
                V3PaymentContextCode = pispUseV4 ? null :
                    paymentInitiationApiSettings.PreferPartyToPartyPaymentContextCode ? PaymentInitiationModelsV3p1p11
                        .OBRisk1PaymentContextCode
                        .PartyToParty : PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode
                        .TransferToSelf,
                ContractPresentIndicator = paymentInitiationApiSettings.UseContractPresentIndicator ? true : null
            }
        };
        var domesticPaymentRequest = new DomesticPaymentRequest
        {
            DomesticPaymentConsentId = default, // logging placeholder
            ExternalApiRequest = paymentInitiationApiSettings
                .DomesticPaymentExternalApiRequestAdjustments(
                    externalApiRequest), // customise external API request using bank profile
            ModifiedBy = "placeholder" // logging placeholder
        };
        await pispFluentRequestLogging
            .AppendToPath("domesticPayment")
            .AppendToPath("postRequest")
            .WriteFile(domesticPaymentRequest);

        domesticPaymentRequest.DomesticPaymentConsentId = domesticPaymentConsentId; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification =
            instructionIdentification; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification =
            endToEndIdentification; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.InstructedAmount.Amount =
            amount; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.SchemeName =
            paymentsEnv.BankAccountSchemeName; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Identification =
            paymentsEnv.BankAccountId; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Name =
            paymentsEnv.BankAccountName; // replace logging placeholder
        domesticPaymentRequest.ExternalApiRequest.Data.Initiation.RemittanceInformation!.Structured =
        [
            new PaymentInitiationModelsPublic.OBRemittanceInformationStructured
            {
                CreditorReferenceInformation =
                    new PaymentInitiationModelsPublic.CreditorReferenceInformation
                    {
                        Reference = "DP " + referenceName // replace logging placeholder
                    }
            }
        ];
        domesticPaymentRequest.ModifiedBy = modifiedBy;

        return domesticPaymentRequest;
    }

    private static async Task<DomesticPaymentConsentRequest> GetDomesticPaymentConsentRequest(
        Guid bankRegistrationId,
        bool pispUseV4,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder pispFluentRequestLogging,
        string instructionIdentification,
        string endToEndIdentification,
        string amount,
        string referenceName,
        PaymentsEnv paymentsEnv,
        PaymentInitiationApiSettings paymentInitiationApiSettings)
    {
        var externalApiRequest = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4
        {
            Data = new PaymentInitiationModelsPublic.Data2
            {
                ReadRefundAccount =
                    paymentInitiationApiSettings.UseReadRefundAccount
                        ? PaymentInitiationModelsPublic.Data2ReadRefundAccount.Yes
                        : null,
                Initiation = new PaymentInitiationModelsPublic.Initiation2
                {
                    InstructionIdentification = "placeholder", // logging placeholder
                    EndToEndIdentification = "placeholder", // logging placeholder
                    InstructedAmount = new PaymentInitiationModelsPublic.InstructedAmount2
                    {
                        Amount = "placeholder", // logging placeholder
                        Currency = "GBP"
                    },
                    CreditorAccount = new PaymentInitiationModelsPublic.CreditorAccount2
                    {
                        SchemeName = "placeholder", // logging placeholder
                        Identification = "placeholder", // logging placeholder
                        Name = "placeholder" // logging placeholder
                    },
                    RemittanceInformation =
                        new PaymentInitiationModelsPublic.OBRemittanceInformation2
                        {
                            Structured =
                            [
                                new PaymentInitiationModelsPublic.OBRemittanceInformationStructured
                                {
                                    CreditorReferenceInformation =
                                        new PaymentInitiationModelsPublic.CreditorReferenceInformation
                                        {
                                            Reference = "placeholder" // logging placeholder 
                                        }
                                }
                            ]
                        }
                }
            },
            Risk = new PaymentInitiationModelsPublic.OBRisk1
            {
                PaymentContextCode =
                    pispUseV4 ? PaymentInitiationModelsPublic.OBRisk1PaymentContextCode.TransferToSelf : null,
                V3PaymentContextCode = pispUseV4 ? null :
                    paymentInitiationApiSettings.PreferPartyToPartyPaymentContextCode ? PaymentInitiationModelsV3p1p11
                        .OBRisk1PaymentContextCode
                        .PartyToParty : PaymentInitiationModelsV3p1p11.OBRisk1PaymentContextCode
                        .TransferToSelf,
                ContractPresentIndicator = paymentInitiationApiSettings.UseContractPresentIndicator ? true : null
            }
        };

        var domesticPaymentConsentRequest = new DomesticPaymentConsentRequest
        {
            BankRegistrationId = default, // logging placeholder
            ExternalApiRequest = paymentInitiationApiSettings
                .DomesticPaymentConsentExternalApiRequestAdjustments(
                    externalApiRequest), // customise external API request using bank profile
            Reference = "placeholder", // logging placeholder
            CreatedBy = "placeholder" // logging placeholder
        };

        await pispFluentRequestLogging
            .AppendToPath("domesticPaymentConsent")
            .AppendToPath("postRequest")
            .WriteFile(domesticPaymentConsentRequest);

        domesticPaymentConsentRequest.BankRegistrationId = bankRegistrationId; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.InstructionIdentification =
            instructionIdentification; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.EndToEndIdentification =
            endToEndIdentification; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.InstructedAmount.Amount =
            amount; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.SchemeName =
            paymentsEnv.BankAccountSchemeName; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Identification =
            paymentsEnv.BankAccountId; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Name =
            paymentsEnv.BankAccountName; // replace logging placeholder
        domesticPaymentConsentRequest.ExternalApiRequest.Data.Initiation.RemittanceInformation!.Structured =
        [
            new PaymentInitiationModelsPublic.OBRemittanceInformationStructured
            {
                CreditorReferenceInformation =
                    new PaymentInitiationModelsPublic.CreditorReferenceInformation
                    {
                        Reference = "DP " + referenceName // replace logging placeholder
                    }
            }
        ];
        domesticPaymentConsentRequest.Reference = testNameUnique; // replace logging placeholder
        domesticPaymentConsentRequest.CreatedBy = modifiedBy; // replace logging placeholder

        return domesticPaymentConsentRequest;
    }
}
