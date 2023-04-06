// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class
    DomesticPaymentConsentAuthContextPost : LocalEntityCreate<
        DomesticPaymentConsentAuthContextPersisted,
        DomesticPaymentConsentAuthContextRequest,
        DomesticPaymentConsentAuthContextCreateResponse>
{
    private readonly IBankProfileService _bankProfileService;
    protected readonly IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted> _domesticPaymentConsentMethods;

    public DomesticPaymentConsentAuthContextPost(
        IDbReadWriteEntityMethods<DomesticPaymentConsentAuthContextPersisted>
            entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted> domesticPaymentConsentMethods,
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

    protected override async Task<DomesticPaymentConsentAuthContextCreateResponse> AddEntity(
        DomesticPaymentConsentAuthContextRequest request,
        ITimeProvider timeProvider)
    {
        // Load relevant data objects
        DomesticPaymentConsentPersisted domesticPaymentConsent =
            _domesticPaymentConsentMethods
                .DbSetNoTracking
                .Include(o => o.BankRegistrationNavigation)
                .SingleOrDefault(x => x.Id == request.DomesticPaymentConsentId) ??
            throw new KeyNotFoundException(
                $"No record found for Domestic Payment Consent with ID {request.DomesticPaymentConsentId}.");
        BankRegistration bankRegistration = domesticPaymentConsent.BankRegistrationNavigation;
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
            customBehaviour?.DomesticPaymentConsentAuthGet?.AudClaim ??
            issuerUrl;

        (string authUrl, string state, string nonce) = CreateAuthUrl.Create(
            domesticPaymentConsent.ExternalApiId,
            processedSoftwareStatementProfile,
            bankRegistration,
            bankRegistration.ExternalApiObject.ExternalApiId,
            customBehaviour?.DomesticPaymentConsentAuthGet,
            authorizationEndpoint,
            consentAuthGetAudClaim,
            supportsSca,
            "payments",
            _instrumentationClient);

        // Create persisted entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();

        var entity = new DomesticPaymentConsentAuthContextPersisted(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            state,
            nonce,
            request.DomesticPaymentConsentId);

        // Add entity
        await _entityMethods.AddAsync(entity);

        var response =
            new DomesticPaymentConsentAuthContextCreateResponse(
                entity.Id,
                entity.Created,
                entity.CreatedBy,
                entity.Reference,
                entity.DomesticPaymentConsentId,
                authUrl);

        return response;
    }
}
