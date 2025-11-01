// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Generators;

public class TsbGenerator : BankProfileGeneratorBase<TsbBank>
{
    public TsbGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Tsb) { }

    public override BankProfile GetBankProfile(
        TsbBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            "https://apis.tsb.co.uk/apis/open-banking/v3.1/", // from https://apis.developer.tsb.co.uk/ and https://apis.developer.tsb.co.uk/#/security
            GetFinancialId(bank),
            new AccountAndTransactionApi { BaseUrl = GetApiBaseUrl(true) },
            null,
            new PaymentInitiationApi { BaseUrl = GetApiBaseUrl(false) },
            null,
            null,
            null,
            true,
            instrumentationClient)
        {
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                TokenEndpointAuthMethod = TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt,
                UseRegistrationGetEndpoint = true,
                UseRegistrationDeleteEndpoint = true,
                IdTokenSubClaimType = IdTokenSubClaimType.EndUserId
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    List<AccountAndTransactionModelsPublic.Permissions> elementsToRemove =
                    [
                        AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                        AccountAndTransactionModelsPublic.Permissions.ReadParty,
                        AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU
                    ];
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }
                    return externalApiRequest;
                }
            },
            PaymentInitiationApiSettings = new PaymentInitiationApiSettings
            {
                UseReadRefundAccount = false,
                PreferPartyToPartyPaymentContextCode = true,
                UseContractPresentIndicator = false
            },
            CustomBehaviour = new CustomBehaviourClass
            {
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { IdTokenMayBeAbsent = true },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour { UseB64JoseHeader = true },
                DomesticPayment =
                    new DomesticPaymentCustomBehaviour
                    {
                        UseB64JoseHeader = true,
                        ResponseDataRefundMayBeMissingOrWrong = true,
                        ResponseDataDebtorMayBeMissingOrWrong = true
                    }
            },
            AspspBrandId = 0
        };

    private static string GetApiBaseUrl(bool isAisp) =>
        $"https://apis.tsb.co.uk/apis/open-banking/v3.1{(isAisp ? "/aisp" : "/pisp")}"; // from https://apis.developer.tsb.co.uk/#/accounts, https://apis.developer.tsb.co.uk/#/payments
}
