// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal class BaseConsentConfig<TEntity>(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConfig<TEntity>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
    where TEntity : BaseConsent
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.Id);
        builder.Property(e => e.BankRegistrationId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.CreatedWithV4)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        builder.Property(e => e.ExternalApiId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.AuthContextState);
        builder.Property(e => e.AuthContextNonce);
        builder.Property(e => e.AuthContextModified);
        builder.Property(e => e.AuthContextModifiedBy);
        builder.Property(e => e.AuthContextCodeVerifier);
        builder.Property(e => e.ExternalApiUserId);
        builder.Property(e => e.ExternalApiUserIdModified);
        builder.Property(e => e.ExternalApiUserIdModifiedBy);

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne(e => e.BankRegistrationNavigation)
                .WithMany()
                .HasForeignKey(e => e.BankRegistrationId)
                .IsRequired();
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.Property(p => p.AuthContextCodeVerifier).HasElementName("authContextCodeVerifier");
            builder.Property(p => p.AuthContextModified).HasElementName("authContextModified");
            builder.Property(p => p.AuthContextModifiedBy).HasElementName("authContextModifiedBy");
            builder.Property(p => p.AuthContextNonce).HasElementName("authContextNonce");
            builder.Property(p => p.AuthContextState).HasElementName("authContextState");
            builder.Property(p => p.BankRegistrationId).HasElementName("bankRegistrationId");
            builder.Property(p => p.CreatedWithV4).HasElementName("createdWithV4");
            builder.Property(p => p.ExternalApiId).HasElementName("externalApiId");
            builder.Property(p => p.ExternalApiUserId).HasElementName("externalApiUserId");
            builder.Property(p => p.ExternalApiUserIdModified).HasElementName("externalApiUserIdModified");
            builder.Property(p => p.ExternalApiUserIdModifiedBy).HasElementName("externalApiUserIdModifiedBy");
        }
    }
}
