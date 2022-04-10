// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountAndTransactionApiVersionEnum
    {
        [EnumMember(Value = "Version3p1p7")]
        Version3p1p7,

        [EnumMember(Value = "Version3p1p8")]
        Version3p1p8,

        [EnumMember(Value = "Version3p1p9")]
        Version3p1p9
    }
}
