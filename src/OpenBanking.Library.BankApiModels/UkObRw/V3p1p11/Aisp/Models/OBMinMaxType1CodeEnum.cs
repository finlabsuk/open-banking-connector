// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    /// <summary> Min Max type. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBMinMaxType1CodeEnum
    {
        /// <summary> FMMN. </summary>
        [EnumMember(Value = "Fmmn")]
        Fmmn,
        /// <summary> FMMX. </summary>
        [EnumMember(Value = "Fmmx")]
        Fmmx
    }
}
