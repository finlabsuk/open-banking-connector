// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BankConfig = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Bank;
using BankProfileConfig = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankProfile;
using BankRegistrationConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankRegistration;
using DomesticPaymentConsentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.
    DomesticPaymentConsent;
using DomesticPaymentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.DomesticPayment;


namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    // DB provider-independent DB context
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options) { }

        // Use no JSON formatting by default.
        protected virtual Formatting JsonFormatting { get; } = Formatting.None;

        internal DbSet<Bank> Banks => Set<Bank>();

        internal DbSet<BankRegistration> BankRegistrations => Set<BankRegistration>();

        internal DbSet<BankProfile> BankProfiles => Set<BankProfile>();

        internal DbSet<DomesticPaymentConsent> DomesticPaymentConsents => Set<DomesticPaymentConsent>();

        internal DbSet<DomesticPayment> DomesticPayments => Set<DomesticPayment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BankConfig());

            modelBuilder.ApplyConfiguration(new BankProfileConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new BankRegistrationConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new DomesticPaymentConsentConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new DomesticPaymentConfig(JsonFormatting));

            // modelBuilder
            //     .Entity<DomesticPaymentConsent>()
            //     .HasOne<BankProfile>()
            //     .WithMany()
            //     .HasForeignKey(p => p.BankProfileId);
        }
    }
}
