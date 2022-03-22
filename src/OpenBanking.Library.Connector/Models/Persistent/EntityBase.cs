// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    [Index(nameof(Name), IsUnique = true)]
    internal abstract class EntityBase : IEntity
    {
        public EntityBase() { }

        protected EntityBase(
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            Id = id;
            Name = name;
            IsDeleted = new ReadWriteProperty<bool>(false, timeProvider, CreatedBy);
            Created = timeProvider.GetUtcNow();
            CreatedBy = createdBy;
        }

        /// <summary>
        ///     Friendly name to support debugging etc. (must be unique i.e. not already in use).
        ///     This is optional.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     Optional reference for linking object to something else - e.g. a user ID in the client
        ///     application. This field is not used by Open Banking Connector.
        /// </summary>
        public string? Reference { get; set; }

        /// <summary>
        ///     Unique OBC ID
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Mutable "is deleted" status to support soft delete of record
        /// </summary>
        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;

        /// <summary>
        ///     Created timestamp
        /// </summary>
        public DateTimeOffset Created { get; set; }

        /// <summary>
        ///     "Created by" string. Similarly to "modified by" for mutable fields, this field
        ///     cna be used to denote authorship.
        /// </summary>
        public string? CreatedBy { get; set; }
    }
}
