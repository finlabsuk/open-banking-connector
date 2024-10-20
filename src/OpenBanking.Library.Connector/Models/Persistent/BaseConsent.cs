// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;

public class ConsentAccessToken
{
    public ConsentAccessToken(
        string? token,
        int expiresIn,
        string? refreshToken,
        DateTimeOffset modified,
        string? modifiedBy)
    {
        Token = token;
        ExpiresIn = expiresIn;
        RefreshToken = refreshToken;
        Modified = modified;
        ModifiedBy = modifiedBy;
    }

    public string? Token { get; }

    public int ExpiresIn { get; }

    public string? RefreshToken { get; }

    public DateTimeOffset Modified { get; }

    public string? ModifiedBy { get; }
}

/// <summary>
///     Persisted type.
///     Internal to help ensure public request and response types used on public API.
/// </summary>
internal abstract class BaseConsent : BaseEntity, IConsentPublicQuery
{
    /// <summary>
    ///     Access token including "access_token" (value1) and "expires_in" (value2) fields. If value2 is null, indicates auth
    ///     not successfully completed.
    /// </summary>
    [Column("access_token_access_token")]
    private string? _accessTokenAccessToken;

    [Column("access_token_expires_in")]
    private int _accessTokenExpiresIn;

    [Column("access_token_modified")]
    private DateTimeOffset _accessTokenModified;

    [Column("access_token_modified_by")]
    private string? _accessTokenModifiedBy;

    /// <summary>
    ///     Refresh token. If null, indicates no refresh token received.
    /// </summary>
    [Column("access_token_refresh_token")]
    private string? _accessTokenRefreshToken;


    protected BaseConsent(
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
        createdBy)
    {
        _accessTokenAccessToken = accessTokenAccessToken;
        _accessTokenExpiresIn = accessTokenExpiresIn;
        _accessTokenModified = accessTokenModified;
        _accessTokenModifiedBy = accessTokenModifiedBy;
        _accessTokenRefreshToken = accessTokenRefreshToken;
        AuthContextState = authContextState;
        AuthContextNonce = authContextNonce;
        AuthContextCodeVerifier = authContextCodeVerifier;
        AuthContextModified = authContextModified;
        AuthContextModifiedBy = authContextModifiedBy;
        ExternalApiUserId = externalApiUserId;
        ExternalApiUserIdModified = externalApiUserIdModified;
        ExternalApiUserIdModifiedBy = externalApiUserIdModifiedBy;
        BankRegistrationId = bankRegistrationId;
        ExternalApiId = externalApiId ?? throw new ArgumentNullException(nameof(externalApiId));
    }

    [ForeignKey(nameof(BankRegistrationId))]
    public BankRegistrationEntity BankRegistrationNavigation { get; private set; } = null!;

    /// <summary>
    ///     OAuth2 "state". This is mutable as re-auth will generate a new value.
    /// </summary>
    public string? AuthContextState { get; private set; }

    /// <summary>
    ///     OpenID Connect "nonce". This is mutable as re-auth will generate a new value.
    /// </summary>
    public string? AuthContextNonce { get; private set; }

    public string? AuthContextCodeVerifier { get; private set; }

    public DateTimeOffset AuthContextModified { get; private set; }

    public string? AuthContextModifiedBy { get; private set; }

    /// <summary>
    ///     User ID at external API (bank) which may or may not be available via ID token "sub" claim. If retrieved from ID
    ///     token or supplied on object creation, it will be stored here.
    /// </summary>
    public string? ExternalApiUserId { get; private set; }

    public DateTimeOffset ExternalApiUserIdModified { get; private set; }

    public string? ExternalApiUserIdModifiedBy { get; private set; }

    public ConsentAccessToken ConsentAccessToken => new(
        _accessTokenAccessToken,
        _accessTokenExpiresIn,
        _accessTokenRefreshToken,
        _accessTokenModified,
        _accessTokenModifiedBy);

    /// <summary>
    ///     Associated BankRegistration object
    /// </summary>
    public Guid BankRegistrationId { get; }

    /// <summary>
    ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
    ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
    /// </summary>
    public string ExternalApiId { get; }

    /// <summary>
    ///     Returns true when it is known that auth was previously performed successfully.
    /// </summary>
    /// <returns></returns>
    public bool AuthPreviouslySucceessfullyPerformed() => AuthContextState is not null;

    public void UpdateAccessToken(
        string? accessTokenValue,
        int accessTokenExpiresIn,
        string? accessTokenRefreshToken,
        DateTimeOffset modified,
        string? modifiedBy)

    {
        _accessTokenAccessToken = accessTokenValue;
        _accessTokenExpiresIn = accessTokenExpiresIn;
        _accessTokenRefreshToken = accessTokenRefreshToken;
        _accessTokenModified = modified;
        _accessTokenModifiedBy = modifiedBy;
    }

    public void UpdateAuthContext(
        string state,
        string nonce,
        string? codeVerifier,
        DateTimeOffset modified,
        string? modifiedBy)
    {
        AuthContextState = state;
        AuthContextNonce = nonce;
        AuthContextCodeVerifier = codeVerifier;
        AuthContextModified = modified;
        AuthContextModifiedBy = modifiedBy;
    }

    protected abstract string GetConsentTypeString();

    public abstract ConsentType GetConsentType();

    public string GetCacheKey() => string.Join(":", "token", GetConsentTypeString(), Id.ToString());

    public string GetAssociatedData(BankRegistrationEntity bankRegistration) => string.Join(
        ":",
        GetConsentTypeString(),
        Id.ToString(),
        ExternalApiId,
        BankRegistrationId.ToString(),
        bankRegistration.ExternalApiId,
        bankRegistration.BankProfile.ToString());

    public abstract AccessTokenEntity AddNewAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy);

    public abstract RefreshTokenEntity AddNewRefreshToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy);

    public void UpdateExternalApiUserId(
        string? externalApiUserId,
        DateTimeOffset externalApiUserIdModified,
        string? externalApiUserIdModifiedBy)
    {
        ExternalApiUserId = externalApiUserId;
        ExternalApiUserIdModified = externalApiUserIdModified;
        ExternalApiUserIdModifiedBy = externalApiUserIdModifiedBy;
    }
}
