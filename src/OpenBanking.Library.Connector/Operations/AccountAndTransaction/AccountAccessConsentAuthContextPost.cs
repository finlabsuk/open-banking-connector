// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using AccountAccessConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.
    AccountAccessConsentAuthContext;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using AccountAccessConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.
    AccountAccessConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction;

internal class
    AccountAccessConsentAuthContextPost : LocalEntityCreate<
        AccountAccessConsentAuthContextPersisted,
        AccountAccessConsentAuthContextRequest,
        AccountAccessConsentAuthContextCreateResponse>
{
    protected readonly IDbReadOnlyEntityMethods<AccountAccessConsentPersisted> _accountAccessConsentMethods;
    private readonly IBankProfileService _bankProfileService;

    public AccountAccessConsentAuthContextPost(
        IDbReadWriteEntityMethods<AccountAccessConsentAuthContextPersisted>
            entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadOnlyEntityMethods<AccountAccessConsentPersisted> accountAccessConsentMethods,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient)
    {
        _accountAccessConsentMethods = accountAccessConsentMethods;
        _bankProfileService = bankProfileService;
    }

    protected override async Task<AccountAccessConsentAuthContextCreateResponse> AddEntity(
        AccountAccessConsentAuthContextRequest request,
        ITimeProvider timeProvider)
    {
        // Load relevant data objects
        AccountAccessConsentPersisted accountAccessConsent =
            _accountAccessConsentMethods
                .DbSetNoTracking
                .Include(o => o.BankRegistrationNavigation)
                .SingleOrDefault(x => x.Id == request.AccountAccessConsentId) ??
            throw new KeyNotFoundException(
                $"No record found for Account Access Consent with ID {request.AccountAccessConsentId}.");

        BankRegistration bankRegistration = accountAccessConsent.BankRegistrationNavigation;
        string authorizationEndpoint = bankRegistration.AuthorizationEndpoint;

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
        string redirectUri = processedSoftwareStatementProfile.GetRedirectUri(
            bankProfile.DefaultResponseMode,
            bankRegistration.DefaultFragmentRedirectUri,
            bankRegistration.DefaultQueryRedirectUri);

        // Create auth URL
        string consentAuthGetAudClaim =
            customBehaviour?.AccountAccessConsentAuthGet?.AudClaim ??
            issuerUrl;

        (string authUrl, string state, string nonce, string sessionId) = CreateAuthUrl.Create(
            accountAccessConsent.ExternalApiId,
            processedSoftwareStatementProfile.OBSealKey,
            bankRegistration,
            bankRegistration.ExternalApiId,
            customBehaviour?.AccountAccessConsentAuthGet,
            authorizationEndpoint,
            consentAuthGetAudClaim,
            supportsSca,
            redirectUri,
            "accounts",
            _instrumentationClient);

        // Create persisted entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new AccountAccessConsentAuthContextPersisted(
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
            request.AccountAccessConsentId);

        // Add entity
        await _entityMethods.AddAsync(entity);

        var response =
            new AccountAccessConsentAuthContextCreateResponse(
                entity.Id,
                entity.Created,
                entity.CreatedBy,
                entity.Reference,
                null,
                entity.AccountAccessConsentId,
                state,
                authUrl,
                sessionId);

        return response;
    }
}
