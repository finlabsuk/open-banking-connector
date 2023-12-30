// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

[JsonConverter(typeof(StringEnumConverter))]
public enum DynamicClientRegistrationApiVersion
{
    [EnumMember(Value = "Version3p1")]
    Version3p1,

    [EnumMember(Value = "Version3p2")]
    Version3p2,

    [EnumMember(Value = "Version3p3")]
    Version3p3
}
