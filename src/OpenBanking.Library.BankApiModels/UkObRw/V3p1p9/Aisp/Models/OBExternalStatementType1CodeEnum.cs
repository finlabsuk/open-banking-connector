// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Statement type, in a coded form. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBExternalStatementType1CodeEnum
    {
        /// <summary> AccountClosure. </summary>
        [EnumMember(Value = "AccountClosure")]
        AccountClosure,
        /// <summary> AccountOpening. </summary>
        [EnumMember(Value = "AccountOpening")]
        AccountOpening,
        /// <summary> Annual. </summary>
        [EnumMember(Value = "Annual")]
        Annual,
        /// <summary> Interim. </summary>
        [EnumMember(Value = "Interim")]
        Interim,
        /// <summary> RegularPeriodic. </summary>
        [EnumMember(Value = "RegularPeriodic")]
        RegularPeriodic
    }
}
