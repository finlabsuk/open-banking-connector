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
    private readonly IDbEntityMethods<DomesticPaymentConsentAccessToken> _accessTokenEntityMethods;
    private readonly IDbReadOnlyEntityMethods<BankRegistrationEntity> _bankRegistrationMethods;
    private readonly IDbReadOnlyMethods _dbMethods;
    private readonly IDbEntityMethods<DomesticPaymentConsentPersisted> _entityMethods;
    private readonly IDbReadOnlyEntityMethods<ExternalApiSecretEntity> _externalApiSecretMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbEntityMethods<DomesticPaymentConsentRefreshToken> _refreshTokenEntityMethods;
    private readonly IDbReadOnlyEntityMethods<SoftwareStatementEntity> _softwareStatementMethods;

    public DomesticPaymentConsentCommon(
        IDbEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
        IDbEntityMethods<DomesticPaymentConsentAccessToken> accessTokenEntityMethods,
        IDbEntityMethods<DomesticPaymentConsentRefreshToken> refreshTokenEntityMethods,
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
        DomesticPaymentConsentPersisted persistedConsent;
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
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {consentId}.");
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
                throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {consentId}.");
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
