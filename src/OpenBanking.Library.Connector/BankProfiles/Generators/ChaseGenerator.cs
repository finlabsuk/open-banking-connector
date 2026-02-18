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

public class ChaseGenerator : BankProfileGeneratorBase<ChaseBank>
{
    public ChaseGenerator(
        ISettingsProvider<BankProfilesSettings> bankProfilesSettingsProvider) : base(
        bankProfilesSettingsProvider,
        BankGroup.Chase) { }

    public override BankProfile GetBankProfile(
        ChaseBank bank,
        IInstrumentationClient instrumentationClient) =>
        new(
            _bankGroupData.GetBankProfile(bank),
            "https://auth.openbanking-obie.chase.co.uk/auth/realms/provider", // from https://developer.openbanking-obie.chase.co.uk/docs/additional-information
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
            BankConfigurationApiSettings = new BankConfigurationApiSettings
            {
                UseRegistrationEndpoint = false,
                TokenEndpointAuthMethod = TokenEndpointAuthMethodSupportedValues.PrivateKeyJwt,
                IdTokenSubClaimType = IdTokenSubClaimType.EndUserId
            },
            AccountAndTransactionApiSettings = new AccountAndTransactionApiSettings
            {
                AccountAccessConsentTemplateExternalApiRequestAdjustments = externalApiRequest =>
                {
                    var elementsToRemove =
                        new List<AccountAndTransactionModelsPublic.Permissions>
                        {
                            AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                            AccountAndTransactionModelsPublic.Permissions.ReadPAN,
                            AccountAndTransactionModelsPublic.Permissions.ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.Permissions.ReadStatementsDetail
                        };
                    foreach (AccountAndTransactionModelsPublic.Permissions element in
                             elementsToRemove)
                    {
                        externalApiRequest.Data.Permissions.Remove(element);
                    }

                    return externalApiRequest;
                },
                UseGetParty2Endpoint = false,
                UseReauth = false,
                UseBalancesNotAccountEndpointInSecondSession = true
            },
            PaymentInitiationApiSettings = new PaymentInitiationApiSettings
            {
                DomesticPaymentConsentExternalApiRequestAdjustments = request =>
                {
                    request.Data.ReadRefundAccount = null;
                    return request;
                }
            },
            CustomBehaviour = new CustomBehaviourClass
            {
                AccountAccessConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour { ResponseScopeMayIncludeExtraValues = true },
                AccountAccessConsentRefreshTokenGrantPost =
                    new RefreshTokenGrantPostCustomBehaviour { ResponseScopeMayIncludeExtraValues = true },
                DomesticPaymentConsentAuthCodeGrantPost =
                    new AuthCodeGrantPostCustomBehaviour { ResponseScopeMayIncludeExtraValues = true },
                DomesticPaymentConsent =
                    new DomesticPaymentConsentCustomBehaviour
                    {
                        ResponseRiskContractPresentIndicatorMayBeMissingOrWrong = true
                    },
                DomesticPayment = new DomesticPaymentCustomBehaviour
                {
                    ResponseDataDebtorMayBeMissingOrWrong = true,
                    ResponseDataRefundMayBeMissingOrWrong = true
                },
                ClientCredentialsGrantPost = new ClientCredentialsGrantPostCustomBehaviour()
            },
            AspspBrandId = 0
        };

    private static string GetApiBaseUrl(string suffix) =>
        $"https://api.openbanking-obie.chase.co.uk/obie2-{suffix}/open-banking/v3.1/{suffix}"; // from https://developer.openbanking-obie.chase.co.uk/docs/additional-information
}
