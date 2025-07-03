// Licensed to Finnovation Labs Limited under one or more agreements.
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
        string? createdBy) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy) { }

    public ObWacCertificateEntity DefaultObWacCertificateNavigation { get; } = null!;

    public ObSealCertificateEntity DefaultObSealCertificateNavigation { get; } = null!;

    public required DateTimeOffset Modified { get; set; }

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
    ///     ID of default <see cref="ObWacCertificateEntity" /> to use for mutual TLS with this software statement.
    /// </summary>
    public required Guid DefaultObWacCertificateId { get; set; }

    /// <summary>
    ///     ID of default <see cref="ObSealCertificateEntity" /> to use for signing JWTs etc with this software statement.
    /// </summary>
    public required Guid DefaultObSealCertificateId { get; set; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = query.
    /// </summary>
    public required string DefaultQueryRedirectUrl { get; set; }

    /// <summary>
    ///     Default redirect URL for consent authorisation when OAuth2 response_mode = fragment.
    /// </summary>
    public required string DefaultFragmentRedirectUrl { get; set; }

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
