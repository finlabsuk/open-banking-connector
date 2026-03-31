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

public class WiseGenerator : BankProfileGeneratorBase<WiseBank>
{
    public WiseGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Wise) { }

    public override BankProfile GetBankProfile(
        WiseBank bank,
        IInstrumentationClient instrumentationClient)
    {
        var authCodeGrantPostCustomBehaviour =
            new AuthCodeGrantPostCustomBehaviour
            {
                ResponseTokenTypeCaseMayBeIncorrect = true,
                ResponseScopeMayIncludeExtraValues = true
            };
        return new BankProfile(
            _bankGroupData.GetBankProfile(bank),
            "https://openbanking.transferwise.com", // from https://docs.wise.com/guides/developer/open-banking
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
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    List<AccountAndTransactionModelsPublic.Permissions> elementsToRemove =
                    [
                        AccountAndTransactionModelsPublic.Permissions.ReadBeneficiariesBasic,
                        AccountAndTransactionModelsPublic.Permissions.ReadBeneficiariesDetail,
                        AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                        AccountAndTransactionModelsPublic.Permissions.ReadPAN,
                        AccountAndTransactionModelsPublic.Permissions.ReadParty,
                        AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU,
                        AccountAndTransactionModelsPublic.Permissions.ReadProducts,
                        AccountAndTransactionModelsPublic.Permissions.ReadScheduledPaymentsBasic,
                        AccountAndTransactionModelsPublic.Permissions.ReadScheduledPaymentsDetail,
                        AccountAndTransactionModelsPublic.Permissions.ReadStandingOrdersBasic,
                        AccountAndTransactionModelsPublic.Permissions.ReadStandingOrdersDetail,
                        AccountAndTransactionModelsPublic.Permissions.ReadStatementsBasic,
                        AccountAndTransactionModelsPublic.Permissions.ReadStatementsDetail
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
                PreferPartyToPartyPaymentContextCode = true,
                UseContractPresentIndicator = false,
                UseReadRefundAccount = false
            },
            CustomBehaviour = new CustomBehaviourClass
            {
                OpenIdConfigurationGet = new OpenIdConfigurationGetCustomBehaviour
                {
                    Url =
                        "https://wise.com/openbanking/.well-known/openid-configuration" // from https://docs.wise.com/guides/developer/open-banking
                },
                ClientCredentialsGrantPost =
                    new ClientCredentialsGrantPostCustomBehaviour
                    {
                        ResponseTokenTypeCaseMayBeIncorrect = true,
                        ResponseScopeMayIncludeExtraValues = true
                    },
                AccountAccessConsentAuthCodeGrantPost =
                    authCodeGrantPostCustomBehaviour,
                AccountAccessConsentRefreshTokenGrantPost = new RefreshTokenGrantPostCustomBehaviour
                {
                    ResponseTokenTypeCaseMayBeIncorrect = true,
                    ResponseScopeMayIncludeExtraValues = true,
                    IdTokenMayBeAbsent = true
                },
                DomesticPaymentConsentAuthCodeGrantPost =
                    authCodeGrantPostCustomBehaviour,
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    ResponseDataRefundMayBeMissingOrWrong = true,
                    ResponseDataDebtorMayBeMissingOrWrong = true
                }
            },
            DefaultResponseMode = OAuth2ResponseMode.Query,
            AspspBrandId = 0
        };
    }

    private static string GetApiBaseUrl(string suffix) =>
        $"https://openbanking.transferwise.com/open-banking/v3.1.11/{suffix}"; // from https://docs.wise.com/guides/developer/open-banking
}
