// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Management;

internal class
    ExternalApiSecretConfig(
        bool supportsGlobalQueryFilter,
        DbProvider dbProvider,
        bool isRelationalDatabase,
        Formatting jsonFormatting)
    : EncryptedObjectConfig<
        ExternalApiSecretEntity>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<ExternalApiSecretEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.BankRegistrationId); // shared column

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne(e => e.BankRegistrationNavigation)
                .WithMany(e => e.ExternalApiSecretsNavigation)
                .HasForeignKey(e => e.BankRegistrationId);
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.Property(p => p.BankRegistrationId).HasElementName("bankRegistrationId");
        }
    }
}
