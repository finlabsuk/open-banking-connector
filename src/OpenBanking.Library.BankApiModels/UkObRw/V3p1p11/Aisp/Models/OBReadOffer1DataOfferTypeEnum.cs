// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> Offer type, in a coded form. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBReadOffer1DataOfferTypeEnum
    {
        /// <summary> BalanceTransfer. </summary>
        [EnumMember(Value = "BalanceTransfer")]
        BalanceTransfer,
        /// <summary> LimitIncrease. </summary>
        [EnumMember(Value = "LimitIncrease")]
        LimitIncrease,
        /// <summary> MoneyTransfer. </summary>
        [EnumMember(Value = "MoneyTransfer")]
        MoneyTransfer,
        /// <summary> Other. </summary>
        [EnumMember(Value = "Other")]
        Other,
        /// <summary> PromotionalRate. </summary>
        [EnumMember(Value = "PromotionalRate")]
        PromotionalRate
    }
}
