// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
public partial class SoftwareStatementEntity :
    BaseEntity,
    ISoftwareStatementPublicQuery
{
    public SoftwareStatementEntity(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string organisationId,
        string softwareId,
        bool sandboxEnvironment,
        Guid defaultObWacCertificateId,
        Guid defaultObSealCertificateId,
        string defaultQueryRedirectUrl,
        string defaultFragmentRedirectUrl) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
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

    [ForeignKey(nameof(DefaultObWacCertificateId))]
    public ObWacCertificateEntity DefaultObWacCertificateNavigation { get; private set; } = null!;

    [ForeignKey(nameof(DefaultObSealCertificateId))]
    public ObSealCertificateEntity DefaultObSealCertificateNavigation { get; private set; } = null!;

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
    ///     ID of default <see cref="ObWacCertificateEntity" /> to use for mutual TLS with this software statement.
    /// </summary>
    public Guid DefaultObWacCertificateId { get; }

    /// <summary>
    ///     ID of default <see cref="ObSealCertificateEntity" /> to use for signing JWTs etc with this software statement.
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

public partial class SoftwareStatementEntity :
    ISupportsFluentLocalEntityGet<SoftwareStatementResponse>
{
    public SoftwareStatementResponse PublicGetLocalResponse => new(
        Id,
        Created,
        CreatedBy,
        Reference,
        OrganisationId,
        SoftwareId,
        SandboxEnvironment,
        DefaultObWacCertificateId,
        DefaultObSealCertificateId,
        DefaultQueryRedirectUrl,
        DefaultFragmentRedirectUrl);
}
