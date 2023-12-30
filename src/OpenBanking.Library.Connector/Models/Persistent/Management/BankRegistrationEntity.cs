// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class BankRegistrationEntity :
    BaseEntity,
    IBankRegistrationPublicQuery
{
    public BankRegistrationEntity(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string? externalApiSecret,
        string? registrationAccessToken,
        OAuth2ResponseMode? defaultResponseModeOverride,
        TokenEndpointAuthMethodSupportedValues tokenEndpointAuthMethod,
        BankGroupEnum bankGroup,
        Guid? softwareStatementId,
        string softwareStatementProfileId,
        string? softwareStatementProfileOverride,
        bool useSimulatedBank,
        string externalApiId,
        BankProfileEnum bankProfile,
        string jwksUri,
        string? registrationEndpoint,
        string tokenEndpoint,
        string authorizationEndpoint,
        string defaultFragmentRedirectUri,
        string defaultQueryRedirectUri,
        IList<string> redirectUris,
        RegistrationScopeEnum registrationScope) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        ExternalApiSecret = externalApiSecret;
        RegistrationAccessToken = registrationAccessToken;
        DefaultResponseModeOverride = defaultResponseModeOverride;
        TokenEndpointAuthMethod = tokenEndpointAuthMethod;
        BankGroup = bankGroup;
        SoftwareStatementId = softwareStatementId;
        SoftwareStatementProfileId = softwareStatementProfileId ??
                                     throw new ArgumentNullException(nameof(softwareStatementProfileId));
        SoftwareStatementProfileOverride = softwareStatementProfileOverride;
        UseSimulatedBank = useSimulatedBank;
        ExternalApiId = externalApiId ?? throw new ArgumentNullException(nameof(externalApiId));
        BankProfile = bankProfile;
        JwksUri = jwksUri ?? throw new ArgumentNullException(nameof(jwksUri));
        RegistrationEndpoint = registrationEndpoint;
        TokenEndpoint = tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint));
        AuthorizationEndpoint = authorizationEndpoint ?? throw new ArgumentNullException(nameof(authorizationEndpoint));
        DefaultFragmentRedirectUri = defaultFragmentRedirectUri ??
                                     throw new ArgumentNullException(nameof(defaultFragmentRedirectUri));
        DefaultQueryRedirectUri =
            defaultQueryRedirectUri ?? throw new ArgumentNullException(nameof(defaultQueryRedirectUri));
        RedirectUris = redirectUris ?? throw new ArgumentNullException(nameof(redirectUris));
        RegistrationScope = registrationScope;
    }

    /// <summary>
    ///     External API secret. Present to allow use of legacy token auth method "client_secret_basic" in sandboxes etc.
    /// </summary>
    public string? ExternalApiSecret { get; }

    /// <summary>
    ///     External API registration access token. Sometimes used to support registration adjustments etc.
    /// </summary>
    public string? RegistrationAccessToken { get; }

    /// <summary>
    ///     Default OAuth2 response_mode override.
    /// </summary>
    public OAuth2ResponseMode? DefaultResponseModeOverride { get; }

    /// <summary>
    ///     Token endpoint authorisation method
    /// </summary>
    public TokenEndpointAuthMethodSupportedValues TokenEndpointAuthMethod { get; }

    /// <summary>
    ///     Bank group
    /// </summary>
    public BankGroupEnum BankGroup { get; set; }

    [ForeignKey(nameof(SoftwareStatementId))]
    public SoftwareStatementEntity? SoftwareStatementNavigation { get; private set; }

    public Guid? SoftwareStatementId { get; set; }

    /// <summary>
    ///     ID of SoftwareStatementProfile to use in association with BankRegistration
    /// </summary>
    public string SoftwareStatementProfileId { get; }

    public string? SoftwareStatementProfileOverride { get; }

    /// <summary>
    ///     Use simulated bank.
    /// </summary>
    public bool UseSimulatedBank { get; }

    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; }

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
    ///     Default fragment redirect URI used for this registration.
    /// </summary>
    public string DefaultFragmentRedirectUri { get; set; }

    /// <summary>
    ///     Default query redirect URI used for this registration.
    /// </summary>
    public string DefaultQueryRedirectUri { get; set; }

    /// <summary>
    ///     Redirect URIs used for registration.
    /// </summary>
    public IList<string> RedirectUris { get; set; }

    /// <summary>
    ///     Functional APIs used for bank registration.
    /// </summary>
    public RegistrationScopeEnum RegistrationScope { get; }
}
