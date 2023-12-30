// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;

public interface ISoftwareStatementPublicQuery : IEntityBaseQuery
{
    /// <summary>
    ///     Organisation ID from UK Open Banking directory as string.
    /// </summary>
    public string OrganisationId { get; }

    /// <summary>
    ///     Software statement ID from UK Open Banking directory as string.
    /// </summary>
    public string SoftwareId { get; }

    /// <summary>
    ///     When true, denotes software statement is defined in UK OB directory sandbox (not production) environment.
    /// </summary>
    public bool SandboxEnvironment { get; }

    /// <summary>
    ///     ID of default ObWacCertificate to use for mutual TLS with this software statement.
    /// </summary>
    public Guid DefaultObWacCertificateId { get; }

    /// <summary>
    ///     ID of default ObSealCertificate to use for signing JWTs etc with this software statement.
    /// </summary>
    public Guid DefaultObSealCertificateId { get; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    public string DefaultQueryRedirectUrl { get; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    public string DefaultFragmentRedirectUrl { get; }
}

/// <summary>
///     Response to SoftwareStatement read and create requests.
/// </summary>
public class SoftwareStatementResponse : EntityBaseResponse, ISoftwareStatementPublicQuery
{
    /// <summary>
    ///     Organisation ID from UK Open Banking directory as string.
    /// </summary>
    public required string OrganisationId { get; init; }

    /// <summary>
    ///     Software statement ID from UK Open Banking directory as string.
    /// </summary>
    public required string SoftwareId { get; init; }

    /// <summary>
    ///     When true, denotes software statement is defined in UK OB directory sandbox (not production) environment.
    /// </summary>
    public required bool SandboxEnvironment { get; init; }

    /// <summary>
    ///     ID of default ObWacCertificate to use for mutual TLS with this software statement.
    /// </summary>
    public required Guid DefaultObWacCertificateId { get; init; }

    /// <summary>
    ///     ID of default ObSealCertificate to use for signing JWTs etc with this software statement.
    /// </summary>
    public required Guid DefaultObSealCertificateId { get; init; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    public required string DefaultQueryRedirectUrl { get; init; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    public required string DefaultFragmentRedirectUrl { get; init; }
}
