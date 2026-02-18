// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;

public class EncryptionKeyDescription : EntityBase, ISupportsValidation
{
    /// <summary>
    ///     Description of encryption key secret. An encryption key (256-bit) enables symmetric encryption (AES-256-GCM) of
    ///     sensitive data in the database such as bank tokens. It should be specified as a base64-encoded string.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required SecretDescription Key { get; init; }

    /// <summary>
    ///     When creating a database record for this encryption key description, update the database settings table to
    ///     set this as the current encryption key.
    /// </summary>
    public bool SetAsCurrentEncryptionKey { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new EncryptionKeyDescriptionValidator()
            .ValidateAsync(this);
}
