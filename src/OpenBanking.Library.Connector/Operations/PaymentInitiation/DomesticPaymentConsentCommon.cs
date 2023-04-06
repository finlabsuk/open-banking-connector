// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
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
        Task<(DomesticPaymentConsentPersisted persistedConsent, BankRegistration bankRegistration,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile)> GetDomesticPaymentConsent(
            Guid consentId)
    {
        // Load DomesticPaymentConsent and related
        DomesticPaymentConsentPersisted persistedConsent =
            await _entityMethods
                .DbSetNoTracking
                .Include(o => o.DomesticPaymentConsentAuthContextsNavigation)
                .Include(o => o.BankRegistrationNavigation)
                .SingleOrDefaultAsync(x => x.Id == consentId) ??
            throw new KeyNotFoundException($"No record found for Domestic Payment Consent with ID {consentId}.");
        BankRegistration bankRegistration = persistedConsent.BankRegistrationNavigation;

        // Get software statement profile
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementProfileId,
                bankRegistration.SoftwareStatementProfileOverride);

        return (persistedConsent, bankRegistration, processedSoftwareStatementProfile);
    }

    public static string GetBankTokenIssuerClaim(CustomBehaviourClass? customBehaviour, string issuerUrl) =>
        customBehaviour
            ?.DomesticPaymentConsentAuthGet
            ?.AudClaim ?? issuerUrl;
}
