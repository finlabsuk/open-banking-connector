﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal partial class SoftwareStatementEntity :
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
        DateTimeOffset modified,
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
        Modified = modified;
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

    public ObWacCertificateEntity DefaultObWacCertificateNavigation { get; } = null!;

    public ObSealCertificateEntity DefaultObSealCertificateNavigation { get; } = null!;

    public DateTimeOffset Modified { get; private set; }

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
    public Guid DefaultObWacCertificateId { get; private set; }

    /// <summary>
    ///     ID of default <see cref="ObSealCertificateEntity" /> to use for signing JWTs etc with this software statement.
    /// </summary>
    public Guid DefaultObSealCertificateId { get; private set; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    public string DefaultQueryRedirectUrl { get; private set; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    public string DefaultFragmentRedirectUrl { get; private set; }

    public string GetRedirectUri(
        OAuth2ResponseMode responseMode,
        string? registrationFragmentRedirectUrl,
        string? registrationQueryRedirectUrl) =>
        responseMode switch
        {
            OAuth2ResponseMode.Query => registrationQueryRedirectUrl ?? DefaultQueryRedirectUrl,
            OAuth2ResponseMode.Fragment => registrationFragmentRedirectUrl ?? DefaultFragmentRedirectUrl,
            //OAuth2ResponseMode.FormPost => expr,
            _ => throw new ArgumentOutOfRangeException(nameof(responseMode), responseMode, null)
        };

    public void Update(
        string? defaultFragmentRedirectUrl,
        string? defaultQueryRedirectUrl,
        Guid? defaultObSealCertificateId,
        Guid? defaultObWacCertificateId,
        DateTimeOffset utcNow)
    {
        if (defaultFragmentRedirectUrl is not null)
        {
            DefaultFragmentRedirectUrl = defaultFragmentRedirectUrl;
        }
        if (defaultQueryRedirectUrl is not null)
        {
            DefaultQueryRedirectUrl = defaultQueryRedirectUrl;
        }
        if (defaultObSealCertificateId is not null)
        {
            DefaultObSealCertificateId = defaultObSealCertificateId.Value;
        }
        if (defaultObWacCertificateId is not null)
        {
            DefaultObWacCertificateId = defaultObWacCertificateId.Value;
        }
        Modified = utcNow;
    }
}

internal partial class SoftwareStatementEntity :
    ISupportsFluentLocalEntityGet<SoftwareStatementResponse>
{
    public SoftwareStatementResponse PublicGetLocalResponse => new()
    {
        Id = Id,
        Created = Created,
        CreatedBy = CreatedBy,
        Reference = Reference,
        OrganisationId = OrganisationId,
        SoftwareId = SoftwareId,
        SandboxEnvironment = SandboxEnvironment,
        DefaultObWacCertificateId = DefaultObWacCertificateId,
        DefaultObSealCertificateId = DefaultObSealCertificateId,
        DefaultQueryRedirectUrl = DefaultQueryRedirectUrl,
        DefaultFragmentRedirectUrl = DefaultFragmentRedirectUrl
    };
}
