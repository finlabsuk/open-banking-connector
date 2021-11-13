// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal abstract class Base<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : EntityBase
    {
        protected readonly Formatting _jsonFormatting;
        private readonly bool _supportsGlobalQueryFilter;

        protected Base(bool supportsGlobalQueryFilter, Formatting jsonFormatting)
        {
            _supportsGlobalQueryFilter = supportsGlobalQueryFilter;
            _jsonFormatting = jsonFormatting;
        }

        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Top-level read-only properties
            builder.Property(e => e.Id)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Name)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Created)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.CreatedBy)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            builder.OwnsOne(
                e => e.IsDeleted,
                o =>
                {
                    o.Property(e => e.Data)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    o.Property(e => e.Modified)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                    o.Property(e => e.ModifiedBy)
                        .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
                });
            builder.Navigation(e => e.IsDeleted).IsRequired();

            // Enforce soft delete where supported (global query filters not supported for inherited types in type per hierarchy)
            if (_supportsGlobalQueryFilter)
            {
                builder.HasQueryFilter(p => !p.IsDeleted.Data);
            }
        }
    }
}
