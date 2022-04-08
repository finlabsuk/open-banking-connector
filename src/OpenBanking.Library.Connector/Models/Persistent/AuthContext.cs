// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class AuthContext :
        EntityBase
    {
        public AuthContext() { }

        public AuthContext(
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider,
            string? accessToken,
            int accessTokenExpiresIn,
            string? refreshToken) : base(id, name, createdBy, timeProvider)
        {
            AccessToken = new ReadWritePropertyGroup<string?, int>(
                accessToken,
                accessTokenExpiresIn,
                timeProvider,
                createdBy);
            RefreshToken = new ReadWriteProperty<string?>(refreshToken, timeProvider, createdBy);
        }

        /// <summary>
        ///     Access token including "access_token" (value1) and "expires_in" (value2) fields. If value2 is null, indicates auth
        ///     not successfully completed.
        /// </summary>
        public ReadWritePropertyGroup<string?, int> AccessToken { get; set; } = null!;


        /// <summary>
        ///     Refresh token. If null, indicates no refresh token received.
        /// </summary>
        public ReadWriteProperty<string?> RefreshToken { get; set; } = null!;
    }
}
