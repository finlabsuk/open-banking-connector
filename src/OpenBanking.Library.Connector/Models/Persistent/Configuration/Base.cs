// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal abstract class Base<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : EntityBase
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Top-level read-only properties and foreign keys
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
                    o.Property(e => e.Data);
                    o.Property(e => e.Modified);
                    o.Property(e => e.ModifiedBy);
                });
            builder.Navigation(e => e.IsDeleted).IsRequired();

            builder.HasQueryFilter(p => !p.IsDeleted.Data);
        }
    }
}
