// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class AuthContext : BaseEntity
    {
        public AuthContext(
            Guid id,
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string nonce) : base(id, reference, isDeleted, isDeletedModified, isDeletedModifiedBy, created, createdBy)
        {
            Nonce = nonce;
        }

        /// <summary>
        ///     OpenID Connect "nonce".
        /// </summary>
        public string Nonce { get; }
    }
}
