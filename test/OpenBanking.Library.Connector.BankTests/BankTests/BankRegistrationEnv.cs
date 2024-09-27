﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class BankRegistrationEnvFile : Dictionary<string, BankRegistrationEnv>;

public class BankRegistrationEnv
{
    [JsonPropertyName("softwareStatement")]
    public required string SoftwareStatement { get; init; }

    [JsonPropertyName("registrationScope")]
    [JsonConverter(typeof(JsonStringEnumConverter<RegistrationScopeEnum>))]
    public required RegistrationScopeEnum RegistrationScope { get; init; }

    [JsonPropertyName("bankProfile")]
    [JsonConverter(typeof(JsonStringEnumConverter<BankProfileEnum>))]
    public required BankProfileEnum BankProfile { get; init; }

    [JsonPropertyName("externalApiBankRegistrationId")]
    public required string ExternalApiBankRegistrationId { get; init; }

    [JsonPropertyName("externalApiClientSecretName")]
    public string? ExternalApiClientSecretName { get; init; }

    [JsonPropertyName("externalApiBankRegistrationRegistrationAccessTokenName")]
    public string? ExternalApiBankRegistrationRegistrationAccessTokenName { get; init; }

    [JsonPropertyName("testAccountAccessConsent")]
    public required bool TestAccountAccessConsent { get; init; }

    [JsonPropertyName("testDomesticPaymentConsent")]
    public required bool TestDomesticPaymentConsent { get; init; }

    [JsonPropertyName("testDomesticVrpConsent")]
    public required bool TestDomesticVrpConsent { get; init; }

    [JsonPropertyName("externalApiAccountAccessConsentId")]
    public string? ExternalApiAccountAccessConsentId { get; init; }

    [JsonPropertyName("sandboxAuthUserName")]
    public string? SandboxAuthUserName { get; init; }

    [JsonPropertyName("sandboxAuthPassword")]
    public string? SandboxAuthPassword { get; init; }

    [JsonPropertyName("sandboxAuthExtraWord1")]
    public string? SandboxAuthExtraWord1 { get; init; }

    [JsonPropertyName("sandboxAuthExtraWord2")]
    public string? SandboxAuthExtraWord2 { get; init; }

    [JsonPropertyName("sandboxAuthExtraWord3")]
    public string? SandboxAuthExtraWord3 { get; init; }
}
