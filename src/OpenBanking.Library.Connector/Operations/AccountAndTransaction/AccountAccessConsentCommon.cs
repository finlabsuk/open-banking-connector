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
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentAccessToken> _accessTokenEntityMethods;
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentRefreshToken> _refreshTokenEntityMethods;

    public AccountAccessConsentCommon(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
        IDbReadWriteEntityMethods<AccountAccessConsentAccessToken> accessTokenEntityMethods,
        IDbReadWriteEntityMethods<AccountAccessConsentRefreshToken> refreshTokenEntityMethods,
        IInstrumentationClient instrumentationClient)
    {
        _entityMethods = entityMethods;
        _accessTokenEntityMethods = accessTokenEntityMethods;
        _instrumentationClient = instrumentationClient;
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

        AccountAccessConsentPersisted persistedConsent =
            await db
                .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
                .Include(o => o.BankRegistrationNavigation.ExternalApiSecretsNavigation)
                .AsSplitQuery() // Load collections in separate SQL queries
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
        BankRegistrationEntity bankRegistration = persistedConsent.BankRegistrationNavigation;

        SoftwareStatementEntity softwareStatement = bankRegistration.SoftwareStatementNavigation!;

        ExternalApiSecretEntity? externalApiSecret =
            bankRegistration.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);

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
