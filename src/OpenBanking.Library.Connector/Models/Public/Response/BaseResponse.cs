// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response
{
    public interface IBaseQuery
    {
        public Guid Id { get; }

        public string? Name { get; }

        public DateTimeOffset Created { get; }

        public string? CreatedBy { get; }
    }

    /// <summary>
    ///     Base response for any entity.
    /// </summary>
    public abstract class BaseResponse : IBaseQuery
    {
        internal BaseResponse(Guid id, string? name, DateTimeOffset created, string? createdBy)
        {
            Id = id;
            Name = name;
            Created = created;
            CreatedBy = createdBy;
        }

        /// <summary>
        ///     Unique OBC ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     Friendly name to support debugging etc. (must be unique i.e. not already in use).
        ///     This is optional.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        ///     Created timestamp
        /// </summary>
        public DateTimeOffset Created { get; }

        /// <summary>
        ///     "Created by" string. Similarly to "modified by" for mutable fields, this field
        ///     cna be used to denote authorship.
        /// </summary>
        public string? CreatedBy { get; }
    }
}
