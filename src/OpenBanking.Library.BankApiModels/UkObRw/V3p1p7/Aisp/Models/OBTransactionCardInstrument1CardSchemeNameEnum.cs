// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Name of the card scheme. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBTransactionCardInstrument1CardSchemeNameEnum
    {
        /// <summary> AmericanExpress. </summary>
        [EnumMember(Value = "American Express")]
        AmericanExpress,
        /// <summary> Diners. </summary>
        [EnumMember(Value = "Diners")]
        Diners,
        /// <summary> Discover. </summary>
        [EnumMember(Value = "Discover")]
        Discover,
        /// <summary> MasterCard. </summary>
        [EnumMember(Value = "MasterCard")]
        MasterCard,
        /// <summary> VISA. </summary>
        [EnumMember(Value = "Visa")]
        Visa,
        
        [EnumMember(Value = "CRD.SCHM.NM.1")]
        Unknown
    }
}
