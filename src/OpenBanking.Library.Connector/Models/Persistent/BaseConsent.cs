// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;

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
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string? accessTokenAccessToken,
            int accessTokenExpiresIn,
            string? accessTokenRefreshToken,
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy) : base(
            id,
            reference,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy)
        {
            AccessTokenAccessToken = accessTokenAccessToken;
            AccessTokenExpiresIn = accessTokenExpiresIn;
            AccessTokenRefreshToken = accessTokenRefreshToken;
            AccessTokenModified = accessTokenModified;
            AccessTokenModifiedBy = accessTokenModifiedBy;
        }

        /// <summary>
        ///     Access token including "access_token" (value1) and "expires_in" (value2) fields. If value2 is null, indicates auth
        ///     not successfully completed.
        /// </summary>
        [Column("access_token__access_token")]
        public string? AccessTokenAccessToken { get; private set; }

        [Column("access_token__expires_in")]
        public int AccessTokenExpiresIn { get; private set; }

        /// <summary>
        ///     Refresh token. If null, indicates no refresh token received.
        /// </summary>
        [Column("access_token__refresh_token")]
        public string? AccessTokenRefreshToken { get; private set; }

        public DateTimeOffset AccessTokenModified { get; private set; }

        public string? AccessTokenModifiedBy { get; private set; }

        public AccessToken? AccessToken => AccessTokenAccessToken switch
        {
            null => null,
            { } value =>
                new AccessToken(value, AccessTokenExpiresIn, AccessTokenRefreshToken, AccessTokenModified)
        };

        public void UpdateAccessToken(
            string? accessTokenValue,
            int accessTokenExpiresIn,
            string? accessTokenRefreshToken,
            DateTimeOffset modified,
            string? modifiedBy)

        {
            AccessTokenAccessToken = accessTokenValue;
            AccessTokenExpiresIn = accessTokenExpiresIn;
            AccessTokenRefreshToken = accessTokenRefreshToken;
            AccessTokenModified = modified;
            AccessTokenModifiedBy = modifiedBy;
        }
    }
}
