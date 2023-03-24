﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class DomesticPaymentConsentCommon
{
    private readonly IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;

    public DomesticPaymentConsentCommon(
        IDbReadWriteEntityMethods<DomesticPaymentConsentPersisted> entityMethods,
        IInstrumentationClient instrumentationClient,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo)
    {
        _entityMethods = entityMethods;
        _instrumentationClient = instrumentationClient;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
    }

    public async
        Task<(DomesticPaymentConsentPersisted persistedConsent, string externalApiConsentId, BankRegistration
            bankRegistration, string bankFinancialId, ProcessedSoftwareStatementProfile
            processedSoftwareStatementProfile)> GetDomesticPaymentConsent(Guid consentId)
    {
        // Load DomesticPaymentConsent and related
        DomesticPaymentConsentPersisted persistedConsent =
            await _entityMethods
                .DbSetNoTracking
                .Include(o => o.DomesticPaymentConsentAuthContextsNavigation)
                .Include(o => o.BankRegistrationNavigation.BankNavigation)
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {consentId}.");
        string externalApiConsentId = persistedConsent.ExternalApiId;
        BankRegistration bankRegistration = persistedConsent.BankRegistrationNavigation;
        string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

        // Get software statement profile
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementProfileId,
                bankRegistration.SoftwareStatementProfileOverride);

        return (persistedConsent, externalApiConsentId, bankRegistration, bankFinancialId,
            processedSoftwareStatementProfile);
    }
}
