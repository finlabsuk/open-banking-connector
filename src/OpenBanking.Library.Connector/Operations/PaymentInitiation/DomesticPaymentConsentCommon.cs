// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class DomesticPaymentConsentCommon
{
    private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;

    public DomesticPaymentConsentCommon(
        IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
        IInstrumentationClient instrumentationClient)
    {
        _entityMethods = entityMethods;
        _instrumentationClient = instrumentationClient;
    }

    public async
        Task<(DomesticPaymentConsentPersisted persistedConsent, BankRegistrationEntity bankRegistration,
            DomesticPaymentConsentAccessToken? storedAccessToken, DomesticPaymentConsentRefreshToken? storedRefreshToken
            , SoftwareStatementEntity softwareStatementEntity, ExternalApiSecretEntity? externalApiSecret)>
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
                .Include(o => o.DomesticPaymentConsentAccessTokensNavigation)
                .Include(o => o.DomesticPaymentConsentRefreshTokensNavigation)
                .AsSplitQuery() // Load collections in separate SQL queries
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {consentId}.");
        DomesticPaymentConsentAccessToken? storedAccessToken =
            persistedConsent
                .DomesticPaymentConsentAccessTokensNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        DomesticPaymentConsentRefreshToken? storedRefreshToken =
            persistedConsent
                .DomesticPaymentConsentRefreshTokensNavigation
                .SingleOrDefault(x => !x.IsDeleted);
        BankRegistrationEntity bankRegistration = persistedConsent.BankRegistrationNavigation;
        SoftwareStatementEntity softwareStatementEntity =
            persistedConsent.BankRegistrationNavigation.SoftwareStatementNavigation!;
        ExternalApiSecretEntity? externalApiSecret =
            bankRegistration.ExternalApiSecretsNavigation
                .SingleOrDefault(x => !x.IsDeleted);

        return (persistedConsent, bankRegistration, storedAccessToken, storedRefreshToken,
            softwareStatementEntity, externalApiSecret);
    }
}
