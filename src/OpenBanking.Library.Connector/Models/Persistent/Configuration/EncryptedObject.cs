// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal class EncryptedObjectConfig<TEntity>(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConfig<TEntity>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
    where TEntity : EncryptedObject
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property("_nonce").HasElementName("nonce");
        builder.Property("_text").HasElementName("text");
        builder.Property("_tag").HasElementName("tag");
        builder.Property("_text2").HasElementName("text2");

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne(e => e.EncryptionKeyDescriptionNavigation)
                .WithMany()
                .HasForeignKey(e => e.EncryptionKeyDescriptionId)
                .IsRequired();
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.ToCollection("encryptedObject");
            builder.Property(p => p.EncryptionKeyDescriptionId).HasElementName("encryptionKeyDescriptionId");
            builder.Property(p => p.Modified).HasElementName("modified");
            builder.Property(p => p.ModifiedBy).HasElementName("modifiedBy");
        }
    }
}
