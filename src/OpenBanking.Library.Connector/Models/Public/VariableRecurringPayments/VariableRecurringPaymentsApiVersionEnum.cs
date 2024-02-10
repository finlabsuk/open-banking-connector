// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

[JsonConverter(typeof(StringEnumConverter))]
public enum VariableRecurringPaymentsApiVersion
{
    // Support not currently required
    // [EnumMember(Value = "Version3p1p8")]
    // Version3p1p8,

    [EnumMember(Value = "Version3p1p11")]
    Version3p1p11,

    [EnumMember(Value = "VersionPublic")]
    VersionPublic = Version3p1p11
}
