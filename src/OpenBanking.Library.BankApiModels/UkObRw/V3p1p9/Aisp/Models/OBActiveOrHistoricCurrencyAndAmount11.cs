// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> The amount of the last (most recent) Standing Order instruction. </summary>
    public partial class OBActiveOrHistoricCurrencyAndAmount11
    {
        /// <summary> Initializes a new instance of OBActiveOrHistoricCurrencyAndAmount11. </summary>
        /// <param name="amount"> A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217. </param>
        /// <param name="currency"> A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 &quot;Codes for the representation of currencies and funds&quot;. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="amount"/> or <paramref name="currency"/> is null. </exception>
        internal OBActiveOrHistoricCurrencyAndAmount11(string amount, string currency)
        {
            if (amount == null)
            {
                throw new ArgumentNullException(nameof(amount));
            }
            if (currency == null)
            {
                throw new ArgumentNullException(nameof(currency));
            }

            Amount = amount;
            Currency = currency;
        }

        /// <summary> A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217. </summary>
        public string Amount { get; }
        /// <summary> A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 &quot;Codes for the representation of currencies and funds&quot;. </summary>
        public string Currency { get; }
    }
}
