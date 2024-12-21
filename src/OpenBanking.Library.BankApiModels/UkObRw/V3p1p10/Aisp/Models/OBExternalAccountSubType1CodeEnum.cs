// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    /// <summary> Specifies the sub type of account (product family group). </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OBExternalAccountSubType1CodeEnum
    {
        /// <summary> ChargeCard. </summary>
        [EnumMember(Value = "ChargeCard")]
        ChargeCard,
        /// <summary> CreditCard. </summary>
        [EnumMember(Value = "CreditCard")]
        CreditCard,
        /// <summary> CurrentAccount. </summary>
        [EnumMember(Value = "CurrentAccount")]
        CurrentAccount,
        /// <summary> EMoney. </summary>
        [EnumMember(Value = "EMoney")]
        EMoney,
        /// <summary> Loan. </summary>
        [EnumMember(Value = "Loan")]
        Loan,
        /// <summary> Mortgage. </summary>
        [EnumMember(Value = "Mortgage")]
        Mortgage,
        /// <summary> PrePaidCard. </summary>
        [EnumMember(Value = "PrePaidCard")]
        PrePaidCard,
        /// <summary> Savings. </summary>
        [EnumMember(Value = "Savings")]
        Savings
    }
}