// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> The OBReadBalance1Data. </summary>
    [SourceApiEquivalent(typeof(V3p1p7.Aisp.Models.OBReadBalance1Data))]
    public partial class OBReadBalance1Data
    {
        /// <summary> Initializes a new instance of OBReadBalance1Data. </summary>
        /// <param name="balance"></param>
        /// <exception cref="ArgumentNullException"> <paramref name="balance"/> is null. </exception>
        public OBReadBalance1Data(IEnumerable<OBReadBalance1DataBalanceItem> balance)
        {
            if (balance == null)
            {
                throw new ArgumentNullException(nameof(balance));
            }

            Balance = balance.ToList();
        }

        /// <summary> Initializes a new instance of OBReadBalance1Data. </summary>
        /// <param name="balance"></param>
        /// <param name="totalValue"> Combined sum of all Amounts in the accounts base currency. </param>
        [JsonConstructor]
        public OBReadBalance1Data(IReadOnlyList<OBReadBalance1DataBalanceItem> balance, OBReadBalance1DataTotalValue totalValue)
        {
            Balance = balance;
            TotalValue = totalValue;
        }

        /// <summary> Gets the balance. </summary>
        public IReadOnlyList<OBReadBalance1DataBalanceItem> Balance { get; }
        /// <summary> Combined sum of all Amounts in the accounts base currency. </summary>
        public OBReadBalance1DataTotalValue TotalValue { get; }
    }
}