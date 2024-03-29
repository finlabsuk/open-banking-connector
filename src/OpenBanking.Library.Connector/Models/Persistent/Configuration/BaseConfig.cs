﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal abstract class BaseConfig<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    protected readonly DbProvider _dbProvider;
    protected readonly Formatting _jsonFormatting;
    private readonly bool _supportsGlobalQueryFilter;

    protected BaseConfig(DbProvider dbProvider, bool supportsGlobalQueryFilter, Formatting jsonFormatting)
    {
        _supportsGlobalQueryFilter = supportsGlobalQueryFilter;
        _jsonFormatting = jsonFormatting;
        _dbProvider = dbProvider;
    }

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
        if (_supportsGlobalQueryFilter)
        {
            builder.HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
