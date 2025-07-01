// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class DomesticPaymentConsentCommon
{
    private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentAccessToken> _accessTokenEntityMethods;
    private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentRefreshToken> _refreshTokenEntityMethods;

    public DomesticPaymentConsentCommon(
        IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
        IDbReadWriteEntityMethods<DomesticPaymentConsentAccessToken> accessTokenEntityMethods,
        IDbReadWriteEntityMethods<DomesticPaymentConsentRefreshToken> refreshTokenEntityMethods,
        IInstrumentationClient instrumentationClient)
    {
        _entityMethods = entityMethods;
        _instrumentationClient = instrumentationClient;
        _refreshTokenEntityMethods = refreshTokenEntityMethods;
        _accessTokenEntityMethods = accessTokenEntityMethods;
    }

    public async
        Task<(DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
            SoftwareStatementEntity softwareStatementEntity, ExternalApiSecretEntity? externalApiSecret)>
        GetDomesticPaymentConsent(
            Guid consentId,
            bool dbTracking)
    {
        IQueryable<DomesticPaymentConsent> db = dbTracking
            ? _entityMethods.DbSet
            : _entityMethods.DbSetNoTracking;

        // Load DomesticPaymentConsent and related
        DomesticPaymentConsentPersisted persistedConsent =
            await db
                .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
                .Include(o => o.BankRegistrationNavigation.ExternalApiSecretsNavigation)
                .AsSplitQuery() // Load collections in separate SQL queries
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {consentId}.");
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
        IQueryable<DomesticPaymentConsentAccessToken> db = dbTracking
            ? _accessTokenEntityMethods.DbSet
            : _accessTokenEntityMethods.DbSetNoTracking;

        DomesticPaymentConsentAccessToken? accessToken =
            await db
                .SingleOrDefaultAsync(x => x.DomesticPaymentConsentId == consentId && !x.IsDeleted);

        return accessToken;
    }

    public async Task<RefreshTokenEntity?> GetRefreshToken(Guid consentId, bool dbTracking)
    {
        IQueryable<DomesticPaymentConsentRefreshToken> db = dbTracking
            ? _refreshTokenEntityMethods.DbSet
            : _refreshTokenEntityMethods.DbSetNoTracking;

        DomesticPaymentConsentRefreshToken? refreshToken =
            await db
                .SingleOrDefaultAsync(x => x.DomesticPaymentConsentId == consentId && !x.IsDeleted);

        return refreshToken;
    }
}
