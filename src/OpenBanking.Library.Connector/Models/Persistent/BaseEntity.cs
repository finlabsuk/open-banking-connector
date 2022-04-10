// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    [Index(nameof(Name), IsUnique = true)]
    internal abstract class BaseEntity : IEntity
    {
        protected BaseEntity(
            string? name,
            string? reference,
            Guid id,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy)
        {
            Name = name;
            Reference = reference;
            Id = id;
            IsDeleted = isDeleted;
            IsDeletedModified = isDeletedModified;
            IsDeletedModifiedBy = isDeletedModifiedBy;
            Created = created;
            CreatedBy = createdBy;
        }


        /// <summary>
        ///     Friendly name to support debugging etc. (must be unique i.e. not already in use).
        ///     This is optional.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        ///     Optional reference for linking object to something else - e.g. a user ID in the client
        ///     application. This field is not used by Open Banking Connector.
        /// </summary>
        public string? Reference { get; }

        /// <summary>
        ///     Unique Open Banking Connector ID
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        ///     Mutable "is deleted" status to support soft delete of record
        /// </summary>
        public bool IsDeleted { get; private set; }

        public DateTimeOffset IsDeletedModified { get; private set; }

        public string? IsDeletedModifiedBy { get; private set; }

        public void UpdateIsDeleted(bool isDeleted, DateTimeOffset isDeletedModified, string? modifiedBy)
        {
            IsDeleted = isDeleted;
            IsDeletedModified = isDeletedModified;
            IsDeletedModifiedBy = modifiedBy;
        }

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
