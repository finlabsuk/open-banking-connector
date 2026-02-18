// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.EntityFrameworkCore.Extensions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments;

internal class DomesticVrpConsentConfig(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConsentConfig<DomesticVrpConsent>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<DomesticVrpConsent> builder)
    {
        base.Configure(builder);

        builder.Property(e => e.MigratedToV4);
        builder.Property(e => e.MigratedToV4Modified);

        if (_dbProvider is DbProvider.PostgreSql)
        {
            builder.Property(e => e.MigratedToV4Modified).HasColumnName("migrated_to_v4_modified");
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.ToCollection("domesticVrpConsent");
            builder.Property(p => p.MigratedToV4).HasElementName("migratedToV4");
            builder.Property(p => p.MigratedToV4Modified).HasElementName("migratedToV4Modified");
        }
    }
}
