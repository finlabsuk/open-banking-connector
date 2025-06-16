// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;

public interface IBankRegistrationPublicQuery : IEntityBaseQuery
{
    /// <summary>
    ///     Default OAuth2 response_mode override.
    /// </summary>
    OAuth2ResponseMode? DefaultResponseModeOverride { get; }

    /// <summary>
    ///     Token endpoint authorisation method
    /// </summary>
    TokenEndpointAuthMethodSupportedValues TokenEndpointAuthMethod { get; }

    // <summary>
    //     ID of software statement to use for registration. The ID must
    //     correspond to a previously-added software statement.
    // </summary>
    //Guid? SoftwareStatementId { get; }

    /// <summary>
    ///     Use simulated bank (only supported for some bank profiles).
    /// </summary>
    public bool UseSimulatedBank { get; }

    public string ExternalApiId { get; }

    /// <summary>
    ///     Bank profile to use that specifies configuration for bank (OIDC Issuer).
    /// </summary>
    public BankProfileEnum BankProfile { get; }

    /// <summary>
    ///     Bank group
    /// </summary>
    public BankGroup BankGroup { get; }

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

    /// <summary>
    ///     Functional APIs used for bank registration.
    /// </summary>
    RegistrationScopeEnum RegistrationScope { get; }

    /// <summary>
    ///     Use v4 external (bank) API for AISP.
    /// </summary>
    public bool AispUseV4 { get; }

    /// <summary>
    ///     Use v4 external (bank) API for new PISP consent creation.
    /// </summary>
    public bool PispUseV4 { get; }

    /// <summary>
    ///     Use v4 external (bank) API for new VRP consent creation.
    /// </summary>
    public bool VrpUseV4 { get; }
}

/// <summary>
///     Response to BankRegistration read and create requests.
/// </summary>
public class BankRegistrationResponse : EntityBaseResponse, IBankRegistrationPublicQuery
{
    public ClientRegistrationModelsPublic.OBClientRegistration1Response? ExternalApiResponse { get; init; }

    /// <summary>
    ///     ID of software statement to use for registration. The ID must
    ///     correspond to a previously-added software statement.
    /// </summary>
    public required Guid SoftwareStatementId { get; init; }

    /// <summary>
    ///     Default OAuth2 response_mode override.
    /// </summary>
    public required OAuth2ResponseMode? DefaultResponseModeOverride { get; init; }

    /// <summary>
    ///     Token endpoint authorisation method
    /// </summary>
    public required TokenEndpointAuthMethodSupportedValues TokenEndpointAuthMethod { get; init; }

    /// <summary>
    ///     Bank profile to use that specifies configuration for bank (OIDC Issuer).
    /// </summary>
    public required BankProfileEnum BankProfile { get; init; }

    /// <summary>
    ///     Bank group
    /// </summary>
    public required BankGroup BankGroup { get; init; }

    /// <summary>
    ///     JWK Set URI (normally supplied from OpenID Configuration)
    /// </summary>
    public required string JwksUri { get; init; }

    /// <summary>
    ///     Registration endpoint (normally supplied from OpenID Configuration)
    /// </summary>
    public required string? RegistrationEndpoint { get; init; }

    /// <summary>
    ///     Token endpoint (normally supplied from OpenID Configuration)
    /// </summary>
    public required string TokenEndpoint { get; init; }

    /// <summary>
    ///     Authorization endpoint (normally supplied from OpenID Configuration)
    /// </summary>
    public required string AuthorizationEndpoint { get; init; }

    /// <summary>
    ///     Functional APIs used for bank registration.
    /// </summary>
    public required RegistrationScopeEnum RegistrationScope { get; init; }

    /// <summary>
    ///     Default fragment redirect URI to use for this registration. This URI must
    ///     be included in the redirect URIs used for this registration (these are specified by RedirectUris and if that is
    ///     null default to those specified in the software statement in software statement profile
    ///     SoftwareStatementProfileId).
    ///     If null, the default fragment redirect URI specified in the software statement profile
    ///     will be used.
    /// </summary>
    public required string DefaultFragmentRedirectUri { get; init; }

    public required string DefaultQueryRedirectUri { get; init; }

    /// <summary>
    ///     Redirect URIs to use for this registration. Must be a subset of those specified in
    ///     the software statement in software statement profile SoftwareStatementProfileId.
    ///     If null, redirect URIs specified in the software statement will be used.
    /// </summary>
    public required IList<string> RedirectUris { get; init; }

    public required string ExternalApiId { get; init; }

    /// <summary>
    ///     Use simulated bank (only supported for some bank profiles).
    /// </summary>
    public required bool UseSimulatedBank { get; init; }

    /// <summary>
    ///     Use v4 external (bank) API for AISP.
    /// </summary>
    public required bool AispUseV4 { get; init; }

    /// <summary>
    ///     Use v4 external (bank) API for new PISP consent creation.
    /// </summary>
    public required bool PispUseV4 { get; init; }

    /// <summary>
    ///     Use v4 external (bank) API for new VRP consent creation.
    /// </summary>
    public required bool VrpUseV4 { get; init; }
}
