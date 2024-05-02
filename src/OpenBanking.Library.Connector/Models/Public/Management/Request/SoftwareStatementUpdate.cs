// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FluentValidation.Results;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;

public class SoftwareStatementUpdate : ISupportsValidation
{
    /// <summary>
    ///     ID of default ObWacCertificate to use for mutual TLS with this software statement.
    /// </summary>
    public Guid? DefaultObWacCertificateId { get; init; }

    /// <summary>
    ///     ID of default ObSealCertificate to use for signing JWTs etc with this software statement.
    /// </summary>
    public Guid? DefaultObSealCertificateId { get; init; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    public string? DefaultQueryRedirectUrl { get; init; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    public string? DefaultFragmentRedirectUrl { get; init; }

    public Task<ValidationResult> ValidateAsync() =>
        Task.FromResult(new ValidationResult());
}
