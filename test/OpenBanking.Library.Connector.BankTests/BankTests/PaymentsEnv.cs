// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class PaymentsEnvFile : Dictionary<string, PaymentsEnv>;

public class PaymentsEnv
{
    [JsonPropertyName("accountName")]
    public required string AccountName { get; init; }

    [JsonPropertyName("bankAccountSchemeName")]
    public required string BankAccountSchemeName { get; init; }

    [JsonPropertyName("bankAccountId")]
    public required string BankAccountId { get; init; }

    [JsonPropertyName("bankAccountName")]
    public required string BankAccountName { get; init; }
}
