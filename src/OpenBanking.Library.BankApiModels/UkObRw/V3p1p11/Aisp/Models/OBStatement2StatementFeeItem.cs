// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> Set of elements used to provide details of a fee for the statement resource. </summary>
    public partial class OBStatement2StatementFeeItem
    {
        /// <summary> Initializes a new instance of OBStatement2StatementFeeItem. </summary>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </param>
        /// <param name="type"> Fee type, in a coded form. </param>
        /// <param name="amount"> Amount of money associated with the statement fee type. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> or <paramref name="amount"/> is null. </exception>
        internal OBStatement2StatementFeeItem(OBCreditDebitCode0Enum creditDebitIndicator, string type, OBActiveOrHistoricCurrencyAndAmount6 amount)
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

        /// <summary> Initializes a new instance of OBStatement2StatementFeeItem. </summary>
        /// <param name="description"> Description that may be available for the statement fee. </param>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </param>
        /// <param name="type"> Fee type, in a coded form. </param>
        /// <param name="rate"> Rate charged for Statement Fee (where it is charged in terms of a rate rather than an amount). </param>
        /// <param name="rateType"> Description that may be available for the statement fee rate type. </param>
        /// <param name="frequency"> How frequently the fee is applied to the Account. </param>
        /// <param name="amount"> Amount of money associated with the statement fee type. </param>
        internal OBStatement2StatementFeeItem(string description, OBCreditDebitCode0Enum creditDebitIndicator, string type, float? rate, string rateType, string frequency, OBActiveOrHistoricCurrencyAndAmount6 amount)
        {
            Description = description;
            CreditDebitIndicator = creditDebitIndicator;
            Type = type;
            Rate = rate;
            RateType = rateType;
            Frequency = frequency;
            Amount = amount;
        }

        /// <summary> Description that may be available for the statement fee. </summary>
        public string Description { get; }
        /// <summary>
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </summary>
        public OBCreditDebitCode0Enum CreditDebitIndicator { get; }
        /// <summary> Fee type, in a coded form. </summary>
        public string Type { get; }
        /// <summary> Rate charged for Statement Fee (where it is charged in terms of a rate rather than an amount). </summary>
        public float? Rate { get; }
        /// <summary> Description that may be available for the statement fee rate type. </summary>
        public string RateType { get; }
        /// <summary> How frequently the fee is applied to the Account. </summary>
        public string Frequency { get; }
        /// <summary> Amount of money associated with the statement fee type. </summary>
        public OBActiveOrHistoricCurrencyAndAmount6 Amount { get; }
    }
}
