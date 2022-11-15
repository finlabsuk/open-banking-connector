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
    [EnumMember(Value = "Danske")]
    Danske,
    
    [EnumMember(Value = "Hsbc")]
    Hsbc,

    [EnumMember(Value = "Lloyds")]
    Lloyds,

    [EnumMember(Value = "Obie")]
    Obie,

    [EnumMember(Value = "Monzo")]
    Monzo,

    [EnumMember(Value = "NatWest")]
    NatWest
}
