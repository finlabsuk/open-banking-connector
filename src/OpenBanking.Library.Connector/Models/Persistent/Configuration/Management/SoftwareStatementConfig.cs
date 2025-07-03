// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Management;

internal class SoftwareStatementConfig : BaseConfig<SoftwareStatementEntity>
{
    public SoftwareStatementConfig(
        DbProvider dbProvider,
        bool supportsGlobalQueryFilter,
        Formatting jsonFormatting) : base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

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

        if (_dbProvider is DbProvider.PostgreSql or DbProvider.Sqlite)
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
    }
}
