// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class CooperativeGenerator : BankProfileGeneratorBase<CooperativeBank>
{
    public CooperativeGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Cooperative) { }

    public override BankProfile GetBankProfile(
        CooperativeBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            GetIssuer(bank),
            GetFinancialId(bank),
            GetAccountAndTransactionApi(bank),
            null,
            bank is CooperativeBank.CooperativeSandbox ? null : GetPaymentInitiationApi(bank),
            null,
            null,
            null,
            bank is not CooperativeBank.CooperativeSandbox,
            instrumentationClient)
        {
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationEndpoint = false,
                TokenEndpointAuthMethod = TokenEndpointAuthMethodSupportedValues.ClientSecretPost
            },
            CustomBehaviour = new CustomBehaviourClass
            {
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    OpenIdConfiguration = new OpenIdConfiguration
                    {
                        Issuer = GetIssuer(bank),
                        ResponseTypesSupported = ["code"],
                        ScopesSupported = ["accounts", "offline", "payments"],
                        ResponseModesSupported = [OAuth2ResponseMode.Query],
                        TokenEndpoint = GetTokenEndpoint(bank),
                        AuthorizationEndpoint = GetAuthorisationEndpoint(bank),
                        RegistrationEndpoint = null,
                        JwksUri = "", // not used
                        TokenEndpointAuthMethodsSupported =
                        [
                            TokenEndpointAuthMethodOpenIdConfiguration.ClientSecretPost
                        ]
                    }
                },
                ClientCredentialsGrantPost =
                    new ClientCredentialsGrantPostCustomBehaviour { ResponseTokenTypeCaseMayBeIncorrect = true },
                AccountAccessConsentAuthGet =
                    new AccountAccessConsentAuthGetCustomBehaviour
                    {
                        UsePkce = true,
                        Scope = "accounts+offline",
                        GetExtraParameters = reAuthNotInitialAuth =>
                        {
                            var concurrentDictionary = new ConcurrentDictionary<string, string>
                            {
                                ["login_hint"] = GetLoginHint(bank)
                            };
                            if (reAuthNotInitialAuth)
                            {
                                concurrentDictionary["stepUp"] = "require";
                            }
                            return concurrentDictionary;
                        },
                        DetectReAuthCaseViaConsentStatus = true,
                        ExtraConsentParameterName = "display",
                        DoNotUseUrlPathEncoding = true,
                        SingleBase64EncodedParameterName = "data"
                    },
                DomesticPaymentConsentAuthGet =
                    new DomesticPaymentConsentAuthGetCustomBehaviour
                    {
                        UsePkce = true,
                        // from: https://www.developer.co-operativebank.co.uk/apis/general-specifications/
                        GetExtraParameters = request =>
                        {
                            DomesticPaymentConsentAuthContextCooperativeOnly cooperativeOnly =
                                request.CooperativeOnly ??
                                throw new ArgumentException(
                                    $"{nameof(request.CooperativeOnly)} must be specified for bank group {BankGroup.Cooperative}.",
                                    nameof(request));
                            if (cooperativeOnly.CreditorAccountName.Contains('&'))
                            {
                                throw new ArgumentException(
                                    $"{nameof(cooperativeOnly.CreditorAccountName)} must not contain '&' for bank group {BankGroup.Cooperative} " +
                                    "as auth URL parameters are not URL-encoded.",
                                    nameof(request));
                            }
                            return new ConcurrentDictionary<string, string>
                            {
                                ["login_hint"] = GetLoginHint(bank),
                                ["jrn_typ"] = "DP",
                                ["ben_name"] = cooperativeOnly.CreditorAccountName,
                                ["amount"] = cooperativeOnly.InstructedAmountAmount
                            };
                        },
                        ExtraConsentParameterName = "display",
                        DoNotUseUrlPathEncoding = true,
                        SingleBase64EncodedParameterName = "data"
                    },
                AccountAccessConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour
                    {
                        ResponseTokenTypeCaseMayBeIncorrect = true,
                        Scope = "accounts offline"
                    },
                DomesticPaymentConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour
                    {
                        ResponseTokenTypeCaseMayBeIncorrect = true,
                        ExpectedResponseRefreshTokenMayBeAbsent = true
                    },
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour
                    {
                        ResponseTokenTypeCaseMayBeIncorrect = true,
                        Scope = "accounts offline"
                    },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour { PreferMisspeltContractPresentIndicator = true },
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    PreferMisspeltContractPresentIndicator = true,
                    ResponseDataDebtorMayBeMissingOrWrong = true
                }
            },
            DefaultResponseType = OAuth2ResponseType.Code,
            UseOpenIdConnect = false,
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.Permissions>
                        {
                            AccountAndTransactionModelsPublic.Permissions.ReadBeneficiariesBasic,
                            AccountAndTransactionModelsPublic.Permissions.ReadBeneficiariesDetail,
                            AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                            AccountAndTransactionModelsPublic.Permissions.ReadPAN,
                            AccountAndTransactionModelsPublic.Permissions.ReadParty,
                            AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU,
                            AccountAndTransactionModelsPublic.Permissions.ReadProducts,
                            AccountAndTransactionModelsPublic.Permissions.ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStatementsDetail
                        };
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                },
                UseReauth = bank is not CooperativeBank.CooperativeSandbox
            }
        };

    private static string GetLoginHint(CooperativeBank bank) =>
        bank switch
        {
            CooperativeBank.Cooperative => "coop",
            CooperativeBank.CooperativeSandbox => "coop",
            CooperativeBank.Smile => "smile",
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        }; // from: https://www.developer.co-operativebank.co.uk/apis/general-specifications/

    private static string GetIssuer(CooperativeBank bank) =>
        bank switch
        {
            CooperativeBank.CooperativeSandbox => "https://sandbox-openbanking-retail.apis.co-operativebanktest.co.uk",
            CooperativeBank.Cooperative => "https://openbanking-retail.apis.co-operativebank.co.uk",
            CooperativeBank.Smile => "https://openbanking-retail.apis.co-operativebank.co.uk",
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        }; // Not used, so set to first part of API base URL

    private static string GetTokenEndpoint(CooperativeBank bank) =>
        bank switch
        {
            CooperativeBank.CooperativeSandbox =>
                "https://sandbox-openbanking-retail.apis.co-operativebanktest.co.uk/apis/oauth/v1/oauth2/token", // from https://www.developer.co-operativebank.co.uk/apis/general-specifications/
            CooperativeBank.Cooperative =>
                "https://openbanking-retail.apis.co-operativebank.co.uk/apis/oauth/v1/oauth2/token", // from https://www.developer.co-operativebank.co.uk/apis/general-specifications/
            CooperativeBank.Smile =>
                "https://openbanking-retail.apis.co-operativebank.co.uk/apis/oauth/v1/oauth2/token", // from https://www.developer.co-operativebank.co.uk/apis/general-specifications/
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        };

    private static string GetAuthorisationEndpoint(CooperativeBank bank) =>
        bank switch
        {
            CooperativeBank.CooperativeSandbox =>
                "https://sandbox-bank.co-operativebanktest.co.uk/openbanking/coop/oauthRedirect", // from https://www.developer.co-operativebank.co.uk/apis/general-specifications/
            CooperativeBank.Cooperative =>
                "https://bank.co-operativebank.co.uk/openbanking/coop/oauthRedirect", // from https://www.developer.co-operativebank.co.uk/apis/general-specifications/
            CooperativeBank.Smile =>
                "https://bank.co-operativebank.co.uk/openbanking/smile/oauthRedirect", // from https://www.developer.co-operativebank.co.uk/apis/general-specifications/
            _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
        };

    private static AccountAndTransactionApi GetAccountAndTransactionApi(CooperativeBank bank) =>
        new()
        {
            BaseUrl = bank switch
            {
                CooperativeBank.CooperativeSandbox =>
                    "https://sandbox-openbanking-retail.apis.co-operativebanktest.co.uk", // from https://www.developer.co-operativebank.co.uk/apis/aisp/endpoints-and-errors/index
                CooperativeBank.Cooperative =>
                    "https://openbanking-retail.apis.co-operativebank.co.uk", // from https://www.developer.co-operativebank.co.uk/apis/aisp/endpoints-and-errors/index
                CooperativeBank.Smile =>
                    "https://openbanking-retail.apis.co-operativebank.co.uk", // from https://www.developer.co-operativebank.co.uk/apis/aisp/endpoints-and-errors/index
                _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
            } + "/apis/retail/open-banking/v3.1/aisp"
        };

    private static PaymentInitiationApi GetPaymentInitiationApi(CooperativeBank bank) =>
        new()
        {
            BaseUrl = bank switch
                      {
                          CooperativeBank.Cooperative =>
                              "https://openbanking-retail.apis.co-operativebank.co.uk", // from: https://www.developer.co-operativebank.co.uk/apis/pisp/endpoints-and-errors/index
                          CooperativeBank.Smile =>
                              "https://openbanking-retail.apis.co-operativebank.co.uk", // from: https://www.developer.co-operativebank.co.uk/apis/pisp/endpoints-and-errors/index
                          _ => throw new ArgumentOutOfRangeException(nameof(bank), bank, null)
                      } +
                      "/apis/retail/open-banking/v3.1/pisp" // from: https://www.developer.co-operativebank.co.uk/apis/pisp/endpoints-and-errors/index
        };
}
