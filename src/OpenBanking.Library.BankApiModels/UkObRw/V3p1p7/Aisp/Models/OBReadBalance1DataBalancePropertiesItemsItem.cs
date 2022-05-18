// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Set of elements used to provide details on the credit line. </summary>
    public partial class OBReadBalance1DataBalancePropertiesItemsItem
    {
        /// <summary> Initializes a new instance of OBReadBalance1DataBalancePropertiesItemsItem. </summary>
        /// <param name="included">
        /// Indicates whether or not the credit line is included in the balance of the account.
        /// Usage: If not present, credit line is not included in the balance amount of the account.
        /// </param>
        public OBReadBalance1DataBalancePropertiesItemsItem(bool included)
        {
            Included = included;
        }

        /// <summary> Initializes a new instance of OBReadBalance1DataBalancePropertiesItemsItem. </summary>
        /// <param name="included">
        /// Indicates whether or not the credit line is included in the balance of the account.
        /// Usage: If not present, credit line is not included in the balance amount of the account.
        /// </param>
        /// <param name="type"> Limit type, in a coded form. </param>
        /// <param name="amount"> Amount of money of the credit line. </param>
        [JsonConstructor] public OBReadBalance1DataBalancePropertiesItemsItem(bool included, OBReadBalance1DataBalanceCreditLineTypeEnum? type, OBReadBalance1DataBalanceItemCreditLineItemAmount amount)
        {
            Included = included;
            Type = type;
            Amount = amount;
        }

        /// <summary>
        /// Indicates whether or not the credit line is included in the balance of the account.
        /// Usage: If not present, credit line is not included in the balance amount of the account.
        /// </summary>
        public bool Included { get; }
        /// <summary> Limit type, in a coded form. </summary>
        public OBReadBalance1DataBalanceCreditLineTypeEnum? Type { get; }
        /// <summary> Amount of money of the credit line. </summary>
        public OBReadBalance1DataBalanceItemCreditLineItemAmount Amount { get; }
    }
}
