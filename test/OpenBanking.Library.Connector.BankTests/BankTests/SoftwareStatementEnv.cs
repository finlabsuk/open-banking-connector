﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class SoftwareStatementEnvFile : Dictionary<string, SoftwareStatementEnv>;

public class SoftwareStatementEnv
{
    [JsonPropertyName("softwareStatementName")]
    public required string SoftwareStatementName { get; init; }

    [JsonPropertyName("obSealAssociatedKeyId")]
    public required string ObSealAssociatedKeyId { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter<SecretSource>))]
    [JsonPropertyName("obSealAssociatedKeySource")]
    public SecretSource ObSealAssociatedKeySource { get; init; } = SecretSource.Configuration;

    [JsonPropertyName("obSealAssociatedKeyName")]
    public required string ObSealAssociatedKeyName { get; init; }

    [JsonPropertyName("obSealCertificate")]
    public required string ObSealCertificate { get; init; }

    [JsonConverter(typeof(JsonStringEnumConverter<SecretSource>))]
    [JsonPropertyName("obWacAssociatedKeySource")]
    public SecretSource ObWacAssociatedKeySource { get; init; } = SecretSource.Configuration;

    [JsonPropertyName("obWacAssociatedKeyName")]
    public required string ObWacAssociatedKeyName { get; init; }

    [JsonPropertyName("obWacCertificate")]
    public required string ObWacCertificate { get; init; }

    [JsonPropertyName("organisationId")]
    public required string OrganisationId { get; init; }

    [JsonPropertyName("softwareId")]
    public required string SoftwareId { get; init; }

    [JsonPropertyName("sandboxEnvironment")]
    public required bool SandboxEnvironment { get; init; }

    [JsonPropertyName("defaultQueryRedirectUrl")]
    public required string DefaultQueryRedirectUrl { get; init; }

    [JsonPropertyName("defaultFragmentRedirectUrl")]
    public required string DefaultFragmentRedirectUrl { get; init; }
}
