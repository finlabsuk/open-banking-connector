// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using DomesticPaymentConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.PaymentInitiation.
    DomesticPaymentConsentAuthContext;
using AuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.AuthContextConfig<
        FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AuthContext>;
using DomesticPaymentConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;
using DomesticVrpConsentAuthContext =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentAuthContextConfig =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

// DB provider-independent DB context
public abstract class BaseDbContext : DbContext
{
    protected BaseDbContext(DbContextOptions options) : base(options) { }

    // Formatting choice for JSON fields
    protected virtual Formatting JsonFormatting { get; } = Formatting.None;

    // Set DB Provider
    protected abstract DbProvider DbProvider { get; }

    // Management objects
    internal DbSet<BankRegistrationEntity> BankRegistration => Set<BankRegistrationEntity>();

    public DbSet<ObWacCertificateEntity> ObWacCertificate =>
        Set<ObWacCertificateEntity>();

    public DbSet<ObSealCertificateEntity> ObSealCertificate =>
        Set<ObSealCertificateEntity>();

    internal DbSet<SoftwareStatementEntity> SoftwareStatement =>
        Set<SoftwareStatementEntity>();

    // Deprecated objects
    internal DbSet<Bank> Bank => Set<Bank>();

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

    // Encrypted objects
    internal DbSet<EncryptedObject> EncryptedObject => Set<EncryptedObject>();

    internal DbSet<AccountAccessConsentAccessToken> AccountAccessConsentAccessToken =>
        Set<AccountAccessConsentAccessToken>();

    internal DbSet<AccountAccessConsentRefreshToken> AccountAccessConsentRefreshToken =>
        Set<AccountAccessConsentRefreshToken>();

    internal DbSet<DomesticPaymentConsentAccessToken> DomesticPaymentConsentAccessToken =>
        Set<DomesticPaymentConsentAccessToken>();

    internal DbSet<DomesticPaymentConsentRefreshToken> DomesticPaymentConsentRefreshToken =>
        Set<DomesticPaymentConsentRefreshToken>();

    internal DbSet<DomesticVrpConsentAccessToken> DomesticVrpConsentAccessToken =>
        Set<DomesticVrpConsentAccessToken>();

    internal DbSet<DomesticVrpConsentRefreshToken> DomesticVrpConsentRefreshToken =>
        Set<DomesticVrpConsentRefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Bank configuration
        modelBuilder.ApplyConfiguration(new BankConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new BankRegistrationConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new AccountAndTransactionApiConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new PaymentInitiationApiConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new VariableRecurringPaymentsApiConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new ObWacCertificateConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new ObSealCertificateConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new SoftwareStatementConfig(DbProvider, true, JsonFormatting));

        // Auth contexts (note global query filter not supported for inherited types)
        // var x = new AuthContextConfig(DbProvider, true, JsonFormatting);
        modelBuilder.ApplyConfiguration(new AuthContextConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new AccountAccessConsentAuthContextConfig(DbProvider, false, JsonFormatting));
        modelBuilder.ApplyConfiguration(new DomesticPaymentConsentAuthContextConfig(DbProvider, false, JsonFormatting));
        modelBuilder.ApplyConfiguration(new DomesticVrpConsentAuthContextConfig(DbProvider, false, JsonFormatting));

        // Consents
        modelBuilder.ApplyConfiguration(new AccountAccessConsentConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new DomesticPaymentConsentConfig(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new DomesticVrpConsentConfig(DbProvider, true, JsonFormatting));

        // Encrypted objects
        // var y = new EncryptedObjectConfig<EncryptedObject>(DbProvider, true, JsonFormatting);
        modelBuilder.ApplyConfiguration(new EncryptedObjectConfig<EncryptedObject>(DbProvider, true, JsonFormatting));
        modelBuilder.ApplyConfiguration(new AccountAccessConsentAccessTokenConfig(DbProvider, false, JsonFormatting));
        modelBuilder.ApplyConfiguration(new AccountAccessConsentRefreshTokenConfig(DbProvider, false, JsonFormatting));
        modelBuilder.ApplyConfiguration(new DomesticPaymentConsentAccessTokenConfig(DbProvider, false, JsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticPaymentConsentRefreshTokenConfig(DbProvider, false, JsonFormatting));
        modelBuilder.ApplyConfiguration(new DomesticVrpConsentAccessTokenConfig(DbProvider, false, JsonFormatting));
        modelBuilder.ApplyConfiguration(new DomesticVrpConsentRefreshTokenConfig(DbProvider, false, JsonFormatting));

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration common to all DB contexts. Use*() method for provider selection
        // requires connection string so is specified in e.g. services.AddDbContext() method rather than
        // OnConfiguring() override.
        optionsBuilder
            .ConfigureWarnings(
                warnings =>
                    // Suppress warnings relating to non-root auth context entities since can manually apply
                    // unsupported global query filter to entity queries
                    warnings.Ignore(
                        CoreEventId
                            .PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning))
            .UseSnakeCaseNamingConvention();

        base.OnConfiguring(optionsBuilder);
    }
}
