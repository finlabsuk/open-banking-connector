// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;

/// <summary>
///     Persisted type for Bank.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal partial class Bank :
    BaseEntity,
    IBankPublicQuery
{
    public Bank(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        IdTokenSubClaimType idTokenSubClaimType,
        string jwksUri,
        bool supportsSca,
        string issuerUrl,
        string financialId,
        string? registrationEndpoint,
        string tokenEndpoint,
        string authorizationEndpoint,
        DynamicClientRegistrationApiVersion dcrApiVersion,
        CustomBehaviourClass? customBehaviour) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy)
    {
        IdTokenSubClaimType = idTokenSubClaimType;
        JwksUri = jwksUri;
        SupportsSca = supportsSca;
        IssuerUrl = issuerUrl;
        FinancialId = financialId;
        RegistrationEndpoint = registrationEndpoint;
        TokenEndpoint = tokenEndpoint;
        AuthorizationEndpoint = authorizationEndpoint;
        DcrApiVersion = dcrApiVersion;
        CustomBehaviour = customBehaviour;
    }

    /// <summary>
    ///     ID token "sub" claim type. Determined by how bank uses ID token.
    /// </summary>
    public IdTokenSubClaimType IdTokenSubClaimType { get; }

    /// <summary>
    ///     JWK Set URI (normally supplied from OpenID Configuration)
    /// </summary>
    public string JwksUri { get; }

    public bool SupportsSca { get; }

    public string IssuerUrl { get; }

    public string FinancialId { get; }

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
    ///     API version used for DCR requests (POST, GET etc)
    /// </summary>
    public DynamicClientRegistrationApiVersion DcrApiVersion { get; }

    /// <summary>
    ///     Custom behaviour, usually bank-specific, to handle quirks, formatting issues, etc.
    ///     For a well-behaved bank, normally this object should be null.
    /// </summary>
    public CustomBehaviourClass? CustomBehaviour { get; set; }
}

internal partial class Bank :
    ISupportsFluentLocalEntityGet<BankResponse>
{
    public BankResponse PublicGetLocalResponse => new(
        Id,
        Created,
        CreatedBy,
        Reference,
        null,
        IdTokenSubClaimType,
        JwksUri,
        SupportsSca,
        IssuerUrl,
        FinancialId,
        RegistrationEndpoint,
        TokenEndpoint,
        AuthorizationEndpoint,
        DcrApiVersion,
        CustomBehaviour);
}
