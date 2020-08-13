// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ApiVersion
    {
        [EnumMember(Value = "v3p2")] V3P2
    }
}
