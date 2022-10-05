// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Indicates whether the transaction is a credit or a debit entry. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBCreditDebitCode1Enum
    {
        /// <summary> Credit. </summary>
        [EnumMember(Value = "Credit")]
        Credit,
        /// <summary> Debit. </summary>
        [EnumMember(Value = "Debit")]
        Debit
    }
}
