// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;

/// <summary>
///     Banks for which bank profiles have been created. Called BankProfileEnum to avoid confusion with the associated
///     class BankProfile.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum BankProfileEnum
{
    [EnumMember(Value = "Obie_Modelo")]
    Obie_Modelo,

    [EnumMember(Value = "NatWest")]
    NatWest,

    [EnumMember(Value = "VrpHackathon")]
    VrpHackathon,

    [EnumMember(Value = "Santander")]
    Santander,

    [EnumMember(Value = "Barclays")]
    Barclays,

    [EnumMember(Value = "RoyalBankOfScotland")]
    RoyalBankOfScotland,

    [EnumMember(Value = "NewDayAmazon")]
    NewDayAmazon,

    [EnumMember(Value = "Nationwide")]
    Nationwide,

    [EnumMember(Value = "Lloyds")]
    Lloyds,

    [EnumMember(Value = "Hsbc_FirstDirect")]
    Hsbc_FirstDirect,

    [EnumMember(Value = "Hsbc_Sandbox")]
    Hsbc_Sandbox,

    [EnumMember(Value = "Hsbc_UkBusiness")]
    Hsbc_UkBusiness,

    [EnumMember(Value = "Hsbc_UkKinetic")]
    Hsbc_UkKinetic,

    [EnumMember(Value = "Hsbc_UkPersonal")]
    Hsbc_UkPersonal,

    [EnumMember(Value = "Danske")]
    Danske,

    [EnumMember(Value = "AlliedIrish")]
    AlliedIrish,

    [EnumMember(Value = "Monzo")]
    Monzo,

    [EnumMember(Value = "Tsb")]
    Tsb
}
