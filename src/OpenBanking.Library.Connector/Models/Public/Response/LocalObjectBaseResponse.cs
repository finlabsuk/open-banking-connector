// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBaseQuery
    {
        public Guid Id { get; }
        public string? Reference { get; }
        public DateTimeOffset Created { get; }
        public string? CreatedBy { get; }
    }

    /// <summary>
    ///     Base response for any entity.
    /// </summary>
    public abstract class LocalObjectBaseResponse : IBaseQuery
    {
        internal LocalObjectBaseResponse(Guid id, DateTimeOffset created, string? createdBy, string? reference)
        {
            Id = id;
            Created = created;
            CreatedBy = createdBy;
            Reference = reference;
        }

        /// <summary>
        ///     Unique Open Banking Connector ID (used in local database).
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     Created timestamp in local database.
        /// </summary>
        public DateTimeOffset Created { get; }

        /// <summary>
        ///     Optional "created by" string in local database. Similar to "modified by" for mutable fields in local database, this
        ///     field
        ///     can be used to denote authorship.
        /// </summary>
        public string? CreatedBy { get; }

        /// <summary>
        ///     Optional reference for linking object to something else - e.g. a user ID in the client
        ///     application. This field is not used by Open Banking Connector.
        /// </summary>
        public string? Reference { get; }
    }
}
