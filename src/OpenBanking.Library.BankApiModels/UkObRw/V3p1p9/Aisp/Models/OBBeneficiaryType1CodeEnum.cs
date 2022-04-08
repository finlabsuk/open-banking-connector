// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    /// <summary> Specifies the Beneficiary Type. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBBeneficiaryType1CodeEnum
    {
        /// <summary> Trusted. </summary>
        [EnumMember(Value = "Trusted")]
        Trusted,
        /// <summary> Ordinary. </summary>
        [EnumMember(Value = "Ordinary")]
        Ordinary
    }
}
