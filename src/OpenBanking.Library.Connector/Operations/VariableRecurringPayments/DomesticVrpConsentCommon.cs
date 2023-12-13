// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

internal class DomesticVrpConsentCommon
{
    private readonly IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;

    public DomesticVrpConsentCommon(
        IDbReadWriteEntityMethods<DomesticVrpConsentPersisted> entityMethods,
        IInstrumentationClient instrumentationClient,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo)
    {
        _entityMethods = entityMethods;
        _instrumentationClient = instrumentationClient;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
    }

    public async
        Task<(DomesticVrpConsentPersisted persistedConsent, BankRegistration bankRegistration,
            DomesticVrpConsentAccessToken? storedAccessToken, DomesticVrpConsentRefreshToken? storedRefreshToken,
            ProcessedSoftwareStatementProfile
            processedSoftwareStatementProfile)> GetDomesticVrpConsent(Guid consentId, bool dbTracking)
    {
        IQueryable<DomesticVrpConsent> db = dbTracking
            ? _entityMethods.DbSet
            : _entityMethods.DbSetNoTracking;

        // Load DomesticVrpConsent and related
        DomesticVrpConsentPersisted persistedConsent =
            await db
                .Include(o => o.BankRegistrationNavigation)
                .Include(o => o.DomesticVrpConsentAccessTokensNavigation)
                .Include(o => o.DomesticVrpConsentRefreshTokensNavigation)
                .AsSplitQuery() // Load collections in separate SQL queries
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Domestic VRP Consent with ID {consentId}.");
        DomesticVrpConsentAccessToken? storedAccessToken =
            persistedConsent
                .DomesticVrpConsentAccessTokensNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        DomesticVrpConsentRefreshToken? storedRefreshToken =
            persistedConsent
                .DomesticVrpConsentRefreshTokensNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        BankRegistration bankRegistration = persistedConsent.BankRegistrationNavigation;

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
            ?.DomesticVrpConsentAuthGet
            ?.AudClaim ?? issuerUrl;
}
