// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BankConfig = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Bank;
using BankRegistrationConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankRegistration;
using BankApiSetConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankApiSet;
using DomesticPaymentConsentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.
    DomesticPaymentConsent;
using DomesticPaymentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.DomesticPayment;
using DomesticPaymentConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.
    DomesticPaymentConsentAuthContext;


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

        internal DbSet<BankApiSet> BankApiSets => Set<BankApiSet>();

        internal DbSet<DomesticPaymentConsent> DomesticPaymentConsents => Set<DomesticPaymentConsent>();

        internal DbSet<DomesticPayment> DomesticPayments => Set<DomesticPayment>();

        internal DbSet<DomesticPaymentConsentAuthContext> DomesticPaymentConsentAuthContexts =>
            Set<DomesticPaymentConsentAuthContext>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BankConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new BankApiSetConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new BankRegistrationConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new DomesticPaymentConsentConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new DomesticPaymentConfig(JsonFormatting));

            modelBuilder.ApplyConfiguration(new DomesticPaymentConsentAuthContextConfig(JsonFormatting));
        }
    }
}
