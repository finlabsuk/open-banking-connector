// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Status of a transaction entry on the books of the account servicer. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBEntryStatus1CodeEnum
    {
        /// <summary> Booked. </summary>
        [EnumMember(Value = "Booked")]
        Booked,
        /// <summary> Pending. </summary>
        [EnumMember(Value = "Pending")]
        Pending
    }
}
