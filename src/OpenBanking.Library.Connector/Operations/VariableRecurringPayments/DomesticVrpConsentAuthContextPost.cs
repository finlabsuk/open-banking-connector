// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.DomesticVrpConsent;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

internal class
    DomesticVrpConsentAuthContextPost : LocalEntityCreate<
        DomesticVrpConsentAuthContextPersisted,
        DomesticVrpConsentAuthContextRequest,
        DomesticVrpConsentAuthContextCreateResponse>
{
    private readonly IBankProfileService _bankProfileService;
    protected readonly IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> _domesticPaymentConsentMethods;

    public DomesticVrpConsentAuthContextPost(
        IDbReadWriteEntityMethods<DomesticVrpConsentAuthContextPersisted>
            entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> domesticPaymentConsentMethods,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient)
    {
        _domesticPaymentConsentMethods = domesticPaymentConsentMethods;
        _bankProfileService = bankProfileService;
    }

    protected override async Task<DomesticVrpConsentAuthContextCreateResponse> AddEntity(
        DomesticVrpConsentAuthContextRequest request,
        ITimeProvider timeProvider)
    {
        // Load relevant data objects
        DomesticVrpConsentPersisted domesticVrpConsent =
            _domesticPaymentConsentMethods
                .DbSetNoTracking
                .Include(o => o.BankRegistrationNavigation)
                .SingleOrDefault(x => x.Id == request.DomesticVrpConsentId) ??
            throw new KeyNotFoundException(
                $"No record found for Domestic Payment Consent with ID {request.DomesticVrpConsentId}.");
        BankRegistration bankRegistration = domesticVrpConsent.BankRegistrationNavigation;
        string authorizationEndpoint =
            bankRegistration.AuthorizationEndpoint;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

        // Get software statement profile
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await _softwareStatementProfileRepo.GetAsync(
                bankRegistration.SoftwareStatementProfileId,
                bankRegistration
                    .SoftwareStatementProfileOverride);

        // Create auth URL
        string consentAuthGetAudClaim =
            customBehaviour?.DomesticVrpConsentAuthGet?.AudClaim ??
            issuerUrl;

        (string authUrl, string state, string nonce, string sessionId) = CreateAuthUrl.Create(
            domesticVrpConsent.ExternalApiId,
            processedSoftwareStatementProfile.OBSealKey,
            bankRegistration,
            bankRegistration.ExternalApiObject.ExternalApiId,
            customBehaviour?.DomesticVrpConsentAuthGet,
            authorizationEndpoint,
            consentAuthGetAudClaim,
            supportsSca,
            "payments",
            _instrumentationClient);

        // Create persisted entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new DomesticVrpConsentAuthContextPersisted(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            state,
            nonce,
            sessionId,
            request.DomesticVrpConsentId);

        // Add entity
        await _entityMethods.AddAsync(entity);

        var response =
            new DomesticVrpConsentAuthContextCreateResponse(
                entity.Id,
                entity.Created,
                entity.CreatedBy,
                entity.Reference,
                null,
                entity.DomesticVrpConsentId,
                state,
                authUrl,
                sessionId);

        return response;
    }
}
