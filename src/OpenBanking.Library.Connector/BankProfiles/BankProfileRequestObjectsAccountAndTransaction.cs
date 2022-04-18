// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public enum AccountAccessConsentType
    {
        MaximumPermissions
    }

    public partial class BankProfile
    {
        public AccountAccessConsent AccountAccessConsentRequest(
            Guid bankRegistrationId,
            Guid accountAndTransactionApiId,
            AccountAccessConsentType accountAccessConsentType,
            string? createdBy)
        {
            var accountAccessConsentRequest = new AccountAccessConsent
            {
                ExternalApiRequest = new OBReadConsent1(
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
                    new Dictionary<string, string>()),
                CreatedBy = createdBy,
                AccountAndTransactionApiId = accountAndTransactionApiId,
                BankRegistrationId = bankRegistrationId,
            };

            return AccountAndTransactionApiSettings.AccountAccessConsentAdjustments(accountAccessConsentRequest);
        }
    }
}
