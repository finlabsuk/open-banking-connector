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
using MongoDB.EntityFrameworkCore.Extensions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Management;

internal class BankRegistrationConfig(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConfig<BankRegistrationEntity>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
{
    public override void Configure(EntityTypeBuilder<BankRegistrationEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.ExternalApiId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.DefaultResponseModeOverride)
            .HasConversion(new EnumToStringConverter<OAuth2ResponseMode>())
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.DefaultFragmentRedirectUri)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.DefaultQueryRedirectUri)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
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
                    c => c.ToList()))
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
        builder.Property(e => e.AispUseV4)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.PispUseV4)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.VrpUseV4)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.BankProfile)
            .HasConversion(new EnumToStringConverter<BankProfileEnum>())
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
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

        // Only set up relationships (foreign keys and navigations) if not MongoDB
        if (_dbProvider is not DbProvider.MongoDb)
        {
            builder
                .HasOne(e => e.SoftwareStatementNavigation)
                .WithMany()
                .HasForeignKey(e => e.SoftwareStatementId);
        }

        // Use camel case for MongoDB
        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.ToCollection("bankRegistration");
            builder.Property(p => p.AispUseV4).HasElementName("aispUseV4");
            builder.Property(p => p.AuthorizationEndpoint).HasElementName("authorizationEndpoint");
            builder.Property(p => p.BankGroup).HasElementName("bankGroup");
            builder.Property(p => p.BankProfile).HasElementName("bankProfile");
            builder.Property(p => p.DefaultResponseModeOverride).HasElementName("defaultResponseModeOverride");
            builder.Property(p => p.DefaultFragmentRedirectUri).HasElementName("defaultFragmentRedirectUri");
            builder.Property(p => p.DefaultQueryRedirectUri).HasElementName("defaultQueryRedirectUri");
            builder.Property(p => p.ExternalApiId).HasElementName("externalApiId");
            builder.Property(p => p.JwksUri).HasElementName("jwksUri");
            builder.Property(p => p.PispUseV4).HasElementName("pispUseV4");
            builder.Property(p => p.RedirectUris).HasElementName("redirectUris");
            builder.Property(p => p.RegistrationEndpoint).HasElementName("registrationEndpoint");
            builder.Property(p => p.RegistrationScope).HasElementName("registrationScope");
            builder.Property(p => p.SoftwareStatementId).HasElementName("softwareStatementId");
            builder.Property(p => p.TokenEndpoint).HasElementName("tokenEndpoint");
            builder.Property(p => p.TokenEndpointAuthMethod).HasElementName("tokenEndpointAuthMethod");
            builder.Property(p => p.UseSimulatedBank).HasElementName("useSimulatedBank");
            builder.Property(p => p.VrpUseV4).HasElementName("vrpUseV4");
        }
    }
}
