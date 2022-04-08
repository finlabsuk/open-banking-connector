// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Fee/Charge Type. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBFeeType1CodeEnum
    {
        /// <summary> FEPF. </summary>
        [EnumMember(Value = "Fepf")]
        Fepf,
        /// <summary> FTOT. </summary>
        [EnumMember(Value = "Ftot")]
        Ftot,
        /// <summary> FYAF. </summary>
        [EnumMember(Value = "Fyaf")]
        Fyaf,
        /// <summary> FYAM. </summary>
        [EnumMember(Value = "Fyam")]
        Fyam,
        /// <summary> FYAQ. </summary>
        [EnumMember(Value = "Fyaq")]
        Fyaq,
        /// <summary> FYCP. </summary>
        [EnumMember(Value = "Fycp")]
        Fycp,
        /// <summary> FYDB. </summary>
        [EnumMember(Value = "Fydb")]
        Fydb,
        /// <summary> FYMI. </summary>
        [EnumMember(Value = "Fymi")]
        Fymi,
        /// <summary> FYXX. </summary>
        [EnumMember(Value = "Fyxx")]
        Fyxx
    }
}
