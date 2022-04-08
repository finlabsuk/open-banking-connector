// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Specifies the status of the standing order in code form. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBExternalStandingOrderStatus1CodeEnum
    {
        /// <summary> Active. </summary>
        [EnumMember(Value = "Active")]
        Active,
        /// <summary> Inactive. </summary>
        [EnumMember(Value = "Inactive")]
        Inactive
    }
}
