// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Validators;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

public class SoftwareStatement : EntityBase, ISupportsValidation
{
    /// <summary>
    ///     Organisation ID from UK Open Banking directory as string.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string OrganisationId { get; init; }

    /// <summary>
    ///     Software statement ID from UK Open Banking directory as string.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string SoftwareId { get; init; }

    /// <summary>
    ///     When true, denotes software statement is defined in UK OB directory sandbox (not production) environment. Defaults
    ///     to false.
    /// </summary>
    public bool SandboxEnvironment { get; init; }

    /// <summary>
    ///     ID of default ObWacCertificate to use for mutual TLS with this software statement.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required Guid DefaultObWacCertificateId { get; init; }

    /// <summary>
    ///     ID of default ObSealCertificate to use for signing JWTs etc with this software statement.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required Guid DefaultObSealCertificateId { get; init; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string DefaultQueryRedirectUrl { get; init; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    [JsonProperty(Required = Required.Always)]
    public required string DefaultFragmentRedirectUrl { get; init; }

    public async Task<ValidationResult> ValidateAsync() =>
        await new SoftwareStatementValidator()
            .ValidateAsync(this);
}
