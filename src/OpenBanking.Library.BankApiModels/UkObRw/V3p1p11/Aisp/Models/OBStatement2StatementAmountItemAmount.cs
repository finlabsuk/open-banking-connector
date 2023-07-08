// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> Amount of money of the cash balance. </summary>
    public partial class OBStatement2StatementAmountItemAmount
    {
        /// <summary> Initializes a new instance of OBStatement2StatementAmountItemAmount. </summary>
        /// <param name="amount"> A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217. </param>
        /// <param name="currency"> A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 &quot;Codes for the representation of currencies and funds&quot;. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="amount"/> or <paramref name="currency"/> is null. </exception>
        public OBStatement2StatementAmountItemAmount(string amount, string currency)
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

        /// <summary> Initializes a new instance of OBStatement2StatementAmountItemAmount. </summary>
        /// <param name="amount"> A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217. </param>
        /// <param name="currency"> A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 &quot;Codes for the representation of currencies and funds&quot;. </param>
        /// <param name="subType"> The amount in the domestic or base accounting currency. Default is Base Currency (BCUR) if not specified. </param>
        [JsonConstructor]
        public OBStatement2StatementAmountItemAmount(string amount, string currency, OBStatement2StatementAmountSubTypeEnum? subType)
        {
            Amount = amount;
            Currency = currency;
            SubType = subType;
        }

        /// <summary> A number of monetary units specified in an active currency where the unit of currency is explicit and compliant with ISO 4217. </summary>
        public string Amount { get; }
        /// <summary> A code allocated to a currency by a Maintenance Agency under an international identification scheme, as described in the latest edition of the international standard ISO 4217 &quot;Codes for the representation of currencies and funds&quot;. </summary>
        public string Currency { get; }
        /// <summary> The amount in the domestic or base accounting currency. Default is Base Currency (BCUR) if not specified. </summary>
        public OBStatement2StatementAmountSubTypeEnum? SubType { get; }
    }
}
