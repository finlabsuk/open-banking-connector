// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Overdraft fee type. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBOverdraftFeeType1CodeEnum
    {
        /// <summary> FBAO. </summary>
        [EnumMember(Value = "Fbao")]
        Fbao,
        /// <summary> FBAR. </summary>
        [EnumMember(Value = "Fbar")]
        Fbar,
        /// <summary> FBEB. </summary>
        [EnumMember(Value = "Fbeb")]
        Fbeb,
        /// <summary> FBIT. </summary>
        [EnumMember(Value = "Fbit")]
        Fbit,
        /// <summary> FBOR. </summary>
        [EnumMember(Value = "Fbor")]
        Fbor,
        /// <summary> FBOS. </summary>
        [EnumMember(Value = "Fbos")]
        Fbos,
        /// <summary> FBSC. </summary>
        [EnumMember(Value = "Fbsc")]
        Fbsc,
        /// <summary> FBTO. </summary>
        [EnumMember(Value = "Fbto")]
        Fbto,
        /// <summary> FBUB. </summary>
        [EnumMember(Value = "Fbub")]
        Fbub,
        /// <summary> FBUT. </summary>
        [EnumMember(Value = "Fbut")]
        Fbut,
        /// <summary> FTOT. </summary>
        [EnumMember(Value = "Ftot")]
        Ftot,
        /// <summary> FTUT. </summary>
        [EnumMember(Value = "Ftut")]
        Ftut
    }
}