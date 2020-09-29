﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration
{
    internal abstract class Base<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            // Top-level read-only properties and foreign keys
            builder.Property(e => e.Created)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.CreatedBy)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.Id)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

            builder.OwnsOne(
                navigationExpression: e => e.IsDeleted,
                buildAction: o =>
                {
                    o.Property(e => e.Data);
                    o.Property(e => e.Modified);
                    o.Property(e => e.ModifiedBy);
                });
            builder.Navigation(e => e.IsDeleted).IsRequired();
        }
    }
}
