// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    /// <summary> Balance type, in a coded form. </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBBalanceType1CodeEnum
    {
        /// <summary> ClosingAvailable. </summary>
        [EnumMember(Value = "ClosingAvailable")]
        ClosingAvailable,
        /// <summary> ClosingBooked. </summary>
        [EnumMember(Value = "ClosingBooked")]
        ClosingBooked,
        /// <summary> ClosingCleared. </summary>
        [EnumMember(Value = "ClosingCleared")]
        ClosingCleared,
        /// <summary> Expected. </summary>
        [EnumMember(Value = "Expected")]
        Expected,
        /// <summary> ForwardAvailable. </summary>
        [EnumMember(Value = "ForwardAvailable")]
        ForwardAvailable,
        /// <summary> Information. </summary>
        [EnumMember(Value = "Information")]
        Information,
        /// <summary> InterimAvailable. </summary>
        [EnumMember(Value = "InterimAvailable")]
        InterimAvailable,
        /// <summary> InterimBooked. </summary>
        [EnumMember(Value = "InterimBooked")]
        InterimBooked,
        /// <summary> InterimCleared. </summary>
        [EnumMember(Value = "InterimCleared")]
        InterimCleared,
        /// <summary> OpeningAvailable. </summary>
        [EnumMember(Value = "OpeningAvailable")]
        OpeningAvailable,
        /// <summary> OpeningBooked. </summary>
        [EnumMember(Value = "OpeningBooked")]
        OpeningBooked,
        /// <summary> OpeningCleared. </summary>
        [EnumMember(Value = "OpeningCleared")]
        OpeningCleared,
        /// <summary> PreviouslyClosedBooked. </summary>
        [EnumMember(Value = "PreviouslyClosedBooked")]
        PreviouslyClosedBooked,
        
        [EnumMember(Value = "FundsTransfer")]
        FundsTransfer
    }
}
