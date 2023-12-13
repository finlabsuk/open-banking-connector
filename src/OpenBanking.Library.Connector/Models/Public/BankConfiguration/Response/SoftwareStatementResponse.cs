// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;

public interface ISoftwareStatementPublicQuery : IBaseQuery
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
public class SoftwareStatementResponse : LocalObjectBaseResponse, ISoftwareStatementPublicQuery
{
    public SoftwareStatementResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        string organisationId,
        string softwareId,
        bool sandboxEnvironment,
        Guid defaultObWacCertificateId,
        Guid defaultObSealCertificateId,
        string defaultQueryRedirectUrl,
        string defaultFragmentRedirectUrl) : base(id, created, createdBy, reference)
    {
        OrganisationId = organisationId ?? throw new ArgumentNullException(nameof(organisationId));
        SoftwareId = softwareId ?? throw new ArgumentNullException(nameof(softwareId));
        SandboxEnvironment = sandboxEnvironment;
        DefaultObWacCertificateId = defaultObWacCertificateId;
        DefaultObSealCertificateId = defaultObSealCertificateId;
        DefaultQueryRedirectUrl =
            defaultQueryRedirectUrl ?? throw new ArgumentNullException(nameof(defaultQueryRedirectUrl));
        DefaultFragmentRedirectUrl = defaultFragmentRedirectUrl ??
                                     throw new ArgumentNullException(nameof(defaultFragmentRedirectUrl));
    }


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
