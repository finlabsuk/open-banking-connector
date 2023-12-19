// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class AccountAccessConsentCommon
{
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;

    public AccountAccessConsentCommon(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo)
    {
        _entityMethods = entityMethods;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
    }

    public async
        Task<(AccountAccessConsentPersisted persistedConsent, BankRegistrationPersisted bankRegistration,
            AccountAccessConsentAccessToken? storedAccessToken, AccountAccessConsentRefreshToken? storedRefreshToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile)> GetAccountAccessConsent(
            Guid consentId,
            bool dbTracking)
    {
        IQueryable<AccountAccessConsentPersisted> db = dbTracking
            ? _entityMethods.DbSet
            : _entityMethods.DbSetNoTracking;

        AccountAccessConsentPersisted persistedConsent =
            await db
                .Include(o => o.BankRegistrationNavigation)
                .Include(o => o.AccountAccessConsentAccessTokensNavigation)
                .Include(o => o.AccountAccessConsentRefreshTokensNavigation)
                .AsSplitQuery() // Load collections in separate SQL queries
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
        AccountAccessConsentAccessToken? storedAccessToken =
            persistedConsent
                .AccountAccessConsentAccessTokensNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        AccountAccessConsentRefreshToken? storedRefreshToken =
            persistedConsent
                .AccountAccessConsentRefreshTokensNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        BankRegistrationPersisted bankRegistration = persistedConsent.BankRegistrationNavigation;

        // Get software statement profile
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementId.ToString(),
                bankRegistration.SoftwareStatementProfileOverride);

        return (persistedConsent, bankRegistration, storedAccessToken, storedRefreshToken,
            processedSoftwareStatementProfile);
    }

    public static string GetBankTokenIssuerClaim(CustomBehaviourClass? customBehaviour, string issuerUrl) =>
        customBehaviour
            ?.AccountAccessConsentAuthGet
            ?.AudClaim ?? issuerUrl;
}
