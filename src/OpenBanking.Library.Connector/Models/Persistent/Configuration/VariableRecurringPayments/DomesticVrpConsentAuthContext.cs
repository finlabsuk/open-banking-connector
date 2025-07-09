// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments;

internal class
    DomesticVrpConsentAuthContext(
        bool supportsGlobalQueryFilter,
        DbProvider dbProvider,
        bool isRelationalDatabase,
        Formatting jsonFormatting)
    : AuthContextConfig<
        Persistent.VariableRecurringPayments.DomesticVrpConsentAuthContext>(
        supportsGlobalQueryFilter,
        dbProvider,
        isRelationalDatabase,
        jsonFormatting)
{
    public override void Configure(
        EntityTypeBuilder<Persistent.VariableRecurringPayments.DomesticVrpConsentAuthContext> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.DomesticVrpConsentId); // shared column

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne(e => e.DomesticVrpConsentNavigation)
                .WithMany()
                .HasForeignKey(e => e.DomesticVrpConsentId)
                .IsRequired();
        }
    }
}
