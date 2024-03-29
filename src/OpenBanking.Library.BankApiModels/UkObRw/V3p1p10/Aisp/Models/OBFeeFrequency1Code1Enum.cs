// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> How often is the overdraft fee/charge calculated for the account. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBFeeFrequency1Code1Enum
    {
        /// <summary> FEAC. </summary>
        [EnumMember(Value = "Feac")]
        Feac,
        /// <summary> FEAO. </summary>
        [EnumMember(Value = "Feao")]
        Feao,
        /// <summary> FECP. </summary>
        [EnumMember(Value = "Fecp")]
        Fecp,
        /// <summary> FEDA. </summary>
        [EnumMember(Value = "Feda")]
        Feda,
        /// <summary> FEHO. </summary>
        [EnumMember(Value = "Feho")]
        Feho,
        /// <summary> FEI. </summary>
        [EnumMember(Value = "FEI")]
        FEI,
        /// <summary> FEMO. </summary>
        [EnumMember(Value = "Femo")]
        Femo,
        /// <summary> FEOA. </summary>
        [EnumMember(Value = "Feoa")]
        Feoa,
        /// <summary> FEOT. </summary>
        [EnumMember(Value = "Feot")]
        Feot,
        /// <summary> FEPC. </summary>
        [EnumMember(Value = "Fepc")]
        Fepc,
        /// <summary> FEPH. </summary>
        [EnumMember(Value = "Feph")]
        Feph,
        /// <summary> FEPO. </summary>
        [EnumMember(Value = "Fepo")]
        Fepo,
        /// <summary> FEPS. </summary>
        [EnumMember(Value = "Feps")]
        Feps,
        /// <summary> FEPT. </summary>
        [EnumMember(Value = "Fept")]
        Fept,
        /// <summary> FEPTA. </summary>
        [EnumMember(Value = "Fepta")]
        Fepta,
        /// <summary> FEPTP. </summary>
        [EnumMember(Value = "Feptp")]
        Feptp,
        /// <summary> FEQU. </summary>
        [EnumMember(Value = "Fequ")]
        Fequ,
        /// <summary> FESM. </summary>
        [EnumMember(Value = "Fesm")]
        Fesm,
        /// <summary> FEST. </summary>
        [EnumMember(Value = "Fest")]
        Fest,
        /// <summary> FEWE. </summary>
        [EnumMember(Value = "Fewe")]
        Fewe,
        /// <summary> FEYE. </summary>
        [EnumMember(Value = "Feye")]
        Feye
    }
}
