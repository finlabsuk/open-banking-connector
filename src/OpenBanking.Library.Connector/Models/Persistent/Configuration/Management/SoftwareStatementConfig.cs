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

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Management;

internal class SoftwareStatementConfig(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConfig<SoftwareStatementEntity>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<SoftwareStatementEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.OrganisationId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.SoftwareId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.SandboxEnvironment)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne(e => e.DefaultObSealCertificateNavigation)
                .WithMany()
                .HasForeignKey(e => e.DefaultObSealCertificateId)
                .IsRequired();

            builder
                .HasOne(e => e.DefaultObWacCertificateNavigation)
                .WithMany()
                .HasForeignKey(e => e.DefaultObWacCertificateId)
                .IsRequired();
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.ToCollection("softwareStatement");
            builder.Property(p => p.DefaultFragmentRedirectUrl).HasElementName("defaultFragmentRedirectUrl");
            builder.Property(p => p.DefaultObSealCertificateId).HasElementName("defaultObSealCertificateId");
            builder.Property(p => p.DefaultObWacCertificateId).HasElementName("defaultObWacCertificateId");
            builder.Property(p => p.DefaultQueryRedirectUrl).HasElementName("defaultQueryRedirectUrl");
            builder.Property(p => p.Modified).HasElementName("modified");
            builder.Property(p => p.OrganisationId).HasElementName("organisationId");
            builder.Property(p => p.SandboxEnvironment).HasElementName("sandboxEnvironment");
            builder.Property(p => p.SoftwareId).HasElementName("softwareId");
        }
    }
}
