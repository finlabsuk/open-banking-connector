// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.Management;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class StarlingGenerator : BankProfileGeneratorBase<StarlingBank>
{
    public StarlingGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Starling) { }

    public override BankProfile GetBankProfile(
        StarlingBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            "https://api-openbanking.starlingbank.com/", // from https://developer.starlingbank.com/docs/open-banking#the-openid-connect-discovery-url-1
            GetFinancialId(bank),
            new AccountAndTransactionApi { BaseUrl = GetApiBaseUrl("aisp") },
            null,
            new PaymentInitiationApi { BaseUrl = GetApiBaseUrl("pisp") },
            null,
            null,
            null,
            true,
            instrumentationClient)
        {
            DynamicClientRegistrationApiVersion = DynamicClientRegistrationApiVersion.Version3p3,
            BankConfigurationApiSettings = new BankConfigurationApiSettings { UseRegistrationGetEndpoint = true },
            CustomBehaviour = new CustomBehaviourClass
            {
                BankRegistrationPut = new BankRegistrationPutCustomBehaviour { GetCustomTokenScope = _ => "openid" },
                AccountAccessConsentAuthGet =
                    new ConsentAuthGetCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAcrClaim = true }
                    },
                AccountAccessConsentPost = new ReadWritePostCustomBehaviour { PostResponseLinksMayOmitId = true },
                AccountAccessConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour
                    {
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAcrClaim = true }
                    },
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                DomesticPaymentConsentAuthGet = new ConsentAuthGetCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAcrClaim = true }
                },
                DomesticPaymentConsentAuthCodeGrantPost = new AuthCodeGrantPostCustomBehaviour
                {
                    IdTokenProcessingCustomBehaviour =
                        new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAcrClaim = true },
                    ExpectedResponseRefreshTokenMayBeAbsent = true
                },
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    PostResponseLinksMayOmitId = true,
                    ResponseDataDebtorMayBeMissingOrWrong = true
                },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour { PostResponseLinksMayOmitId = true },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url =
                        "https://openbanking.starlingbank.com/.well-known/openid-configuration" // from https://developer.starlingbank.com/docs/open-banking#the-openid-connect-discovery-url-1
                }
            },
            PaymentInitiationApiSettings = new PaymentInitiationApiSettings
            {
                PreferPartyToPartyPaymentContextCode = true,
                UseContractPresentIndicator = false
            },
            DefaultResponseMode = OAuth2ResponseMode.Query,
            AspspBrandId = 1510
        };

    private static string GetApiBaseUrl(string suffix) =>
        $"https://api-openbanking.starlingbank.com/open-banking/v3.1/{suffix}"; // from https://developer.starlingbank.com/docs/open-banking#account-and-transaction-api-1,
    // https://developer.starlingbank.com/docs/open-banking#payment-initiation-api-1
}
