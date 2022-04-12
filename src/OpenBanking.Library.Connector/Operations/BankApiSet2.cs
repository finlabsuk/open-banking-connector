// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class BankApiSet2
    {
        public AccountAndTransactionApi? AccountAndTransactionApi { get; set; }


        public PaymentInitiationApi? PaymentInitiationApi { get; set; }

        public VariableRecurringPaymentsApi? VariableRecurringPaymentsApi { get; set; }
    }
}
