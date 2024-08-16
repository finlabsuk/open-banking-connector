// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal class DomesticPaymentConsent :
    BaseConsent
{
    public DomesticPaymentConsent(
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
    public IList<DomesticPaymentConsentAccessToken> DomesticPaymentConsentAccessTokensNavigation { get; } =
        new List<DomesticPaymentConsentAccessToken>();

    /// <summary>
    ///     Associated refresh tokens
    /// </summary>
    public IList<DomesticPaymentConsentRefreshToken>
        DomesticPaymentConsentRefreshTokensNavigation { get; } =
        new List<DomesticPaymentConsentRefreshToken>();

    protected override string GetConsentTypeString() => "pisp_dom";

    public override ConsentType GetConsentType() => ConsentType.DomesticPaymentConsent;

    public override AccessTokenEntity AddNewAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy)
    {
        var domesticPaymentConsentAccessToken =
            new DomesticPaymentConsentAccessToken(
                id,
                reference,
                isDeleted,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                Id);
        DomesticPaymentConsentAccessTokensNavigation.Add(domesticPaymentConsentAccessToken);
        return domesticPaymentConsentAccessToken;
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
        var domesticPaymentConsentRefreshToken =
            new DomesticPaymentConsentRefreshToken(
                Guid.NewGuid(),
                null,
                false,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                Id);
        DomesticPaymentConsentRefreshTokensNavigation.Add(domesticPaymentConsentRefreshToken);
        return domesticPaymentConsentRefreshToken;
    }
}
