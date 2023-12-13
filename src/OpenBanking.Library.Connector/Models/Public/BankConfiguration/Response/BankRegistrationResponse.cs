// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;

public interface IBankRegistrationPublicQuery : IBaseQuery
{
    // <summary>
    //     ID of software statement to use for registration. The ID must
    //     correspond to a previously-added software statement.
    // </summary>
    //public Guid SoftwareStatementId { get; }

    /// <summary>
    ///     Functional APIs used for bank registration.
    /// </summary>
    RegistrationScopeEnum RegistrationScope { get; }

    /// <summary>
    ///     Bank profile to use that specifies configuration for bank (OIDC Issuer).
    /// </summary>
    public BankProfileEnum BankProfile { get; }

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
    ///     Default fragment redirect URI to use for this registration. This URI must
    ///     be included in the redirect URIs used for this registration (these are specified by RedirectUris and if that is
    ///     null default to those specified in the software statement in software statement profile
    ///     SoftwareStatementProfileId).
    ///     If null, the default fragment redirect URI specified in the software statement profile
    ///     will be used.
    /// </summary>
    public string DefaultFragmentRedirectUri { get; }

    public string DefaultQueryRedirectUri { get; }

    /// <summary>
    ///     Redirect URIs to use for this registration. Must be a subset of those specified in
    ///     the software statement in software statement profile SoftwareStatementProfileId.
    ///     If null, redirect URIs specified in the software statement will be used.
    /// </summary>
    public IList<string> RedirectUris { get; }

    public string ExternalApiId { get; }

    /// <summary>
    ///     Use simulated bank (only supported for some bank profiles).
    /// </summary>
    public bool UseSimulatedBank { get; }
}

/// <summary>
///     Response to BankRegistration read and create requests.
/// </summary>
public class BankRegistrationResponse : LocalObjectBaseResponse, IBankRegistrationPublicQuery
{
    internal BankRegistrationResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse,
        IList<string>? warnings,
        Guid softwareStatementId,
        BankProfileEnum bankProfile,
        string jwksUri,
        string? registrationEndpoint,
        string tokenEndpoint,
        string authorizationEndpoint,
        RegistrationScopeEnum registrationScope,
        string defaultFragmentRedirectUri,
        string defaultQueryRedirectUri,
        IList<string> redirectUris,
        string externalApiId,
        bool useSimulatedBank) : base(id, created, createdBy, reference)
    {
        ExternalApiResponse = externalApiResponse;
        Warnings = warnings;
        SoftwareStatementId = softwareStatementId;
        BankProfile = bankProfile;
        JwksUri = jwksUri ?? throw new ArgumentNullException(nameof(jwksUri));
        RegistrationEndpoint = registrationEndpoint;
        TokenEndpoint = tokenEndpoint ?? throw new ArgumentNullException(nameof(tokenEndpoint));
        AuthorizationEndpoint = authorizationEndpoint ?? throw new ArgumentNullException(nameof(authorizationEndpoint));
        RegistrationScope = registrationScope;
        DefaultFragmentRedirectUri = defaultFragmentRedirectUri ??
                                     throw new ArgumentNullException(nameof(defaultFragmentRedirectUri));
        DefaultQueryRedirectUri =
            defaultQueryRedirectUri ?? throw new ArgumentNullException(nameof(defaultQueryRedirectUri));
        RedirectUris = redirectUris ?? throw new ArgumentNullException(nameof(redirectUris));
        ExternalApiId = externalApiId ?? throw new ArgumentNullException(nameof(externalApiId));
        UseSimulatedBank = useSimulatedBank;
    }

    public ClientRegistrationModelsPublic.OBClientRegistration1Response? ExternalApiResponse { get; }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; }

    /// <summary>
    ///     ID of software statement to use for registration. The ID must
    ///     correspond to a previously-added software statement.
    /// </summary>
    public Guid SoftwareStatementId { get; }

    /// <summary>
    ///     Bank profile to use that specifies configuration for bank (OIDC Issuer).
    /// </summary>
    public BankProfileEnum BankProfile { get; }

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
    ///     Functional APIs used for bank registration.
    /// </summary>
    public RegistrationScopeEnum RegistrationScope { get; }

    /// <summary>
    ///     Default fragment redirect URI to use for this registration. This URI must
    ///     be included in the redirect URIs used for this registration (these are specified by RedirectUris and if that is
    ///     null default to those specified in the software statement in software statement profile
    ///     SoftwareStatementProfileId).
    ///     If null, the default fragment redirect URI specified in the software statement profile
    ///     will be used.
    /// </summary>
    public string DefaultFragmentRedirectUri { get; }

    public string DefaultQueryRedirectUri { get; }

    /// <summary>
    ///     Redirect URIs to use for this registration. Must be a subset of those specified in
    ///     the software statement in software statement profile SoftwareStatementProfileId.
    ///     If null, redirect URIs specified in the software statement will be used.
    /// </summary>
    public IList<string> RedirectUris { get; }

    public string ExternalApiId { get; }

    /// <summary>
    ///     Use simulated bank (only supported for some bank profiles).
    /// </summary>
    public bool UseSimulatedBank { get; }
}
