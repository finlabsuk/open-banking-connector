// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class EncryptionKeyDescriptionEnvFile : Dictionary<string, EncryptionKeyDescriptionEnv>;

public class EncryptionKeyDescriptionEnv
{
    [JsonPropertyName("encryptionKeyDescriptionName")]
    public required string  EncryptionKeyDescriptionName { get; init; }
    
    [JsonConverter(typeof(JsonStringEnumConverter<SecretSource>))]
    [JsonPropertyName("encryptionKeySource")]
    public SecretSource EncryptionKeySource { get; init; } = SecretSource.Configuration;

    [JsonPropertyName("encryptionKeyName")]
    public required string EncryptionKeyName { get; init; }
}
