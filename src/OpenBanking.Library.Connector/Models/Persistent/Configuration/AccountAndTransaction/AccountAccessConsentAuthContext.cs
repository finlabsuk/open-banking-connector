// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AccountAndTransaction
{
    internal class
        AccountAccessConsentAuthContext : AuthContextConfig<
            Persistent.AccountAndTransaction.AccountAccessConsentAuthContext>
    {
        public AccountAccessConsentAuthContext(bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
            base(
                supportsGlobalQueryFilter,
                jsonFormatting) { }

        public override void Configure(
            EntityTypeBuilder<Persistent.AccountAndTransaction.AccountAccessConsentAuthContext> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.AccountAccessConsentId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
