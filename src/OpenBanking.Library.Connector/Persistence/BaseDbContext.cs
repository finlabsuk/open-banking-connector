// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    // DB provider-independent DB context
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<SoftwareStatementProfile> SoftwareStatementProfiles { get; set; }

        public DbSet<BankClientProfile> BankClientProfiles { get; set; }
        
        public DbSet<ApiProfile> ApiProfiles { get; set; }
        
        public DbSet<DomesticConsent> DomesticConsents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Specify fields to be stored as JSON
            modelBuilder
                .Entity<SoftwareStatementProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.SoftwareStatementPayload)
                        .Property(e => e.SoftwareRedirectUris)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });

            modelBuilder
                .Entity<SoftwareStatementProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.SoftwareStatementPayload)
                        .Property(e => e.SoftwareRoles)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });
            
            modelBuilder
                .Entity<BankClientProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.BankClientRegistrationClaims)
                        .Property(e => e.GrantTypes)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });

            modelBuilder
                .Entity<BankClientProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.BankClientRegistrationClaims)
                        .Property(e => e.ResponseTypes)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });
            
            modelBuilder
                .Entity<BankClientProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.BankClientRegistrationClaims)
                        .Property(e => e.RedirectUris)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });

            modelBuilder
                .Entity<BankClientProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.BankClientRegistrationClaims)
                        .Property(e => e.Scope)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });

            modelBuilder
                .Entity<BankClientProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.OpenIdConfiguration)
                        .Property(e => e.ResponseTypesSupported)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });

            modelBuilder
                .Entity<BankClientProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.OpenIdConfiguration)
                        .Property(e => e.ScopesSupported)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });

            modelBuilder
                .Entity<BankClientProfile>(c =>
                {
                    c
                        .OwnsOne(e => e.OpenIdConfiguration)
                        .Property(e => e.ResponseModesSupported)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<string[]>(v)
                        );
                });
            
            modelBuilder.Entity<ApiProfile>(b =>
            {
                b.Property(e => e.Id);
                b.Property(e => e.BankClientProfileId);                
                b.Property(e => e.ApiVersion);
                b.Property(e => e.BaseUrl);                
            });

            modelBuilder
                .Entity<DomesticConsent>(c =>
                {
                    c
                        .Property(e => e.ObWriteDomesticConsent)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<OBWriteDomesticConsent>(v)
                        );
                });

            
            modelBuilder
                .Entity<DomesticConsent>(c =>
                {
                    c
                        .Property(e => e.TokenEndpointResponse)
                        .HasConversion(
                            v => JsonConvert.SerializeObject(v),
                            v => JsonConvert.DeserializeObject<TokenEndpointResponse>(v)
                        );
                });

        }
    }
}
