// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal abstract class BaseConfig<TEntity>(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    protected readonly DbProvider _dbProvider = dbProvider;
    protected readonly bool _isRelationalDatabase = isRelationalDatabase;
    protected readonly Formatting _jsonFormatting = jsonFormatting;

    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // Read-only properties
        builder.Property(e => e.Id)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.Reference)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.Created)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.CreatedBy)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        // Enforce soft delete where supported (global query filters not supported for inherited types in type per hierarchy)
        if (supportsGlobalQueryFilter)
        {
            builder.HasQueryFilter(p => !p.IsDeleted);
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.Property(p => p.Created).HasElementName("created");
            builder.Property(p => p.CreatedBy).HasElementName("createdBy");
            builder.Property(p => p.IsDeleted).HasElementName("isDeleted");
            builder.Property(p => p.IsDeletedModified).HasElementName("isDeletedModified");
            builder.Property(p => p.IsDeletedModifiedBy).HasElementName("isDeletedModifiedBy");
            builder.Property(p => p.Reference).HasElementName("reference");
        }
    }
}
