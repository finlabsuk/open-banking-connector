// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankConfiguration
{
    internal class Bank : BaseConfig<Persistent.BankConfiguration.Bank>
    {
        public Bank(DbProvider dbProvider, bool supportsGlobalQueryFilter, Formatting jsonFormatting) : base(
            dbProvider,
            supportsGlobalQueryFilter,
            jsonFormatting) { }

        public override void Configure(EntityTypeBuilder<Persistent.BankConfiguration.Bank> builder)
        {
            base.Configure(builder);

            // Top-level read-only properties
            builder.Property(e => e.IssuerUrl)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.FinancialId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
