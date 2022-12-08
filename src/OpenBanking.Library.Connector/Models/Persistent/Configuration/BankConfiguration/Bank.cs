// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
            builder.Property(e => e.JwksUri)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.SupportsSca)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.IssuerUrl)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.FinancialId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.TokenEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.AuthorizationEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.DcrApiVersion)
                .HasConversion(new EnumToStringConverter<DynamicClientRegistrationApiVersion>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.IdTokenSubClaimType)
                .HasConversion(new EnumToStringConverter<IdTokenSubClaimType>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.CustomBehaviour)
                .HasConversion(
                    v => JsonConvert.SerializeObject(
                        v,
                        _jsonFormatting,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    v =>
                        JsonConvert.DeserializeObject<CustomBehaviourClass>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            if (_dbProvider is DbProvider.PostgreSql)
            {
                builder.Property(e => e.CustomBehaviour).HasColumnType("jsonb");
            }
        }
    }
}
