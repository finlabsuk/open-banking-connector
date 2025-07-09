// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AccountAndTransaction;

internal class
    AccountAccessConsentRefreshTokenConfig(
        bool supportsGlobalQueryFilter,
        DbProvider dbProvider,
        bool isRelationalDatabase,
        Formatting jsonFormatting)
    : EncryptedObjectConfig<
        AccountAccessConsentRefreshToken>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<AccountAccessConsentRefreshToken> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.AccountAccessConsentId); // shared column

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne<AccountAccessConsent>()
                .WithMany(e => e.AccountAccessConsentRefreshTokensNavigation)
                .HasForeignKey(e => e.AccountAccessConsentId)
                .IsRequired();
        }
    }
}
