﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;

public interface IBankRegistrationExternalApiObjectPublicQuery
{
    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    string ExternalApiId { get; }
}

public class ExternalApiObjectResponse : IBankRegistrationExternalApiObjectPublicQuery
{
    public ExternalApiObjectResponse(string externalApiId)
    {
        ExternalApiId = externalApiId;
    }

    public string ExternalApiId { get; }
}

public interface IBankRegistrationPublicQuery : IBaseQuery
{
    /// <summary>
    ///     ID of SoftwareStatementProfile to use in association with BankRegistration
    /// </summary>
    string SoftwareStatementProfileId { get; }

    string? SoftwareStatementProfileOverride { get; }

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
    ///     Default redirect URI to use for this registration. This redirect URI must
    ///     be included in the software statement in software statement profile SoftwareStatementProfileId.
    /// </summary>
    public string DefaultRedirectUri { get; }

    /// <summary>
    ///     Other redirect URIs in addition to default one to use for this registration.
    ///     Each redirect URI must
    ///     be included in the software statement in software statement profile SoftwareStatementProfileId.
    /// </summary>
    public IList<string> OtherRedirectUris { get; }

    IBankRegistrationExternalApiObjectPublicQuery ExternalApiObject { get; }

    /// <summary>
    ///     Bank registration group. The same external API registration object is
    ///     re-used by all members of a group.
    /// </summary>
    public BankRegistrationGroup? BankRegistrationGroup { get; }
}

/// <summary>
///     Response to BankRegistration Read and Create requests.
/// </summary>
public class BankRegistrationResponse : LocalObjectBaseResponse, IBankRegistrationPublicQuery
{
    internal BankRegistrationResponse(
        Guid id,
        DateTimeOffset created,
        string? createdBy,
        string? reference,
        ExternalApiObjectResponse externalApiObject,
        ClientRegistrationModelsPublic.OBClientRegistration1Response? externalApiResponse,
        IList<string>? warnings,
        BankProfileEnum bankProfile,
        string jwksUri,
        string? registrationEndpoint,
        string tokenEndpoint,
        string authorizationEndpoint,
        string softwareStatementProfileId,
        string? softwareStatementProfileOverride,
        RegistrationScopeEnum registrationScope,
        string defaultRedirectUri,
        IList<string> otherRedirectUris,
        BankRegistrationGroup? bankRegistrationGroup) : base(id, created, createdBy, reference)
    {
        ExternalApiObject = externalApiObject;
        ExternalApiResponse = externalApiResponse;
        Warnings = warnings;
        BankProfile = bankProfile;
        JwksUri = jwksUri;
        RegistrationEndpoint = registrationEndpoint;
        TokenEndpoint = tokenEndpoint;
        AuthorizationEndpoint = authorizationEndpoint;
        SoftwareStatementProfileId = softwareStatementProfileId;
        SoftwareStatementProfileOverride = softwareStatementProfileOverride;
        RegistrationScope = registrationScope;
        DefaultRedirectUri = defaultRedirectUri;
        OtherRedirectUris = otherRedirectUris;
        BankRegistrationGroup = bankRegistrationGroup;
    }

    public ExternalApiObjectResponse ExternalApiObject { get; }

    public ClientRegistrationModelsPublic.OBClientRegistration1Response? ExternalApiResponse { get; }

    /// <summary>
    ///     Optional list of warning messages from Open Banking Connector.
    /// </summary>
    public IList<string>? Warnings { get; }

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
    ///     ID of SoftwareStatementProfile to use in association with BankRegistration
    /// </summary>
    public string SoftwareStatementProfileId { get; }

    public string? SoftwareStatementProfileOverride { get; }

    /// <summary>
    ///     Functional APIs used for bank registration.
    /// </summary>
    public RegistrationScopeEnum RegistrationScope { get; }

    /// <summary>
    ///     Default redirect URI to use for this registration. This redirect URI must
    ///     be included in the software statement in software statement profile SoftwareStatementProfileId.
    /// </summary>
    public string DefaultRedirectUri { get; }

    /// <summary>
    ///     Other redirect URIs in addition to default one to use for this registration.
    ///     Each redirect URI must
    ///     be included in the software statement in software statement profile SoftwareStatementProfileId.
    /// </summary>
    public IList<string> OtherRedirectUris { get; }

    /// <summary>
    ///     Bank registration group. The same external API registration object is
    ///     re-used by all members of a group.
    /// </summary>
    public BankRegistrationGroup? BankRegistrationGroup { get; }

    IBankRegistrationExternalApiObjectPublicQuery IBankRegistrationPublicQuery.ExternalApiObject =>
        ExternalApiObject;
}
