// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    public class AccessToken
    {
        public AccessToken(string value, int expiresIn, string? refreshToken, DateTimeOffset modified)
        {
            Value = value;
            ExpiresIn = expiresIn;
            RefreshToken = refreshToken;
            Modified = modified;
        }

        public string Value { get; }

        public int ExpiresIn { get; }

        public string? RefreshToken { get; }

        public DateTimeOffset Modified { get; }
    }


    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class BaseConsent :
        BaseEntity
    {
        public BaseConsent(
            Guid id,
            string? name,
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string? accessToken_AccessToken,
            int accessToken_ExpiresIn,
            string? accessToken_RefreshToken,
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy) : base(
            id,
            name,
            reference,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy)
        {
            AccessToken_AccessToken = accessToken_AccessToken;
            AccessToken_ExpiresIn = accessToken_ExpiresIn;
            AccessToken_RefreshToken = accessToken_RefreshToken;
            AccessTokenModified = accessTokenModified;
            AccessTokenModifiedBy = accessTokenModifiedBy;
        }


        /// <summary>
        ///     Access token including "access_token" (value1) and "expires_in" (value2) fields. If value2 is null, indicates auth
        ///     not successfully completed.
        /// </summary>
        public string? AccessToken_AccessToken { get; private set; }

        public int AccessToken_ExpiresIn { get; private set; }

        /// <summary>
        ///     Refresh token. If null, indicates no refresh token received.
        /// </summary>
        public string? AccessToken_RefreshToken { get; private set; }

        public DateTimeOffset AccessTokenModified { get; private set; }

        public string? AccessTokenModifiedBy { get; private set; }

        public AccessToken? AccessToken => AccessToken_AccessToken switch
        {
            null => null,
            { } value =>
                new AccessToken(value, AccessToken_ExpiresIn, AccessToken_RefreshToken, AccessTokenModified)
        };

        public void UpdateAccessToken(
            string? accessTokenValue,
            int accessTokenExpiresIn,
            string? accessTokenRefreshToken,
            DateTimeOffset modified,
            string? modifiedBy)

        {
            AccessToken_AccessToken = accessTokenValue;
            AccessToken_ExpiresIn = accessTokenExpiresIn;
            AccessToken_RefreshToken = accessTokenRefreshToken;
            AccessTokenModified = modified;
            AccessTokenModifiedBy = modifiedBy;
        }
    }
}
