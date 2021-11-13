// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
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
using DomesticPaymentConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.DomesticPayment;
using DomesticVrpConsentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments.
    DomesticVrpConsent;
using AuthContextConfig = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AuthContext<
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AuthContext>;
using DomesticVrpConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using DomesticVrpConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments.DomesticVrp;


namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    // DB provider-independent DB context
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options) { }

        // Use no JSON formatting by default.
        protected virtual Formatting JsonFormatting { get; } = Formatting.None;

        // Bank configuration
        internal DbSet<Bank> Banks => Set<Bank>();
        internal DbSet<BankRegistration> BankRegistrations => Set<BankRegistration>();
        internal DbSet<BankApiSet> BankApiSets => Set<BankApiSet>();

        // Auth contexts
        internal DbSet<AuthContext> AuthContexts => Set<AuthContext>();
        internal DbSet<DomesticPaymentConsentAuthContext> DomesticPaymentConsentAuthContexts =>
            Set<DomesticPaymentConsentAuthContext>();
        internal DbSet<DomesticVrpConsentAuthContext> DomesticVrpConsentAuthContexts =>
            Set<DomesticVrpConsentAuthContext>();

        // Domestic payments
        internal DbSet<DomesticPaymentConsent> DomesticPaymentConsents => Set<DomesticPaymentConsent>();
        internal DbSet<DomesticPayment> DomesticPayments => Set<DomesticPayment>();

        // Domestic VRPs
        internal DbSet<DomesticVrpConsent> DomesticVrpConsents => Set<DomesticVrpConsent>();
        internal DbSet<DomesticVrp> DomesticVrps => Set<DomesticVrp>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bank configuration
            modelBuilder.ApplyConfiguration(new BankConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new BankApiSetConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new BankRegistrationConfig(true, JsonFormatting));

            // Auth contexts (note global query filter not supported for inherited types)
            modelBuilder.ApplyConfiguration(new AuthContextConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticPaymentConsentAuthContextConfig(false, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticVrpConsentAuthContextConfig(false, JsonFormatting));

            // Domestic payments
            modelBuilder.ApplyConfiguration(new DomesticPaymentConsentConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticPaymentConfig(true, JsonFormatting));

            // Domestic VRPs
            modelBuilder.ApplyConfiguration(new DomesticVrpConsentConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticVrpConfig(true, JsonFormatting));
        }
    }
}
