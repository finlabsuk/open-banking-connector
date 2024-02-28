// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class AccountAccessConsentCommon
{
    private readonly IDbReadWriteEntityMethods<AccountAccessConsentPersisted> _entityMethods;

    public AccountAccessConsentCommon(
        IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods)
    {
        _entityMethods = entityMethods;
    }

    public async
        Task<(AccountAccessConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
            AccountAccessConsentAccessToken? storedAccessToken, AccountAccessConsentRefreshToken? storedRefreshToken,
            SoftwareStatementEntity softwareStatement)> GetAccountAccessConsent(
            Guid consentId,
            bool dbTracking)
    {
        IQueryable<AccountAccessConsentPersisted> db = dbTracking
            ? _entityMethods.DbSet
            : _entityMethods.DbSetNoTracking;

        AccountAccessConsentPersisted persistedConsent =
            await db
                .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
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
        BankRegistrationEntity bankRegistration = persistedConsent.BankRegistrationNavigation;

        SoftwareStatementEntity softwareStatement = bankRegistration.SoftwareStatementNavigation!;

        return (persistedConsent, bankRegistration, storedAccessToken, storedRefreshToken,
            softwareStatement);
    }

    public static string GetBankTokenIssuerClaim(CustomBehaviourClass? customBehaviour, string issuerUrl) =>
        customBehaviour
            ?.AccountAccessConsentAuthGet
            ?.AudClaim ?? issuerUrl;
}
