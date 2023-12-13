// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

public class ObSealCertificate : Base, ISupportsValidation
{
    /// <summary>
    ///     Key ID of associated key (from UK Open Banking Directory) as string.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public string AssociatedKeyId { get; init; } = null!;

    /// <summary>
    ///     Associated key (PKCS #8) provided as PEM file text (with "PRIVATE KEY" label).
    ///     Newlines in PEM file text should be replaced by "\n".
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public SecretDescription AssociatedKey { get; init; } = null!;

    /// <summary>
    ///     OB Seal (signing) certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and
    ///     "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public string Certificate { get; init; } = null!;

    public Task<ValidationResult> ValidateAsync() => Task.FromResult(new ValidationResult());
}
