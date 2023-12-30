// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;

public class ObWacCertificate : EntityBase, ISupportsValidation
{
    /// <summary>
    ///     Associated key (PKCS #8) as "stringified" PEM file with escaped newline characters ("\n") and "PRIVATE KEY" label.
    ///     Example: "-----BEGIN PRIVATE KEY-----\nABC\n-----END PRIVATE KEY-----\n"
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required SecretDescription AssociatedKey { get; init; }

    /// <summary>
    ///     OB WAC (transport) certificate (X.509) as "stringified" PEM file with escaped newline characters ("\n") and
    ///     "CERTIFICATE"
    ///     label.
    ///     Example: "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string Certificate { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new ObWacCertificateValidator()
            .ValidateAsync(this);
}
