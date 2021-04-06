// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    internal abstract class EntityBase
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

        public Guid Id { get; set; }

        /// <summary>
        ///     Friendly name to support debugging etc. (must be unique i.e. not already in use).
        ///     This is optional.
        /// </summary>
        public string? Name { get; set; }

        public ReadWriteProperty<bool> IsDeleted { get; set; } = null!;

        public DateTimeOffset Created { get; set; }

        public string? CreatedBy { get; set; }
    }
}
