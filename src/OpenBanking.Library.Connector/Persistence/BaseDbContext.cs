// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
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

        public DbSet<Bank> Banks => Set<Bank>();

        public DbSet<BankRegistration> BankRegistrations => Set<BankRegistration>();

        public DbSet<BankProfile> BankProfiles => Set<BankProfile>();

        public DbSet<DomesticPaymentConsent> DomesticConsents => Set<DomesticPaymentConsent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Navigation properties
            modelBuilder
                .Entity<Bank>()
                .HasOne<BankRegistration>()
                .WithOne()
                .HasForeignKey<Bank>(b => b.DefaultBankRegistrationId);

            modelBuilder
                .Entity<Bank>()
                .HasOne<BankRegistration>()
                .WithOne()
                .HasForeignKey<Bank>(b => b.StagingBankRegistrationId);

            modelBuilder
                .Entity<Bank>()
                .HasOne<BankProfile>()
                .WithOne()
                .HasForeignKey<Bank>(b => b.DefaultBankProfileId);

            modelBuilder
                .Entity<Bank>()
                .HasOne<BankProfile>()
                .WithOne()
                .HasForeignKey<Bank>(b => b.StagingBankProfileId);

            modelBuilder
                .Entity<BankRegistration>()
                .HasOne<Bank>()
                .WithMany()
                .HasForeignKey(r => r.BankId);
            
            modelBuilder
                .Entity<BankProfile>()
                .HasOne<Bank>()
                .WithMany()
                .HasForeignKey(p => p.BankId);
            
            modelBuilder
                .Entity<BankProfile>()
                .HasOne<BankRegistration>()
                .WithMany()
                .HasForeignKey(p => p.BankRegistrationId);

            // modelBuilder
            //     .Entity<DomesticPaymentConsent>()
            //     .HasOne<BankProfile>()
            //     .WithMany()
            //     .HasForeignKey(p => p.BankProfileId);
            
            // Specify fields to be stored as JSON
            modelBuilder
                .Entity<BankRegistration>(
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
                .Entity<BankRegistration>(
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
                .Entity<BankRegistration>(
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
                .Entity<BankRegistration>(
                    c =>
                    {
                        c
                            .Property(e => e.BankClientRegistrationRequestData)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<OBClientRegistration1>(v));
                    });

            modelBuilder
                .Entity<BankRegistration>(
                    c =>
                    {
                        c
                            .Property(e => e.BankClientRegistrationData)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<OBClientRegistration1>(v));
                    });

            modelBuilder
                .Entity<BankProfile>(
                    c =>
                    {
                        c
                            .Property(e => e.PaymentInitiationApi)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<PaymentInitiationApi>(v));
                    });

            modelBuilder
                .Entity<DomesticPaymentConsent>(
                    c =>
                    {
                        c
                            .Property(e => e.ObWriteDomesticConsent)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<OBWriteDomesticConsent4>(v));
                    });
            
            modelBuilder
                .Entity<DomesticPaymentConsent>(
                    c =>
                    {
                        c
                            .Property(e => e.ObWriteDomesticResponse)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<OBWriteDomesticConsentResponse4>(v));
                    });

            modelBuilder
                .Entity<DomesticPaymentConsent>(
                    c =>
                    {
                        c
                            .Property(e => e.TokenEndpointResponse)
                            .HasConversion(
                                convertToProviderExpression: v => JsonConvert.SerializeObject(v, JsonFormatting),
                                convertFromProviderExpression: v =>
                                    JsonConvert.DeserializeObject<TokenEndpointResponse?>(v));
                    });
        }
    }
}
