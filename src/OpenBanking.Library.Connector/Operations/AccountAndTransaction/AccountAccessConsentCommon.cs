// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class AccountAccessConsentCommon
{
    private readonly IDbEntityMethods<AccountAccessConsentAccessToken> _accessTokenEntityMethods;
    private readonly IDbReadOnlyEntityMethods<BankRegistrationEntity> _bankRegistrationMethods;
    private readonly IDbReadOnlyMethods _dbMethods;
    private readonly IDbEntityMethods<AccountAccessConsentPersisted> _entityMethods;
    private readonly IDbReadOnlyEntityMethods<ExternalApiSecretEntity> _externalApiSecretMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbEntityMethods<AccountAccessConsentRefreshToken> _refreshTokenEntityMethods;
    private readonly IDbReadOnlyEntityMethods<SoftwareStatementEntity> _softwareStatementMethods;

    public AccountAccessConsentCommon(
        IDbEntityMethods<AccountAccessConsentPersisted> entityMethods,
        IDbEntityMethods<AccountAccessConsentAccessToken> accessTokenEntityMethods,
        IDbEntityMethods<AccountAccessConsentRefreshToken> refreshTokenEntityMethods,
        IInstrumentationClient instrumentationClient,
        IDbReadOnlyEntityMethods<SoftwareStatementEntity> softwareStatementMethods,
        IDbReadOnlyEntityMethods<ExternalApiSecretEntity> externalApiSecretMethods,
        IDbReadOnlyEntityMethods<BankRegistrationEntity> bankRegistrationMethods,
        IDbReadOnlyMethods dbMethods)
    {
        _entityMethods = entityMethods;
        _accessTokenEntityMethods = accessTokenEntityMethods;
        _instrumentationClient = instrumentationClient;
        _softwareStatementMethods = softwareStatementMethods;
        _externalApiSecretMethods = externalApiSecretMethods;
        _bankRegistrationMethods = bankRegistrationMethods;
        _dbMethods = dbMethods;
        _refreshTokenEntityMethods = refreshTokenEntityMethods;
    }

    public async
        Task<(AccountAccessConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
            SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? externalApiSecret)>
        GetAccountAccessConsent(
            Guid consentId,
            bool dbTracking)
    {
        IQueryable<AccountAccessConsentPersisted> db = dbTracking
            ? _entityMethods.DbSet
            : _entityMethods.DbSetNoTracking;

        AccountAccessConsentPersisted persistedConsent;
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
                throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
            bankRegistration = persistedConsent.BankRegistrationNavigation;

            softwareStatement = bankRegistration.SoftwareStatementNavigation;

            externalApiSecret = bankRegistration.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        }
        else
        {
            persistedConsent =
                await db
                    .SingleOrDefaultAsync(x => x.Id == consentId) ??
                throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
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
        IQueryable<AccountAccessConsentAccessToken> db = dbTracking
            ? _accessTokenEntityMethods.DbSet
            : _accessTokenEntityMethods.DbSetNoTracking;

        AccountAccessConsentAccessToken? accessToken =
            await db
                .SingleOrDefaultAsync(x => x.AccountAccessConsentId == consentId && !x.IsDeleted);

        return accessToken;
    }

    public AccessTokenEntity AddNewAccessToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid accountAccessConsentId)
    {
        var accountAccessConsentAccessToken =
            new AccountAccessConsentAccessToken(
                id,
                reference,
                isDeleted,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                accountAccessConsentId);
        _accessTokenEntityMethods.DbSet.AddAsync(accountAccessConsentAccessToken);
        return accountAccessConsentAccessToken;
    }

    public RefreshTokenEntity AddNewRefreshToken(
        Guid id,
        string? reference,
        bool isDeleted,
        DateTimeOffset isDeletedModified,
        string? isDeletedModifiedBy,
        DateTimeOffset created,
        string? createdBy,
        Guid accountAccessConsentId)
    {
        var accountAccessConsentRefreshToken =
            new AccountAccessConsentRefreshToken(
                Guid.NewGuid(),
                null,
                false,
                isDeletedModified,
                isDeletedModifiedBy,
                created,
                createdBy,
                accountAccessConsentId);
        _refreshTokenEntityMethods.DbSet.AddAsync(accountAccessConsentRefreshToken);
        return accountAccessConsentRefreshToken;
    }

    public async Task<RefreshTokenEntity?> GetRefreshToken(Guid consentId, bool dbTracking)
    {
        IQueryable<AccountAccessConsentRefreshToken> db = dbTracking
            ? _refreshTokenEntityMethods.DbSet
            : _refreshTokenEntityMethods.DbSetNoTracking;

        AccountAccessConsentRefreshToken? refreshToken =
            await db
                .SingleOrDefaultAsync(x => x.AccountAccessConsentId == consentId && !x.IsDeleted);

        return refreshToken;
    }

    public static string GetBankTokenIssuerClaim(CustomBehaviourClass? customBehaviour, string issuerUrl) =>
        customBehaviour
            ?.AccountAccessConsentAuthGet
            ?.AudClaim ?? issuerUrl;
}
