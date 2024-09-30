// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Management;

internal class BankRegistrationConfig : BaseConfig<BankRegistrationEntity>
{
    public BankRegistrationConfig(DbProvider dbProvider, bool supportsGlobalQueryFilter, Formatting jsonFormatting) :
        base(dbProvider, supportsGlobalQueryFilter, jsonFormatting) { }

    public override void Configure(EntityTypeBuilder<BankRegistrationEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.Id)
            .HasColumnOrder(1);
        builder.Property(e => e.ExternalApiId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.ExternalApiSecret);
        builder.Property(e => e.RegistrationAccessToken);
        builder.Property(e => e.DefaultResponseModeOverride)
            .HasConversion(new EnumToStringConverter<OAuth2ResponseMode>())
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.DefaultFragmentRedirectUri);
        builder.Property(e => e.DefaultQueryRedirectUri);
        builder.Property(e => e.RedirectUris)
            .HasConversion(
                v =>
                    JsonConvert.SerializeObject(
                        v,
                        _jsonFormatting,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<List<string>>(v)!,
                new ValueComparer<IList<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()));
        builder.Property(e => e.SoftwareStatementProfileId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.SoftwareStatementProfileOverride)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.SoftwareStatementId);
        builder.Property(e => e.RegistrationScope)
            .HasConversion(new EnumToStringConverter<RegistrationScopeEnum>())
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.TokenEndpointAuthMethod)
            .HasConversion(new EnumToStringConverter<TokenEndpointAuthMethodSupportedValues>())
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.BankGroup)
            .HasConversion(new EnumToStringConverter<BankGroup>());
        builder.Property(e => e.UseSimulatedBank)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.BankProfile)
            .HasConversion(new EnumToStringConverter<BankProfileEnum>());
        builder.Property(e => e.JwksUri)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.RegistrationEndpoint)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.TokenEndpoint)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.AuthorizationEndpoint)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        if (_dbProvider is DbProvider.PostgreSql)
        {
            builder.Property(e => e.RedirectUris).HasColumnType("jsonb");
        }

        if (_dbProvider is DbProvider.Sqlite)
        {
            builder.Property(e => e.Created).HasConversion(new DateTimeOffsetToBinaryConverter());
        }
    }
}
