// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using Newtonsoft.Json;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;

public class SoftwareStatement : Base, ISupportsValidation
{
    /// <summary>
    ///     Organisation ID from UK Open Banking directory as string.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public string OrganisationId { get; init; } = null!;

    /// <summary>
    ///     Software statement ID from UK Open Banking directory as string.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public string SoftwareId { get; init; } = null!;

    /// <summary>
    ///     When true, denotes software statement is defined in UK OB directory sandbox (not production) environment. Defaults
    ///     to false.
    /// </summary>
    public bool SandboxEnvironment { get; init; }

    /// <summary>
    ///     ID of default ObWacCertificate to use for mutual TLS with this software statement.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public Guid DefaultObWacCertificateId { get; init; }

    /// <summary>
    ///     ID of default ObSealCertificate to use for signing JWTs etc with this software statement.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public Guid DefaultObSealCertificateId { get; init; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public string DefaultQueryRedirectUrl { get; init; } = null!;

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    [Required]
    [JsonProperty(Required = Required.Always)]
    public string DefaultFragmentRedirectUrl { get; init; } = null!;

    public Task<ValidationResult> ValidateAsync() => Task.FromResult(new ValidationResult());
}
