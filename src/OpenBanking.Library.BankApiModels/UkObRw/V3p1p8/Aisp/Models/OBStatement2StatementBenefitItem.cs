// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    /// <summary> Set of elements used to provide details of a benefit or reward amount for the statement resource. </summary>
    public partial class OBStatement2StatementBenefitItem
    {
        /// <summary> Initializes a new instance of OBStatement2StatementBenefitItem. </summary>
        /// <param name="type"> Benefit type, in a coded form. </param>
        /// <param name="amount"> Amount of money associated with the statement benefit type. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="type"/> or <paramref name="amount"/> is null. </exception>
        internal OBStatement2StatementBenefitItem(string type, OBActiveOrHistoricCurrencyAndAmount5 amount)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (amount == null)
            {
                throw new ArgumentNullException(nameof(amount));
            }

            Type = type;
            Amount = amount;
        }

        /// <summary> Benefit type, in a coded form. </summary>
        public string Type { get; }
        /// <summary> Amount of money associated with the statement benefit type. </summary>
        public OBActiveOrHistoricCurrencyAndAmount5 Amount { get; }
    }
}
