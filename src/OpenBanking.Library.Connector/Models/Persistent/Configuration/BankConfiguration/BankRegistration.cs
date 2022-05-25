// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankConfiguration
{
    internal class BankRegistration : BaseConfig<Persistent.BankConfiguration.BankRegistration>
    {
        public BankRegistration(DbProvider dbProvider, bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
            base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

        public override void Configure(EntityTypeBuilder<Persistent.BankConfiguration.BankRegistration> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.Id)
                .HasColumnOrder(1);
            builder.Property(e => e.BankId)
                .HasColumnOrder(2)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property("_externalApiId")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property("_externalApiSecret")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property("_registrationAccessToken")
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.SoftwareStatementProfileId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.SoftwareStatementProfileOverride)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationScope)
                .HasConversion(new EnumToStringConverter<RegistrationScopeEnum>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.TokenEndpointAuthMethod)
                .HasConversion(new EnumToStringConverter<TokenEndpointAuthMethod>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
