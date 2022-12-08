// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;

internal class ExternalApiObject : IBankRegistrationExternalApiObjectPublicQuery
{
    public ExternalApiObject(string externalApiId, string? externalApiSecret, string? registrationAccessToken)
    {
        ExternalApiId = externalApiId;
        ExternalApiSecret = externalApiSecret;
        RegistrationAccessToken = registrationAccessToken;
    }

    public string? ExternalApiSecret { get; }

    public string? RegistrationAccessToken { get; }

    public string ExternalApiId { get; }
}

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class BankRegistration :
    BaseEntity,
    IBankRegistrationPublicQuery
{
    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    [Column("external_api_id")]
    private readonly string _externalApiId;

    /// <summary>
    ///     External API secret. Present to allow use of legacy token auth method "client_secret_basic" in sandboxes etc.
    /// </summary>
    [Column("external_api_secret")]
    private readonly string? _externalApiSecret;

    /// <summary>
    ///     External API registration access token. Sometimes used to support registration adjustments etc.
    /// </summary>
    [Column("registration_access_token")]
    private readonly string? _registrationAccessToken;

    public BankRegistration(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string externalApiId,
        string? externalApiSecret,
        string? registrationAccessToken,
        string defaultRedirectUri,
        IList<string> otherRedirectUris,
        string softwareStatementProfileId,
        string? softwareStatementProfileOverride,
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        RegistrationScopeEnum registrationScope,
        OAuth2ResponseMode defaultResponseMode,
        Guid bankId) : base(id, reference, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
    {
        _externalApiId = externalApiId;
        _externalApiSecret = externalApiSecret;
        _registrationAccessToken = registrationAccessToken;
        DefaultRedirectUri = defaultRedirectUri;
        OtherRedirectUris = otherRedirectUris;
        SoftwareStatementProfileId = softwareStatementProfileId;
        SoftwareStatementProfileOverride = softwareStatementProfileOverride;
        TokenEndpointAuthMethod = tokenEndpointAuthMethod;
        RegistrationScope = registrationScope;
        DefaultResponseMode = defaultResponseMode;
        BankId = bankId;
    }

    [ForeignKey("BankId")]
    public Bank BankNavigation { get; set; } = null!;

    public ExternalApiObject ExternalApiObject => new(
        _externalApiId,
        _externalApiSecret,
        _registrationAccessToken);

    /// <summary>
    ///     Default redirect URI used for this registration.
    /// </summary>
    public string DefaultRedirectUri { get; set; }

    /// <summary>
    ///     Redirect URIs in addition to default one used for this registration.
    /// </summary>
    public IList<string> OtherRedirectUris { get; set; }

    /// <summary>
    ///     ID of SoftwareStatementProfile to use in association with BankRegistration
    /// </summary>
    public string SoftwareStatementProfileId { get; }

    public string? SoftwareStatementProfileOverride { get; }

    /// <summary>
    ///     Token endpoint authorisation method
    /// </summary>
    public TokenEndpointAuthMethod TokenEndpointAuthMethod { get; }

    /// <summary>
    ///     Functional APIs used for bank registration.
    /// </summary>
    public RegistrationScopeEnum RegistrationScope { get; }

    public OAuth2ResponseMode DefaultResponseMode { get; }

    /// <summary>
    ///     Bank with which this BankRegistration is associated.
    /// </summary>
    public Guid BankId { get; }

    IBankRegistrationExternalApiObjectPublicQuery IBankRegistrationPublicQuery.ExternalApiObject =>
        ExternalApiObject;
}