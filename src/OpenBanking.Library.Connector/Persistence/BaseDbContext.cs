// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BankConfig = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankConfiguration.Bank;
using BankRegistrationConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.BankConfiguration.BankRegistration;
using DomesticPaymentConsentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.
    DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.
    DomesticPaymentConsentAuthContext;
using DomesticVrpConsentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments.
    DomesticVrpConsent;
using AuthContextConfig = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AuthContextConfig<
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AuthContext>;
using DomesticVrpConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using AccountAccessConsentConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AccountAndTransaction.
    AccountAccessConsent;
using AccountAccessConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AccountAndTransaction.
    AccountAccessConsentAuthContext;
using Bank = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.Bank;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;


namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    // DB provider-independent DB context
    public abstract class BaseDbContext : DbContext
    {
        protected BaseDbContext(DbContextOptions options) : base(options) { }

        // Use no JSON formatting by default.
        protected virtual Formatting JsonFormatting { get; } = Formatting.None;

        // Bank configuration
        internal DbSet<Bank> Bank => Set<Bank>();
        internal DbSet<BankRegistration> BankRegistration => Set<BankRegistration>();

        internal DbSet<AccountAndTransactionApiEntity> AccountAndTransactionApi =>
            Set<AccountAndTransactionApiEntity>();

        internal DbSet<PaymentInitiationApiEntity> PaymentInitiationApi =>
            Set<PaymentInitiationApiEntity>();

        internal DbSet<VariableRecurringPaymentsApiEntity> VariableRecurringPaymentsApi =>
            Set<VariableRecurringPaymentsApiEntity>();

        // Auth contexts
        internal DbSet<AuthContext> AuthContext => Set<AuthContext>();

        internal DbSet<AccountAccessConsentAuthContext> AccountAccessConsentAuthContext =>
            Set<AccountAccessConsentAuthContext>();

        internal DbSet<DomesticPaymentConsentAuthContext> DomesticPaymentConsentAuthContext =>
            Set<DomesticPaymentConsentAuthContext>();

        internal DbSet<DomesticVrpConsentAuthContext> DomesticVrpConsentAuthContext =>
            Set<DomesticVrpConsentAuthContext>();

        // Consents
        internal DbSet<AccountAccessConsent> AccountAccessConsent => Set<AccountAccessConsent>();
        internal DbSet<DomesticPaymentConsent> DomesticPaymentConsent => Set<DomesticPaymentConsent>();
        internal DbSet<DomesticVrpConsent> DomesticVrpConsent => Set<DomesticVrpConsent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Bank configuration
            modelBuilder.ApplyConfiguration(new BankConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new BankRegistrationConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new AccountAndTransactionApiConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new PaymentInitiationApiConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new VariableRecurringPaymentsApiConfig(true, JsonFormatting));

            // Auth contexts (note global query filter not supported for inherited types)
            modelBuilder.ApplyConfiguration(new AuthContextConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new AccountAccessConsentAuthContextConfig(false, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticPaymentConsentAuthContextConfig(false, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticVrpConsentAuthContextConfig(false, JsonFormatting));

            // Consents
            modelBuilder.ApplyConfiguration(new AccountAccessConsentConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticPaymentConsentConfig(true, JsonFormatting));
            modelBuilder.ApplyConfiguration(new DomesticVrpConsentConfig(true, JsonFormatting));
        }
    }
}
