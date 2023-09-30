// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class RevolutGenerator : BankProfileGeneratorBase<RevolutBank>
{
    public RevolutGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider,
        IBankGroup<RevolutBank> bankGroup) : base(bankProfilesSettingsProvider, bankGroup) { }

    public override BankProfile GetBankProfile(RevolutBank bank) =>
        new(
            _bankGroup.GetBankProfile(bank),
            "https://oba.revolut.com", // from https://developer.revolut.com/docs/guides/build-banking-apps/register-your-application-using-dcr/open-id-configuration-urls
            "001580000103UAvAAM", // from https://developer.revolut.com/docs/guides/build-banking-apps/tutorials/get-account-and-transaction-information
            GetAccountAndTransactionApi(bank),
            null,
            null,
            true)
        {
            CustomBehaviour = new CustomBehaviourClass
            {
                AccountAccessConsentPost = new AccountAccessConsentPostCustomBehaviour { ResponseLinksOmitId = true },
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url =
                        "https://oba.revolut.com/openid-configuration" // from https://developer.revolut.com/docs/guides/build-banking-apps/register-your-application-using-dcr/open-id-configuration-urls
                }
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum>
                        {
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadPAN,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadOffers,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadParty,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum
                                .ReadStatementsDetail,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadProducts,
                            AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadPartyPSU
                        };
                    foreach (AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                }
            },
            DefaultResponseMode = OAuth2ResponseMode.Query
        };

    private AccountAndTransactionApi GetAccountAndTransactionApi(RevolutBank bank) =>
        new()
        {
            BaseUrl =
                "https://oba.revolut.com" // from https://developer.revolut.com/docs/open-banking/create-account-access-consents
        };
}
