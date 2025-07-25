// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

internal class DomesticVrpConsentCommon
{
    private readonly IDbEntityMethods<DomesticVrpConsentAccessToken> _accessTokenEntityMethods;
    private readonly IDbReadOnlyEntityMethods<BankRegistrationEntity> _bankRegistrationMethods;
    private readonly IDbReadOnlyMethods _dbMethods;
    private readonly IDbEntityMethods<DomesticVrpConsentPersisted> _entityMethods;
    private readonly IDbReadOnlyEntityMethods<ExternalApiSecretEntity> _externalApiSecretMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbEntityMethods<DomesticVrpConsentRefreshToken> _refreshTokenEntityMethods;
    private readonly IDbReadOnlyEntityMethods<SoftwareStatementEntity> _softwareStatementMethods;

    public DomesticVrpConsentCommon(
        IDbEntityMethods<DomesticVrpConsentPersisted> entityMethods,
        IDbEntityMethods<DomesticVrpConsentAccessToken> accessTokenEntityMethods,
        IDbEntityMethods<DomesticVrpConsentRefreshToken> refreshTokenEntityMethods,
        IInstrumentationClient instrumentationClient,
        IDbReadOnlyEntityMethods<SoftwareStatementEntity> softwareStatementMethods,
        IDbReadOnlyEntityMethods<ExternalApiSecretEntity> externalApiSecretMethods,
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        IDbReadOnlyMethods dbMethods)
    {
        _entityMethods = entityMethods;
        _instrumentationClient = instrumentationClient;
        _softwareStatementMethods = softwareStatementMethods;
        _externalApiSecretMethods = externalApiSecretMethods;
        _bankRegistrationMethods = bankRegistrationMethods;
        _dbMethods = dbMethods;
        _refreshTokenEntityMethods = refreshTokenEntityMethods;
        _accessTokenEntityMethods = accessTokenEntityMethods;
    }

    public async
        Task<(DomesticVrpConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
            SoftwareStatementEntity softwareStatementEntity, ExternalApiSecretEntity? externalApiSecret)>
        GetDomesticVrpConsent(Guid consentId, bool dbTracking)
    {
        IQueryable<DomesticVrpConsent> db = dbTracking
            ? _entityMethods.DbSet
            : _entityMethods.DbSetNoTracking;

        // Load DomesticVrpConsent and related
        DomesticVrpConsentPersisted persistedConsent;
        BankRegistrationEntity bankRegistration;
        SoftwareStatementEntity softwareStatement;
        ExternalApiSecretEntity? externalApiSecret;
        if (_dbMethods.DbProvider is not DbProvider.MongoDb)
        {
            persistedConsent =
                await db
                    .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
                    .Include(o => o.BankRegistrationNavigation.ExternalApiSecretsNavigation)
                    .AsSplitQuery() // Load collections in separate SQL queries
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Domestic VRP Consent with ID {consentId}.");
            bankRegistration = persistedConsent.BankRegistrationNavigation;
            softwareStatement = persistedConsent.BankRegistrationNavigation.SoftwareStatementNavigation;
            externalApiSecret = bankRegistration.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        }
        else
        {
            persistedConsent =
                await db
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Domestic VRP Consent with ID {consentId}.");
            bankRegistration = await _bankRegistrationMethods
                .DbSetNoTracking
                .SingleAsync(x => x.Id == persistedConsent.BankRegistrationId);
            softwareStatement = await _softwareStatementMethods
                .DbSetNoTracking
                .SingleAsync(x => x.Id == bankRegistration.SoftwareStatementId);
            externalApiSecret = await _externalApiSecretMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.BankRegistrationId == bankRegistration.Id && !x.IsDeleted);
        }
        return (persistedConsent, bankRegistration, softwareStatement, externalApiSecret);
    }

    public async Task<AccessTokenEntity?> GetAccessToken(Guid consentId, bool dbTracking)
    {
        IQueryable<DomesticVrpConsentAccessToken> db = dbTracking
            ? _accessTokenEntityMethods.DbSet
            : _accessTokenEntityMethods.DbSetNoTracking;

        DomesticVrpConsentAccessToken? accessToken;
        if (_dbMethods.DbProvider is not DbProvider.MongoDb)
        {
            accessToken =
                await db
                    .SingleOrDefaultAsync(x => x.DomesticVrpConsentId == consentId && !x.IsDeleted);
        }
        else
        {
            accessToken =
                await db
                    .Where(x => EF.Property<string>(x, "_t") == "DomesticVrpConsentAccessToken")
                    .SingleOrDefaultAsync(x => x.DomesticVrpConsentId == consentId && !x.IsDeleted);
        }

        return accessToken;
    }

    public async Task<RefreshTokenEntity?> GetRefreshToken(Guid consentId, bool dbTracking)
    {
        IQueryable<DomesticVrpConsentRefreshToken> db = dbTracking
            ? _refreshTokenEntityMethods.DbSet
            : _refreshTokenEntityMethods.DbSetNoTracking;

        DomesticVrpConsentRefreshToken? refreshToken;
        if (_dbMethods.DbProvider is not DbProvider.MongoDb)
        {
            refreshToken =
                await db
                    .SingleOrDefaultAsync(x => x.DomesticVrpConsentId == consentId && !x.IsDeleted);
        }
        else
        {
            refreshToken =
                await db
                    .Where(x => EF.Property<string>(x, "_t") == "DomesticVrpConsentRefreshToken")
                    .SingleOrDefaultAsync(x => x.DomesticVrpConsentId == consentId && !x.IsDeleted);
        }

        return refreshToken;
    }

    public async Task<AccessTokenEntity> AddNewAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid domesticVrpConsentId)
    {
        var domesticVrpConsentAccessToken =
            new DomesticVrpConsentAccessToken(
                id,
                reference,
                isDeleted,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                domesticVrpConsentId);
        await _accessTokenEntityMethods.DbSet.AddAsync(domesticVrpConsentAccessToken);
        return domesticVrpConsentAccessToken;
    }

    public async Task<RefreshTokenEntity> AddNewRefreshToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid domesticVrpConsentId)
    {
        var domesticVrpConsentRefreshToken =
            new DomesticVrpConsentRefreshToken(
                id,
                reference,
                isDeleted,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                domesticVrpConsentId);
        await _refreshTokenEntityMethods.DbSet.AddAsync(domesticVrpConsentRefreshToken);
        return domesticVrpConsentRefreshToken;
    }
}
