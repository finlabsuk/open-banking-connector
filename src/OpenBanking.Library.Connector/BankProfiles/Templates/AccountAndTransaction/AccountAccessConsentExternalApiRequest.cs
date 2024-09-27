// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.AccountAndTransaction;

public static class AccountAccessTemplates
{
    public static AccountAndTransactionModelsPublic.OBReadConsent1 AccountAccessConsentExternalApiRequest(
        AccountAccessConsentTemplateType accountAccessConsentTemplateType,
        AccountAndTransactionApiSettings accountAndTransactionApiSettings)
    {
        AccountAndTransactionModelsPublic.OBReadConsent1 externalApiRequest = accountAccessConsentTemplateType switch
        {
            AccountAccessConsentTemplateType.MaximumPermissions => new AccountAndTransactionModelsPublic.OBReadConsent1
            {
                Data =
                    new AccountAndTransactionModelsPublic.Data4
                    {
                        Permissions = new List<AccountAndTransactionModelsPublic.Permissions>
                        {
                            AccountAndTransactionModelsPublic.Permissions.ReadAccountsBasic,
                            AccountAndTransactionModelsPublic.Permissions.ReadAccountsDetail,
                            AccountAndTransactionModelsPublic.Permissions.ReadBalances,
                            AccountAndTransactionModelsPublic.Permissions.ReadBeneficiariesBasic,
                            AccountAndTransactionModelsPublic.Permissions.ReadBeneficiariesDetail,
                            AccountAndTransactionModelsPublic.Permissions.ReadDirectDebits,
                            AccountAndTransactionModelsPublic.Permissions.ReadOffers,
                            AccountAndTransactionModelsPublic.Permissions.ReadPAN,
                            AccountAndTransactionModelsPublic.Permissions.ReadParty,
                            AccountAndTransactionModelsPublic.Permissions.ReadPartyPSU,
                            AccountAndTransactionModelsPublic.Permissions.ReadProducts,
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadScheduledPaymentsBasic,
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadScheduledPaymentsDetail,
                            AccountAndTransactionModelsPublic.Permissions.ReadStandingOrdersBasic,
                            AccountAndTransactionModelsPublic.Permissions
                                .ReadStandingOrdersDetail,
                            AccountAndTransactionModelsPublic.Permissions.ReadStatementsBasic,
                            AccountAndTransactionModelsPublic.Permissions.ReadStatementsDetail,
                            AccountAndTransactionModelsPublic.Permissions.ReadTransactionsBasic,
                            AccountAndTransactionModelsPublic.Permissions.ReadTransactionsCredits,
                            AccountAndTransactionModelsPublic.Permissions.ReadTransactionsDebits,
                            AccountAndTransactionModelsPublic.Permissions.ReadTransactionsDetail
                        }
                    },
                Risk = new AccountAndTransactionModelsPublic.OBRisk2()
            },
            _ => throw new ArgumentOutOfRangeException(
                nameof(accountAccessConsentTemplateType),
                accountAccessConsentTemplateType,
                null)
        };

        // Customise external API request using bank profile
        externalApiRequest = accountAndTransactionApiSettings
            .AccountAccessConsentTemplateExternalApiRequestAdjustments(externalApiRequest);

        return externalApiRequest;
    }
}
