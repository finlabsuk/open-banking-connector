// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;

[JsonConverter(typeof(StringEnumConverter))]
public enum SecretSource
{
    [EnumMember(Value = "Configuration")]
    Configuration
}

public class SecretDescription
{
    /// <summary>
    ///     Source of secret value.
    /// </summary>
    public SecretSource Source { get; } = SecretSource.Configuration;

    /// <summary>
    ///     Name of secret value.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public required string Name { get; init; }
}
