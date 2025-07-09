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
    private readonly IDbEntityMethods<DomesticVrpConsentPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbEntityMethods<DomesticVrpConsentRefreshToken> _refreshTokenEntityMethods;

    public DomesticVrpConsentCommon(
        IDbEntityMethods<DomesticVrpConsentPersisted> entityMethods,
        IDbEntityMethods<DomesticVrpConsentAccessToken> accessTokenEntityMethods,
        IDbEntityMethods<DomesticVrpConsentRefreshToken> refreshTokenEntityMethods,
        IInstrumentationClient instrumentationClient)
    {
        _entityMethods = entityMethods;
        _instrumentationClient = instrumentationClient;
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
        DomesticVrpConsentPersisted persistedConsent =
            await db
                .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
                .Include(o => o.BankRegistrationNavigation.ExternalApiSecretsNavigation)
                .AsSplitQuery() // Load collections in separate SQL queries
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Domestic VRP Consent with ID {consentId}.");
        BankRegistrationEntity bankRegistration = persistedConsent.BankRegistrationNavigation;
        SoftwareStatementEntity softwareStatementEntity =
            persistedConsent.BankRegistrationNavigation.SoftwareStatementNavigation;
        ExternalApiSecretEntity? externalApiSecret =
            bankRegistration.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);

        return (persistedConsent, bankRegistration, softwareStatementEntity, externalApiSecret);
    }

    public async Task<AccessTokenEntity?> GetAccessToken(Guid consentId, bool dbTracking)
    {
        IQueryable<DomesticVrpConsentAccessToken> db = dbTracking
            ? _accessTokenEntityMethods.DbSet
            : _accessTokenEntityMethods.DbSetNoTracking;

        DomesticVrpConsentAccessToken? accessToken =
            await db
                .SingleOrDefaultAsync(x => x.DomesticVrpConsentId == consentId && !x.IsDeleted);

        return accessToken;
    }

    public async Task<RefreshTokenEntity?> GetRefreshToken(Guid consentId, bool dbTracking)
    {
        IQueryable<DomesticVrpConsentRefreshToken> db = dbTracking
            ? _refreshTokenEntityMethods.DbSet
            : _refreshTokenEntityMethods.DbSetNoTracking;

        DomesticVrpConsentRefreshToken? refreshToken =
            await db
                .SingleOrDefaultAsync(x => x.DomesticVrpConsentId == consentId && !x.IsDeleted);

        return refreshToken;
    }
}
