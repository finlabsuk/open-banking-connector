// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Set of elements used to define the balance as a numerical representation of the net increases and decreases in an account after a transaction entry is applied to the account. </summary>
    public partial class OBTransactionCashBalance
    {
        /// <summary> Initializes a new instance of OBTransactionCashBalance. </summary>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the balance is a credit or a debit balance. 
        /// Usage: A zero balance is considered to be a credit balance.
        /// </param>
        /// <param name="type"> Balance type, in a coded form. </param>
        /// <param name="amount"> Amount of money of the cash balance after a transaction entry is applied to the account.. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="amount"/> is null. </exception>
        internal OBTransactionCashBalance(OBCreditDebitCode2Enum creditDebitIndicator, OBBalanceType1CodeEnum type, OBTransactionCashBalanceAmount amount)
        {
            if (amount == null)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            CreditDebitIndicator = creditDebitIndicator;
            Type = type;
            Amount = amount;
        }

        /// <summary>
        /// Indicates whether the balance is a credit or a debit balance. 
        /// Usage: A zero balance is considered to be a credit balance.
        /// </summary>
        public OBCreditDebitCode2Enum CreditDebitIndicator { get; }
        /// <summary> Balance type, in a coded form. </summary>
        public OBBalanceType1CodeEnum Type { get; }
        /// <summary> Amount of money of the cash balance after a transaction entry is applied to the account.. </summary>
        public OBTransactionCashBalanceAmount Amount { get; }
    }
}
