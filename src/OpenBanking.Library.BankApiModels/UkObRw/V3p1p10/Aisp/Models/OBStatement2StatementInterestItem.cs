// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Set of elements used to provide details of a generic interest amount related to the statement resource. </summary>
    public partial class OBStatement2StatementInterestItem
    {
        /// <summary> Initializes a new instance of OBStatement2StatementInterestItem. </summary>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </param>
        /// <param name="type"> Interest amount type, in a coded form. </param>
        /// <param name="amount"> Amount of money associated with the statement interest amount type. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> or <paramref name="amount"/> is null. </exception>
        public OBStatement2StatementInterestItem(OBCreditDebitCode0Enum creditDebitIndicator, string type, OBActiveOrHistoricCurrencyAndAmount7 amount)
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

        /// <summary> Initializes a new instance of OBStatement2StatementInterestItem. </summary>
        /// <param name="description"> Description that may be available for the statement interest. </param>
        /// <param name="creditDebitIndicator">
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </param>
        /// <param name="type"> Interest amount type, in a coded form. </param>
        /// <param name="rate"> field representing a percentage (e.g. 0.05 represents 5% and 0.9525 represents 95.25%). Note the number of decimal places may vary. </param>
        /// <param name="rateType"> Description that may be available for the statement Interest rate type. </param>
        /// <param name="frequency"> Specifies the statement fee type requested. </param>
        /// <param name="amount"> Amount of money associated with the statement interest amount type. </param>
        [JsonConstructor]
        public OBStatement2StatementInterestItem(string description, OBCreditDebitCode0Enum creditDebitIndicator, string type, float? rate, string rateType, string frequency, OBActiveOrHistoricCurrencyAndAmount7 amount)
        {
            Description = description;
            CreditDebitIndicator = creditDebitIndicator;
            Type = type;
            Rate = rate;
            RateType = rateType;
            Frequency = frequency;
            Amount = amount;
        }

        /// <summary> Description that may be available for the statement interest. </summary>
        public string Description { get; }
        /// <summary>
        /// Indicates whether the amount is a credit or a debit. 
        /// Usage: A zero amount is considered to be a credit amount.
        /// </summary>
        public OBCreditDebitCode0Enum CreditDebitIndicator { get; }
        /// <summary> Interest amount type, in a coded form. </summary>
        public string Type { get; }
        /// <summary> field representing a percentage (e.g. 0.05 represents 5% and 0.9525 represents 95.25%). Note the number of decimal places may vary. </summary>
        public float? Rate { get; }
        /// <summary> Description that may be available for the statement Interest rate type. </summary>
        public string RateType { get; }
        /// <summary> Specifies the statement fee type requested. </summary>
        public string Frequency { get; }
        /// <summary> Amount of money associated with the statement interest amount type. </summary>
        public OBActiveOrHistoricCurrencyAndAmount7 Amount { get; }
    }
}
