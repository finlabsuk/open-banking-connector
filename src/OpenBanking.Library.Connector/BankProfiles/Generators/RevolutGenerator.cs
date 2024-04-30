// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class RevolutGenerator : BankProfileGeneratorBase<RevolutBank>
{
    public RevolutGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<RevolutBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(
        RevolutBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroup.GetBankProfile(bank),
            "https://oba.revolut.com", // from https://developer.revolut.com/docs/guides/build-banking-apps/register-your-application-using-dcr/open-id-configuration-urls
            "001580000103UAvAAM", // from https://developer.revolut.com/docs/guides/build-banking-apps/tutorials/get-account-and-transaction-information
            GetAccountAndTransactionApi(bank),
            null,
            null,
            true,
            instrumentationClient)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                AccountAccessConsentPost = new ConsentPostCustomBehaviour { ResponseLinksOmitId = true },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url =
                        "https://oba.revolut.com/openid-configuration" // from https://developer.revolut.com/docs/guides/build-banking-apps/register-your-application-using-dcr/open-id-configuration-urls
                },
                AccountAccessConsentAuthGet =
                    new ConsentAuthGetCustomBehaviour
                    {
                        AddRedundantOAuth2NonceRequestParameter = true,
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour { IdTokenMayNotHaveAuthTimeClaim = true }
                    },
                AccountAccessConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour
                    {
                        ExpectedResponseRefreshTokenMayBeAbsent = true,
                        IdTokenProcessingCustomBehaviour =
                            new IdTokenProcessingCustomBehaviour
                            {
                                IdTokenMayNotHaveAuthTimeClaim = true,
                                IdTokenMayNotHaveConsentIdClaim = true,
                                IdTokenMayNotHaveAcrClaim = true,
                                IdTokenExpirationTimeClaimJsonConverter =
                                    DateTimeOffsetUnixConverterEnum.UnixMilliSecondsJsonFormat
                            }
                    }
            },
            BankConfigurationApiSettings = new BankConfigurationApiSettings { UseRegistrationDeleteEndpoint = true },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.Permissions>
                        {
                            AccountAndTransactionModelsPublic.Permissions.ReadPAN,
                            AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                            AccountAndTransactionModelsPublic.Permissions.ReadParty,
                            AccountAndTransactionModelsPublic.Permissions.ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStatementsDetail,
                            AccountAndTransactionModelsPublic.Permissions.ReadProducts,
                            AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU
                        };
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                }
            },
            DefaultResponseMode = OAuth2ResponseMode.Query,
            AspspBrandId = 1470
        };

    private AccountAndTransactionApi GetAccountAndTransactionApi(RevolutBank bank) =>
        new()
        {
            BaseUrl =
                "https://oba.revolut.com" // from https://developer.revolut.com/docs/open-banking/create-account-access-consents
        };
}
