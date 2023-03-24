// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

    public async Task<(AccountAccessConsentPersisted persistedConsent, string externalApiConsentId,
        BankRegistrationPersisted
        bankRegistration, string bankFinancialId, ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
        string bankTokenIssuerClaim)> GetAccountAccessConsent(
        Guid consentId,
        bool dbTracking = false)
    {
        IQueryable<AccountAccessConsentPersisted> db = dbTracking
            ? _entityMethods.DbSet
            : _entityMethods.DbSetNoTracking;

        AccountAccessConsentPersisted persistedConsent =
            await db
                .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                .Include(o => o.BankRegistrationNavigation.BankNavigation)
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Account Access Consent with ID {consentId}.");
        string externalApiConsentId = persistedConsent.ExternalApiId;
        BankRegistrationPersisted bankRegistration = persistedConsent.BankRegistrationNavigation;
        string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

        // Get software statement profile
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementProfileId,
                bankRegistration.SoftwareStatementProfileOverride);

        // Get bank token issuer ("iss") claim
        string? requestObjectAudienceClaim =
            persistedConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                ?.AccountAccessConsentAuthGet
                ?.AudClaim;
        string bankTokenIssuerClaim =
            requestObjectAudienceClaim ??
            bankRegistration.BankNavigation.IssuerUrl;

        return (persistedConsent, externalApiConsentId, bankRegistration, bankFinancialId,
            processedSoftwareStatementProfile, bankTokenIssuerClaim);
    }
}
