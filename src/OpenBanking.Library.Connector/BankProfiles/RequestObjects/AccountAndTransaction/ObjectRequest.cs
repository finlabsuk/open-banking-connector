// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.AccountAndTransaction
{
    public enum AccountAccessConsentType
    {
        MaximumPermissions
    }

    public static class BankProfileExtensions
    {
        public static OBReadConsent1 AccountAccessConsentExternalApiRequest(
            this BankProfile bankProfile,
            AccountAccessConsentType accountAccessConsentType) =>
            new(
                new OBReadConsent1Data(
                    new List<OBReadConsent1DataPermissionsEnum>
                    {
                        OBReadConsent1DataPermissionsEnum.ReadAccountsBasic,
                        OBReadConsent1DataPermissionsEnum.ReadAccountsDetail,
                        OBReadConsent1DataPermissionsEnum.ReadBalances,
                        OBReadConsent1DataPermissionsEnum.ReadBeneficiariesBasic,
                        OBReadConsent1DataPermissionsEnum.ReadBeneficiariesDetail,
                        OBReadConsent1DataPermissionsEnum.ReadDirectDebits,
                        OBReadConsent1DataPermissionsEnum.ReadOffers,
                        OBReadConsent1DataPermissionsEnum.ReadPAN,
                        OBReadConsent1DataPermissionsEnum.ReadParty,
                        OBReadConsent1DataPermissionsEnum.ReadPartyPSU,
                        OBReadConsent1DataPermissionsEnum.ReadProducts,
                        OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsBasic,
                        OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsDetail,
                        OBReadConsent1DataPermissionsEnum.ReadStandingOrdersBasic,
                        OBReadConsent1DataPermissionsEnum.ReadStandingOrdersDetail,
                        OBReadConsent1DataPermissionsEnum.ReadStatementsBasic,
                        OBReadConsent1DataPermissionsEnum.ReadStatementsDetail,
                        OBReadConsent1DataPermissionsEnum.ReadTransactionsBasic,
                        OBReadConsent1DataPermissionsEnum.ReadTransactionsCredits,
                        OBReadConsent1DataPermissionsEnum.ReadTransactionsDebits,
                        OBReadConsent1DataPermissionsEnum.ReadTransactionsDetail
                    })
                {
                    ExpirationDateTime = null,
                    TransactionFromDateTime = null,
                    TransactionToDateTime = null
                },
                new Dictionary<string, string>());
    }
}
