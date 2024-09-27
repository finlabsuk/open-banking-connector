﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using DomesticVrpConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;

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
        PaymentsEnvFile paymentsEnvFile,
        string creditorAccount,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        ConsentAuth consentAuth,
        string authUrlLeftPart,
        BankUser? bankUser,
        IServiceProvider appServiceProvider,
        IMemoryCache memoryCache)
    {
        var testDomesticVrpConsentAuth = true;

        // Create DomesticVrpConsent
        if (!paymentsEnvFile.TryGetValue(
                creditorAccount,
                out PaymentsEnv? paymentsEnv))
        {
            throw new InvalidOperationException($"Creditor account {creditorAccount} specified but not found.");
        }

        DomesticVrpConsentRequest domesticVrpConsentRequest = await GetDomesticVrpConsentRequest(
            bankRegistrationId,
            testNameUnique,
            modifiedBy,
            vrpFluentRequestLogging,
            paymentsEnv,
            bankProfile.VariableRecurringPaymentsApiSettings);
        DomesticVrpConsentCreateResponse domesticVrpConsentCreateResponse =
            await variableRecurringPaymentsApiClient.DomesticVrpConsentCreate(
                domesticVrpConsentRequest,
                bankProfile.CustomBehaviour?.DomesticVrpConsent);
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
            double amountDouble = Random.Shared.Next(10, 300) / 100.0;
            var amount = amountDouble.ToString("F2");
            DomesticVrpConsentFundsConfirmationRequest domesticVrpConsentFundsConfirmationRequest =
                await GetDomesticVrpConsentFundsConfirmationRequest(
                    modifiedBy,
                    vrpFluentRequestLogging,
                    amount,
                    bankProfile.VariableRecurringPaymentsApiSettings);
            DomesticVrpConsentFundsConfirmationResponse domesticPaymentConsentResp4 =
                await variableRecurringPaymentsApiClient.DomesticVrpConsentCreateFundsConfirmation(
                    new VrpConsentFundsConfirmationCreateParams
                    {
                        PublicRequestUrlWithoutQuery = null,
                        ExtraHeaders = null,
                        ConsentId = domesticVrpConsentId,
                        Request = domesticVrpConsentFundsConfirmationRequest
                    },
                    bankProfile.CustomBehaviour?.DomesticVrpConsent);

            // Create DomesticVrp
            var instructionIdentification = Guid.NewGuid().ToString("N");
            string endToEndIdentification = Guid.NewGuid().ToString("N")[..31];
            DomesticVrpRequest domesticVrpRequest = await GetDomesticVrpRequest(
                domesticVrpConsentId,
                modifiedBy,
                vrpFluentRequestLogging,
                instructionIdentification,
                endToEndIdentification,
                amount,
                paymentsEnv,
                bankProfile.VariableRecurringPaymentsApiSettings);
            DomesticVrpResponse domesticVrpResp =
                await variableRecurringPaymentsApiClient.DomesticVrpCreate(
                    domesticVrpRequest,
                    new ConsentExternalCreateParams
                    {
                        ExtraHeaders = null,
                        PublicRequestUrlWithoutQuery = null
                    },
                    bankProfile.CustomBehaviour?.DomesticVrp);
            string domesticVrpExternalId = domesticVrpResp.ExternalApiResponse.Data.DomesticVRPId;

            // Allow time for payment to be processed
            await Task.Delay(TimeSpan.FromSeconds(3));

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
                    },
                    bankProfile.CustomBehaviour?.DomesticVrp);

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

            // If refresh token known available, delete access token to check refresh token grant
            bool refreshTokenMayBeAbsent = bankProfile.CustomBehaviour
                ?.DomesticVrpConsentAuthCodeGrantPost
                ?.ExpectedResponseRefreshTokenMayBeAbsent ?? false;
            if (!refreshTokenMayBeAbsent)
            {
                {
                    // Get new application services scope
                    using IServiceScopeContainer serviceScopeContainer =
                        new ServiceScopeFromDependencyInjection(appServiceProvider);

                    // Get consent
                    IDbService dbService = serviceScopeContainer.DbService;
                    IDbEntityMethods<Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent>
                        consentEntityMethods =
                            dbService
                                .GetDbEntityMethodsClass<
                                    Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent>();
                    Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent consent =
                        consentEntityMethods
                            .DbSet
                            .Include(o => o.DomesticVrpConsentAccessTokensNavigation)
                            .Include(o => o.DomesticVrpConsentRefreshTokensNavigation)
                            .AsSplitQuery()
                            .SingleOrDefault(x => x.Id == domesticVrpConsentId) ??
                        throw new KeyNotFoundException();

                    // Ensure refresh token available
                    DomesticVrpConsentRefreshToken unused =
                        consent
                            .DomesticVrpConsentRefreshTokensNavigation
                            .SingleOrDefault(x => !x.IsDeleted) ??
                        throw new Exception("Refresh token not found.");

                    // If available, delete cached access token (to force use of refresh token)
                    memoryCache.Remove(consent.GetCacheKey());

                    // If available, delete stored access token (to force use of refresh token) 
                    DomesticVrpConsentAccessToken? storedAccessToken =
                        consent
                            .DomesticVrpConsentAccessTokensNavigation.SingleOrDefault(x => !x.IsDeleted);
                    if (storedAccessToken is not null)
                    {
                        storedAccessToken.UpdateIsDeleted(true, DateTimeOffset.UtcNow, modifiedBy);
                        await dbService.GetDbSaveChangesMethodClass().SaveChangesAsync();
                    }
                }

                // Create DomesticVrp FundsConfirmation
                double amountDouble2 = Random.Shared.Next(10, 300) / 100.0;
                var amount2 = amountDouble2.ToString("F2");
                DomesticVrpConsentFundsConfirmationRequest domesticVrpConsentFundsConfirmationRequest2 =
                    await GetDomesticVrpConsentFundsConfirmationRequest(
                        modifiedBy,
                        vrpFluentRequestLogging,
                        amount2,
                        bankProfile.VariableRecurringPaymentsApiSettings);
                await variableRecurringPaymentsApiClient.DomesticVrpConsentCreateFundsConfirmation(
                    new VrpConsentFundsConfirmationCreateParams
                    {
                        PublicRequestUrlWithoutQuery = null,
                        ExtraHeaders = null,
                        ConsentId = domesticVrpConsentId,
                        Request = domesticVrpConsentFundsConfirmationRequest2
                    },
                    bankProfile.CustomBehaviour?.DomesticVrpConsent);
            }
        }

        // Delete DomesticVrpConsent
        var excludeExternalApiOperation = false;
        BaseResponse _ = await variableRecurringPaymentsApiClient.DomesticVrpConsentDelete(
            new ConsentDeleteParams
            {
                Id = domesticVrpConsentId,
                ModifiedBy = null,
                ExtraHeaders = null,
                ExcludeExternalApiOperation = excludeExternalApiOperation
            });
    }

    private static async Task<DomesticVrpConsentFundsConfirmationRequest> GetDomesticVrpConsentFundsConfirmationRequest(
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        string amount,
        VariableRecurringPaymentsApiSettings variableRecurringPaymentsApiSettings)
    {
        var externalApiRequest =
            new VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationRequest
            {
                Data = new VariableRecurringPaymentsModelsPublic.Data6
                {
                    ConsentId = "",
                    Reference = "Integration test",
                    InstructedAmount = new VariableRecurringPaymentsModelsPublic.OBActiveOrHistoricCurrencyAndAmount
                    {
                        Amount = "placeholder", // logging placeholder
                        Currency = "GBP"
                    }
                }
            };
        var domesticVrpConsentFundsConfirmationRequest = new DomesticVrpConsentFundsConfirmationRequest
        {
            ExternalApiRequest =
                variableRecurringPaymentsApiSettings
                    .DomesticVrpConsentExternalApiFundsConfirmationRequestAdjustments(
                        externalApiRequest), // customise external API request using bank profile
            ModifiedBy = "placeholder" // logging placeholder
        };

        await vrpFluentRequestLogging
            .AppendToPath("domesticVrpConsent")
            .AppendToPath("fundsConfirmation")
            .AppendToPath("postRequest")
            .WriteFile(domesticVrpConsentFundsConfirmationRequest);

        domesticVrpConsentFundsConfirmationRequest.ModifiedBy = modifiedBy; // replace logging placeholder
        domesticVrpConsentFundsConfirmationRequest.ExternalApiRequest.Data.InstructedAmount.Amount =
            amount; // replace logging placeholder

        return domesticVrpConsentFundsConfirmationRequest;
    }

    private static async Task<DomesticVrpRequest> GetDomesticVrpRequest(
        Guid domesticVrpConsentId,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        string instructionIdentification,
        string endToEndIdentification,
        string amount,
        PaymentsEnv paymentsEnv,
        VariableRecurringPaymentsApiSettings variableRecurringPaymentsApiSettings)
    {
        var externalApiRequest = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest
        {
            Data = new VariableRecurringPaymentsModelsPublic.Data3
            {
                ConsentId = "",
                PSUAuthenticationMethod = "UK.OBIE.SCANotRequired",
                PSUInteractionType = VariableRecurringPaymentsModelsPublic.OBVRPInteractionTypes.OffSession,
                VRPType = "UK.OBIE.VRPType.Sweeping",
                Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                {
                    CreditorAccount = new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                    {
                        SchemeName = "placeholder", // logging placeholder
                        Identification = "placeholder", // logging placeholder
                        Name = "placeholder" // logging placeholder
                    },
                    RemittanceInformation =
                        new VariableRecurringPaymentsModelsPublic.RemittanceInformation
                        {
                            Reference = "Integration test"
                        }
                },
                Instruction = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInstruction
                {
                    InstructionIdentification = "placeholder", // logging placeholder
                    EndToEndIdentification = "placeholder", // logging placeholder
                    RemittanceInformation =
                        new VariableRecurringPaymentsModelsPublic.OBVRPRemittanceInformation
                        {
                            Reference = "Integration test"
                        },
                    InstructedAmount = new VariableRecurringPaymentsModelsPublic.OBActiveOrHistoricCurrencyAndAmount
                    {
                        Amount = "placeholder", // logging placeholder
                        Currency = "GBP"
                    },
                    CreditorAccount = new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                    {
                        SchemeName = "placeholder", // logging placeholder
                        Identification = "placeholder", // logging placeholder
                        Name = "placeholder" // logging placeholder
                    }
                }
            },
            Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
            {
                PaymentContextCode =
                    VariableRecurringPaymentsModelsPublic.OBRisk1PaymentContextCode.TransferToSelf,
                ContractPresentIndicator = true
            }
        };
        var domesticVrpRequest = new DomesticVrpRequest
        {
            DomesticVrpConsentId = default, // logging placeholder
            ExternalApiRequest =
                variableRecurringPaymentsApiSettings
                    .DomesticVrpExternalApiRequestAdjustments(
                        externalApiRequest), // customise external API request using bank profile
            ModifiedBy = "placeholder" // logging placeholder
        };
        await vrpFluentRequestLogging
            .AppendToPath("domesticVrp")
            .AppendToPath("postRequest")
            .WriteFile(domesticVrpRequest);

        domesticVrpRequest.DomesticVrpConsentId = domesticVrpConsentId; // replace logging placeholder
        domesticVrpRequest.ModifiedBy = modifiedBy; // replace logging placeholder

        domesticVrpRequest.ExternalApiRequest.Data.Initiation.CreditorAccount!.SchemeName =
            paymentsEnv.AccountSchemeName; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Identification =
            paymentsEnv.AccountId; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Name =
            paymentsEnv.AccountName; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Instruction.InstructionIdentification =
            instructionIdentification; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Instruction.EndToEndIdentification =
            endToEndIdentification; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Instruction.InstructedAmount.Amount =
            amount; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Instruction.CreditorAccount.SchemeName =
            paymentsEnv.AccountSchemeName; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Instruction.CreditorAccount.Identification =
            paymentsEnv.AccountId; // replace logging placeholder
        domesticVrpRequest.ExternalApiRequest.Data.Instruction.CreditorAccount.Name =
            paymentsEnv.AccountName; // replace logging placeholder


        return domesticVrpRequest;
    }

    private static async Task<DomesticVrpConsentRequest> GetDomesticVrpConsentRequest(
        Guid bankRegistrationId,
        string testNameUnique,
        string modifiedBy,
        FilePathBuilder vrpFluentRequestLogging,
        PaymentsEnv paymentsEnv,
        VariableRecurringPaymentsApiSettings variableRecurringPaymentsApiSettings)
    {
        var externalApiRequest =
            new VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest
            {
                Data = new VariableRecurringPaymentsModelsPublic.Data2
                {
                    ReadRefundAccount = VariableRecurringPaymentsModelsPublic.Data2ReadRefundAccount.Yes,
                    ControlParameters =
                        new VariableRecurringPaymentsModelsPublic.OBDomesticVRPControlParameters
                        {
                            MaximumIndividualAmount =
                                new VariableRecurringPaymentsModelsPublic.OBActiveOrHistoricCurrencyAndAmount
                                {
                                    Amount = "5.00",
                                    Currency = "GBP"
                                },
                            PeriodicLimits =
                            [
                                new VariableRecurringPaymentsModelsPublic.PeriodicLimits
                                {
                                    PeriodType = VariableRecurringPaymentsModelsPublic.PeriodicLimitsPeriodType.Month,
                                    PeriodAlignment =
                                        VariableRecurringPaymentsModelsPublic.PeriodicLimitsPeriodAlignment.Consent,
                                    Amount = "50.00",
                                    Currency = "GBP"
                                }
                            ],
                            VRPType = ["UK.OBIE.VRPType.Sweeping"],
                            PSUAuthenticationMethods = ["UK.OBIE.SCANotRequired"],
                            PSUInteractionTypes =
                            [
                                VariableRecurringPaymentsModelsPublic.OBVRPInteractionTypes.OffSession
                            ]
                        },
                    Initiation = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPInitiation
                    {
                        CreditorAccount = new VariableRecurringPaymentsModelsPublic.OBCashAccountCreditor3
                        {
                            SchemeName = "placeholder", // logging placeholder
                            Identification = "placeholder", // logging placeholder
                            Name = "placeholder" // logging placeholder
                        },
                        RemittanceInformation =
                            new VariableRecurringPaymentsModelsPublic.RemittanceInformation
                            {
                                Reference = "Integration test"
                            }
                    }
                },
                Risk = new VariableRecurringPaymentsModelsPublic.OBRisk1
                {
                    PaymentContextCode =
                        VariableRecurringPaymentsModelsPublic.OBRisk1PaymentContextCode.TransferToSelf,
                    ContractPresentIndicator = true
                }
            };

        var domesticVrpConsentRequest = new DomesticVrpConsentRequest
        {
            BankRegistrationId = default, // logging placeholder
            ExternalApiRequest =
                variableRecurringPaymentsApiSettings
                    .DomesticVrpConsentExternalApiRequestAdjustments(
                        externalApiRequest), // customise external API request using bank profile
            Reference = "placeholder", // logging placeholder
            CreatedBy = "placeholder" // logging placeholder
        };

        await vrpFluentRequestLogging
            .AppendToPath("domesticVrpConsent")
            .AppendToPath("postRequest")
            .WriteFile(domesticVrpConsentRequest);

        domesticVrpConsentRequest.BankRegistrationId = bankRegistrationId; // replace logging placeholder
        domesticVrpConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount!.SchemeName =
            paymentsEnv.AccountSchemeName; // replace logging placeholder
        domesticVrpConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Identification =
            paymentsEnv.AccountId; // replace logging placeholder
        domesticVrpConsentRequest.ExternalApiRequest.Data.Initiation.CreditorAccount.Name =
            paymentsEnv.AccountName; // replace logging placeholder
        domesticVrpConsentRequest.Reference = testNameUnique; // replace logging placeholder
        domesticVrpConsentRequest.CreatedBy = modifiedBy; // replace logging placeholder

        return domesticVrpConsentRequest;
    }
}
