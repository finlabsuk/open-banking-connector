// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal class SettingsConfig(
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : IEntityTypeConfiguration<SettingsEntity>

{
    private readonly DbProvider _dbProvider = dbProvider;
    private readonly bool _isRelationalDatabase = isRelationalDatabase;
    private readonly Formatting _jsonFormatting = jsonFormatting;

    public void Configure(EntityTypeBuilder<SettingsEntity> builder)
    {
        // Read-only properties
        builder.Property(e => e.Id)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.Created)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne<EncryptionKeyDescriptionEntity>()
                .WithMany()
                .HasForeignKey(e => e.CurrentEncryptionKeyDescriptionId)
                .IsRequired();
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.ToCollection("settings");
            builder.Property(p => p.Created).HasElementName("created");
            builder.Property(p => p.CurrentEncryptionKeyDescriptionId)
                .HasElementName("currentEncryptionKeyDescriptionId");
            builder.Property(p => p.Modified).HasElementName("modified");
            builder.Property(p => p.DisableEncryption).HasElementName("disableEncryption");
        }
    }
}
