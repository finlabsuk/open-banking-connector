// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
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
        public BankRegistration(bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
            base(
                supportsGlobalQueryFilter,
                jsonFormatting) { }

        public override void Configure(EntityTypeBuilder<Persistent.BankConfiguration.BankRegistration> builder)
        {
            base.Configure(builder);

            // Top-level property info: read-only, JSON conversion, etc
            builder.Property(e => e.BankId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.SoftwareStatementProfileId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.SoftwareStatementAndCertificateProfileOverrideCase)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationScope)
                .HasConversion(new EnumToStringConverter<RegistrationScopeEnum>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.DynamicClientRegistrationApiVersion)
                .HasConversion(new EnumToStringConverter<DynamicClientRegistrationApiVersion>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.TokenEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.AuthorizationEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationEndpoint)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.TokenEndpointAuthMethod)
                .HasConversion(new EnumToStringConverter<TokenEndpointAuthMethodEnum>())
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.CustomBehaviour)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, _jsonFormatting),
                    v =>
                        JsonConvert.DeserializeObject<CustomBehaviour>(v))
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.ExternalApiId)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.ExternalApiSecret)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
            builder.Property(e => e.RegistrationAccessToken)
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }
    }
}
