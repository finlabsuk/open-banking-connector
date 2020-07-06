// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    // DB provider-independent DB context
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options) { }

        // Use no JSON formatting by default.
        protected virtual Formatting JsonFormatting { get; } = Formatting.None;

        public DbSet<BankClientProfile> BankClientProfiles { get; set; }

        public DbSet<ApiProfile> ApiProfiles { get; set; }

        public DbSet<DomesticConsent> DomesticConsents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Specify fields to be stored as JSON
            modelBuilder
                .Entity<BankClientProfile>(
                    c =>
                    {
                        c
                            .OwnsOne(e => e.OpenIdConfiguration)
                            .Property(e => e.ResponseTypesSupported)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v => JsonConvert.DeserializeObject<string[]>(v));
                    });

            modelBuilder
                .Entity<BankClientProfile>(
                    c =>
                    {
                        c
                            .OwnsOne(e => e.OpenIdConfiguration)
                            .Property(e => e.ScopesSupported)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v => JsonConvert.DeserializeObject<string[]>(v));
                    });

            modelBuilder
                .Entity<BankClientProfile>(
                    c =>
                    {
                        c
                            .OwnsOne(e => e.OpenIdConfiguration)
                            .Property(e => e.ResponseModesSupported)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v => JsonConvert.DeserializeObject<string[]>(v));
                    });

            modelBuilder
                .Entity<BankClientProfile>(
                    c =>
                    {
                        c
                            .Property(e => e.BankClientRegistrationClaims)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<BankClientRegistrationClaims>(v));
                    });

            modelBuilder
                .Entity<BankClientProfile>(
                    c =>
                    {
                        c
                            .Property(e => e.BankClientRegistrationData)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<BankClientRegistrationData>(v));
                    });

            modelBuilder.Entity<ApiProfile>(
                b =>
                {
                    b.Property(e => e.Id);
                    b.Property(e => e.BankClientProfileId);
                    b.Property(e => e.ApiVersion);
                    b.Property(e => e.BaseUrl);
                });

            modelBuilder
                .Entity<DomesticConsent>(
                    c =>
                    {
                        c
                            .Property(e => e.ObWriteDomesticConsent)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<OBWriteDomesticConsent>(v));
                    });


            modelBuilder
                .Entity<DomesticConsent>(
                    c =>
                    {
                        c
                            .Property(e => e.TokenEndpointResponse)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<TokenEndpointResponse>(v));
                    });
        }
    }
}
