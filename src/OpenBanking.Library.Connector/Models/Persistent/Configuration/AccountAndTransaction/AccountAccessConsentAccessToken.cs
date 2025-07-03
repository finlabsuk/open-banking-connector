// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AccountAndTransaction;

internal class
    AccountAccessConsentAccessTokenConfig : EncryptedObjectConfig<
        AccountAccessConsentAccessToken>
{
    public AccountAccessConsentAccessTokenConfig(
        DbProvider dbProvider,
        bool supportsGlobalQueryFilter,
        Formatting jsonFormatting) : base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

    public override void Configure(EntityTypeBuilder<AccountAccessConsentAccessToken> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.AccountAccessConsentId); // shared column

        if (_dbProvider is DbProvider.PostgreSql or DbProvider.Sqlite)
        {
            builder
                .HasOne(e => e.AccountAccessConsentNavigation)
                .WithMany(e => e.AccountAccessConsentAccessTokensNavigation)
                .HasForeignKey(e => e.AccountAccessConsentId)
                .IsRequired();
        }
    }
}
