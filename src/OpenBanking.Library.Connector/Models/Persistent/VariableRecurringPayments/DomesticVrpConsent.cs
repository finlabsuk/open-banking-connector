// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class DomesticVrpConsent :
    BaseConsent
{
    public DomesticVrpConsent(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        string? accessTokenAccessToken,
        int accessTokenExpiresIn,
        DateTimeOffset accessTokenModified,
        string? accessTokenModifiedBy,
        string? accessTokenRefreshToken,
        string? authContextState,
        string? authContextNonce,
        string? authContextCodeVerifier,
        DateTimeOffset authContextModified,
        string? authContextModifiedBy,
        string? externalApiUserId,
        DateTimeOffset externalApiUserIdModified,
        string? externalApiUserIdModifiedBy,
        Guid bankRegistrationId,
        string externalApiId) : base(
        id,
        reference,
        isDeleted,
        isDeletedModified,
        isDeletedModifiedBy,
        created,
        createdBy,
        accessTokenAccessToken,
        accessTokenExpiresIn,
        accessTokenModified,
        accessTokenModifiedBy,
        accessTokenRefreshToken,
        authContextState,
        authContextNonce,
        authContextCodeVerifier,
        authContextModified,
        authContextModifiedBy,
        externalApiUserId,
        externalApiUserIdModified,
        externalApiUserIdModifiedBy,
        bankRegistrationId,
        externalApiId) { }

    /// <summary>
    ///     Associated access tokens
    /// </summary>
    public IList<DomesticVrpConsentAccessToken> DomesticVrpConsentAccessTokensNavigation { get; } =
        new List<DomesticVrpConsentAccessToken>();

    /// <summary>
    ///     Associated refresh tokens
    /// </summary>
    public IList<DomesticVrpConsentRefreshToken> DomesticVrpConsentRefreshTokensNavigation { get; } =
        new List<DomesticVrpConsentRefreshToken>();

    protected override string GetConsentTypeString() => "vrp_dom";

    public override ConsentType GetConsentType() => ConsentType.DomesticVrpConsent;

    public override AccessTokenEntity AddNewAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy)
    {
        var domesticVrpConsentAccessToken =
            new DomesticVrpConsentAccessToken(
                id,
                reference,
                isDeleted,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                Id);
        DomesticVrpConsentAccessTokensNavigation.Add(domesticVrpConsentAccessToken);
        return domesticVrpConsentAccessToken;
    }

    public override RefreshTokenEntity AddNewRefreshToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy)
    {
        var domesticVrpConsentRefreshToken =
            new DomesticVrpConsentRefreshToken(
                Guid.NewGuid(),
                null,
                false,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                Id);
        DomesticVrpConsentRefreshTokensNavigation.Add(domesticVrpConsentRefreshToken);
        return domesticVrpConsentRefreshToken;
    }
}
