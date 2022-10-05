// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary>
    /// Indicates whether the amount is a credit or a debit. 
    /// Usage: A zero amount is considered to be a credit amount.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBCreditDebitCode0Enum
    {
        /// <summary> Credit. </summary>
        [EnumMember(Value = "Credit")]
        Credit,
        /// <summary> Debit. </summary>
        [EnumMember(Value = "Debit")]
        Debit
    }
}
