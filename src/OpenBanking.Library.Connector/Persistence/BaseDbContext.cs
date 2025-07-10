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
using MongoDB.Bson;
using MongoDB.Driver;
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
public abstract class BaseDbContext(
    DbContextOptions options,
    DbProvider dbProvider,
    bool isRelationalNotDocumentDatabase,
    Formatting jsonFormatting = Formatting.None,
    IMongoDatabase? mongoDatabase = null) : DbContext(options)
{
    // Formatting choice for JSON fields
    private readonly Formatting _jsonFormatting = jsonFormatting;

    private readonly IMongoDatabase? _mongoDatabase = mongoDatabase;

    public bool IsRelationalNotDocumentDatabase { get; } = isRelationalNotDocumentDatabase;

    public DbProvider DbProvider { get; } = dbProvider;

    // Management objects
    internal DbSet<BankRegistrationEntity> BankRegistration => Set<BankRegistrationEntity>();

    internal DbSet<ObWacCertificateEntity> ObWacCertificate =>
        Set<ObWacCertificateEntity>();

    internal DbSet<ObSealCertificateEntity> ObSealCertificate =>
        Set<ObSealCertificateEntity>();

    internal DbSet<SoftwareStatementEntity> SoftwareStatement =>
        Set<SoftwareStatementEntity>();

    internal DbSet<EncryptionKeyDescriptionEntity> EncryptionKeyDescription =>
        Set<EncryptionKeyDescriptionEntity>();

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

    internal DbSet<ExternalApiSecretEntity> ExternalApiSecret =>
        Set<ExternalApiSecretEntity>();

    internal DbSet<RegistrationAccessTokenEntity> RegistrationAccessToken =>
        Set<RegistrationAccessTokenEntity>();

    // Settings
    internal DbSet<SettingsEntity> Settings => Set<SettingsEntity>();

    public IMongoDatabase GetMongoDatabase()
    {
        if (DbProvider is not DbProvider.MongoDb)
        {
            throw new InvalidOperationException("MongoDB database " + "unavailable as different provider in use.");
        }
        if (_mongoDatabase is null)
        {
            throw new InvalidOperationException("MongoDB database not found.");
        }
        return _mongoDatabase;
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        if (DbProvider is DbProvider.MongoDb)
        {
            configurationBuilder
                .IgnoreAny(typeof(IList<>));
            configurationBuilder.Properties<DateTimeOffset>().HaveBsonRepresentation(BsonType.DateTime);

            //configurationBuilder.Conventions.Remove<RelationshipDiscoveryConvention>();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bank configuration
        modelBuilder.ApplyConfiguration(
            new BankRegistrationConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new ObWacCertificateConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new ObSealCertificateConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new SoftwareStatementConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new EncryptionKeyDescriptionConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));

        // Auth contexts (note global query filter not supported for inherited types)
        // var x = new AuthContextConfig(DbProvider, true, JsonFormatting);
        modelBuilder.ApplyConfiguration(
            new AuthContextConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new AccountAccessConsentAuthContextConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticPaymentConsentAuthContextConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));

        modelBuilder.ApplyConfiguration(
            new DomesticVrpConsentAuthContextConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        // Consents
        modelBuilder.ApplyConfiguration(
            new AccountAccessConsentConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticPaymentConsentConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticVrpConsentConfig(true, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));

        // Encrypted objects
        modelBuilder.ApplyConfiguration(
            new EncryptedObjectConfig<EncryptedObject>(
                true,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new AccountAccessConsentAccessTokenConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new AccountAccessConsentRefreshTokenConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticPaymentConsentAccessTokenConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticPaymentConsentRefreshTokenConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticVrpConsentAccessTokenConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new DomesticVrpConsentRefreshTokenConfig(
                false,
                DbProvider,
                IsRelationalNotDocumentDatabase,
                _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new ExternalApiSecretConfig(false, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));
        modelBuilder.ApplyConfiguration(
            new RegistrationAccessTokenConfig(false, DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));

        modelBuilder.ApplyConfiguration(
            new SettingsConfig(DbProvider, IsRelationalNotDocumentDatabase, _jsonFormatting));

        modelBuilder.Entity<EncryptedObject>()
            .HasDiscriminator<string>("_t")
            .HasValue<AccountAccessConsentAccessToken>("AccountAccessConsentAccessToken")
            .HasValue<AccountAccessConsentRefreshToken>("AccountAccessConsentRefreshToken")
            .HasValue<DomesticVrpConsentAccessToken>("DomesticVrpConsentAccessToken")
            .HasValue<DomesticVrpConsentRefreshToken>("DomesticVrpConsentRefreshToken")
            .HasValue<DomesticPaymentConsentAccessToken>("DomesticPaymentConsentAccessToken")
            .HasValue<DomesticPaymentConsentRefreshToken>("DomesticPaymentConsentRefreshToken")
            .HasValue<ExternalApiSecretEntity>("ExternalApiSecretEntity")
            .HasValue<RegistrationAccessTokenEntity>("RegistrationAccessTokenEntity")
            .IsComplete(false);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration common to all DB contexts. Use*() method for provider selection
        // requires connection string so is specified in e.g. services.AddDbContext() method rather than
        // OnConfiguring() override.
        optionsBuilder
            .ConfigureWarnings(
                warnings =>
                {
                    // Suppress warnings relating to non-root auth context entities since can manually apply
                    // unsupported global query filter to entity queries
                    warnings.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);

                    //warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning);
                });

        if (DbProvider is not DbProvider.MongoDb)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        base.OnConfiguring(optionsBuilder);
    }
}
