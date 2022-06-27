// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    [Index(nameof(State), IsUnique = true)]
    [Index(nameof(Nonce), IsUnique = true)]
    internal class AuthContext : BaseEntity
    {
        public AuthContext(
            Guid id,
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string state,
            string nonce) : base(id, reference, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
        {
            State = state;
            Nonce = nonce;
        }

        /// <summary>
        ///     OAuth2 "state".
        /// </summary>
        public string State { get; }

        /// <summary>
        ///     OpenID Connect "nonce".
        /// </summary>
        public string Nonce { get; }
    }
}
