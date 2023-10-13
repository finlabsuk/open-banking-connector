// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
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
        TokenEndpointAuthMethod tokenEndpointAuthMethod,
        OAuth2ResponseMode defaultResponseMode,
        BankGroupEnum bankGroup,
        bool useSimulatedBank,
        BankProfileEnum bankProfile,
        string jwksUri,
        string? registrationEndpoint,
        string tokenEndpoint,
        string authorizationEndpoint,
        BankRegistrationGroup? bankRegistrationGroup,
        string defaultFragmentRedirectUri,
        IList<string> otherRedirectUris,
        string softwareStatementProfileId,
        string? softwareStatementProfileOverride,
        RegistrationScopeEnum registrationScope) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        _externalApiId = externalApiId ?? throw new ArgumentNullException(nameof(externalApiId));
        _externalApiSecret = externalApiSecret;
        _registrationAccessToken = registrationAccessToken;
        TokenEndpointAuthMethod = tokenEndpointAuthMethod;
        DefaultResponseMode = defaultResponseMode;
        BankGroup = bankGroup;
        UseSimulatedBank = useSimulatedBank;
        BankProfile = bankProfile;
        JwksUri = jwksUri ?? throw new ArgumentNullException(nameof(jwksUri));
        RegistrationEndpoint = registrationEndpoint;
        TokenEndpoint = tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint));
        AuthorizationEndpoint = authorizationEndpoint ?? throw new ArgumentNullException(nameof(authorizationEndpoint));
        BankRegistrationGroup = bankRegistrationGroup;
        DefaultFragmentRedirectUri = defaultFragmentRedirectUri ??
                                     throw new ArgumentNullException(nameof(defaultFragmentRedirectUri));
        OtherRedirectUris = otherRedirectUris ?? throw new ArgumentNullException(nameof(otherRedirectUris));
        SoftwareStatementProfileId = softwareStatementProfileId ??
                                     throw new ArgumentNullException(nameof(softwareStatementProfileId));
        SoftwareStatementProfileOverride = softwareStatementProfileOverride;
        RegistrationScope = registrationScope;
    }

    public ExternalApiObject ExternalApiObject => new(
        _externalApiId,
        _externalApiSecret,
        _registrationAccessToken);

    /// <summary>
    ///     Token endpoint authorisation method
    /// </summary>
    public TokenEndpointAuthMethod TokenEndpointAuthMethod { get; }

    public OAuth2ResponseMode DefaultResponseMode { get; }

    /// <summary>
    ///     Bank group
    /// </summary>
    public BankGroupEnum BankGroup { get; set; }

    /// <summary>
    ///     Use simulated bank.
    /// </summary>
    public bool UseSimulatedBank { get; }

    /// <summary>
    ///     Bank profile to use that specifies configuration for bank (OIDC Issuer).
    /// </summary>
    public BankProfileEnum BankProfile { get; set; }

    /// <summary>
    ///     JWK Set URI (normally supplied from OpenID Configuration)
    /// </summary>
    public string JwksUri { get; }

    /// <summary>
    ///     Registration endpoint (normally supplied from OpenID Configuration)
    /// </summary>
    public string? RegistrationEndpoint { get; }

    /// <summary>
    ///     Token endpoint (normally supplied from OpenID Configuration)
    /// </summary>
    public string TokenEndpoint { get; }

    /// <summary>
    ///     Authorization endpoint (normally supplied from OpenID Configuration)
    /// </summary>
    public string AuthorizationEndpoint { get; }

    /// <summary>
    ///     Bank registration group. The same external API registration object is
    ///     re-used by all members of a group.
    /// </summary>
    public BankRegistrationGroup? BankRegistrationGroup { get; }

    /// <summary>
    ///     Default fragment redirect URI used for this registration.
    /// </summary>
    public string DefaultFragmentRedirectUri { get; set; }

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
    ///     Functional APIs used for bank registration.
    /// </summary>
    public RegistrationScopeEnum RegistrationScope { get; }

    IBankRegistrationExternalApiObjectPublicQuery IBankRegistrationPublicQuery.ExternalApiObject =>
        ExternalApiObject;
}
