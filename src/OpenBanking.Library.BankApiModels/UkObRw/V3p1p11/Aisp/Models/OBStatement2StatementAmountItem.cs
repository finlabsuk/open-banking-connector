// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
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
        /// <param name="amount"> Amount of money of the cash balance. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> or <paramref name="amount"/> is null. </exception>
        public OBStatement2StatementAmountItem(OBCreditDebitCode0Enum creditDebitIndicator, string type, OBStatement2StatementAmountItemAmount amount)
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

        /// <summary> Initializes a new instance of OBStatement2StatementAmountItem. </summary>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </param>
        /// <param name="type"> Amount type, in a coded form. </param>
        /// <param name="amount"> Amount of money of the cash balance. </param>
        /// <param name="localAmount"> Optional component providing the equivalent of Amount in local currency. </param>
        [JsonConstructor]
        public OBStatement2StatementAmountItem(OBCreditDebitCode0Enum creditDebitIndicator, string type, OBStatement2StatementAmountItemAmount amount, OBStatement2StatementAmountItemLocalAmount localAmount)
        {
            CreditDebitIndicator = creditDebitIndicator;
            Type = type;
            Amount = amount;
            LocalAmount = localAmount;
        }

        /// <summary>
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </summary>
        public OBCreditDebitCode0Enum CreditDebitIndicator { get; }
        /// <summary> Amount type, in a coded form. </summary>
        public string Type { get; }
        /// <summary> Amount of money of the cash balance. </summary>
        public OBStatement2StatementAmountItemAmount Amount { get; }
        /// <summary> Optional component providing the equivalent of Amount in local currency. </summary>
        public OBStatement2StatementAmountItemLocalAmount LocalAmount { get; }
    }
}
