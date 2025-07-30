// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments;

internal class
    DomesticVrpConsentAccessTokenConfig(
        bool supportsGlobalQueryFilter,
        DbProvider dbProvider,
        bool isRelationalDatabase,
        Formatting jsonFormatting)
    : EncryptedObjectConfig<
        DomesticVrpConsentAccessToken>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<DomesticVrpConsentAccessToken> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.DomesticVrpConsentId); // shared column

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne<DomesticVrpConsent>()
                .WithMany(e => e.DomesticVrpConsentAccessTokensNavigation)
                .HasForeignKey(e => e.DomesticVrpConsentId);
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.Property(p => p.DomesticVrpConsentId).HasElementName("domesticVrpConsentId");
        }
    }
}
