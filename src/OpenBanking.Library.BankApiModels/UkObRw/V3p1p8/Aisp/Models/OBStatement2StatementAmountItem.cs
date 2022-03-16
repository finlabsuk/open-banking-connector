// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace AccountAndTransactionAPISpecification.Models
{
    /// <summary> Set of elements used to provide details of a generic amount for the statement resource. </summary>
    public partial class OBStatement2StatementAmountItem
    {
        /// <summary> Initializes a new instance of OBStatement2StatementAmountItem. </summary>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </param>
        /// <param name="type"> Amount type, in a coded form. </param>
        /// <param name="amount"> Amount of money associated with the amount type. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> or <paramref name="amount"/> is null. </exception>
        internal OBStatement2StatementAmountItem(OBCreditDebitCode0Enum creditDebitIndicator, string type, OBActiveOrHistoricCurrencyAndAmount8 amount)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (amount == null)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            CreditDebitIndicator = creditDebitIndicator;
            Type = type;
            Amount = amount;
        }

        /// <summary>
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </summary>
        public OBCreditDebitCode0Enum CreditDebitIndicator { get; }
        /// <summary> Amount type, in a coded form. </summary>
        public string Type { get; }
        /// <summary> Amount of money associated with the amount type. </summary>
        public OBActiveOrHistoricCurrencyAndAmount8 Amount { get; }
    }
}
