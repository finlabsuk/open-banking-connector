// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.AccountAndTransaction;

public static class TemplateRequests
{
    public static AccountAndTransactionModelsPublic.OBReadConsent1 AccountAccessConsentExternalApiRequest(
        AccountAccessConsentTemplateType accountAccessConsentTemplateType) =>
        accountAccessConsentTemplateType switch
        {
            AccountAccessConsentTemplateType.MaximumPermissions => new AccountAndTransactionModelsPublic.OBReadConsent1(
                new AccountAndTransactionModelsPublic.OBReadConsent1Data(
                    new List<AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum>
                    {
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadAccountsBasic,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadAccountsDetail,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadBalances,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadBeneficiariesBasic,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadBeneficiariesDetail,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadDirectDebits,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadOffers,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadPAN,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadParty,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadPartyPSU,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadProducts,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsBasic,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsDetail,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStandingOrdersBasic,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStandingOrdersDetail,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStatementsBasic,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadStatementsDetail,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadTransactionsBasic,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadTransactionsCredits,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadTransactionsDebits,
                        AccountAndTransactionModelsPublic.OBReadConsent1DataPermissionsEnum.ReadTransactionsDetail
                    })
                {
                    ExpirationDateTime = null,
                    TransactionFromDateTime = null,
                    TransactionToDateTime = null
                },
                new Dictionary<string, string>()),
            _ => throw new ArgumentOutOfRangeException(
                nameof(accountAccessConsentTemplateType),
                accountAccessConsentTemplateType,
                null)
        };
}
