// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;

[JsonConverter(typeof(StringEnumConverter))]
public enum BankGroupEnum
{
    [EnumMember(Value = "Barclays")]
    Barclays,

    [EnumMember(Value = "Danske")]
    Danske,

    [EnumMember(Value = "Hsbc")]
    Hsbc,

    [EnumMember(Value = "Lloyds")]
    Lloyds,

    [EnumMember(Value = "Monzo")]
    Monzo,

    [EnumMember(Value = "Nationwide")]
    Nationwide,

    [EnumMember(Value = "NatWest")]
    NatWest,

    [EnumMember(Value = "Obie")]
    Obie,

    [EnumMember(Value = "Revolut")]
    Revolut,

    [EnumMember(Value = "Santander")]
    Santander,

    [EnumMember(Value = "Starling")]
    Starling
}
