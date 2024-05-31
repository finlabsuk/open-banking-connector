// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
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
    private readonly ObSealCertificateMethods _obSealCertificateMethods;

    public AccountAccessConsentAuthContextPost(
        IDbReadWriteEntityMethods<AccountAccessConsentAuthContextPersisted>
            entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadOnlyEntityMethods<AccountAccessConsentPersisted> accountAccessConsentMethods,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObSealCertificateMethods obSealCertificateMethods) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        instrumentationClient)
    {
        _accountAccessConsentMethods = accountAccessConsentMethods;
        _bankProfileService = bankProfileService;
        _obSealCertificateMethods = obSealCertificateMethods;
    }

    protected override async Task<AccountAccessConsentAuthContextCreateResponse> AddEntity(
        AccountAccessConsentAuthContextRequest request,
        ITimeProvider timeProvider)
    {
        // Load relevant data objects
        AccountAccessConsentPersisted accountAccessConsent =
            _accountAccessConsentMethods
                .DbSetNoTracking
                .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
                .SingleOrDefault(x => x.Id == request.AccountAccessConsentId) ??
            throw new KeyNotFoundException(
                $"No record found for Account Access Consent with ID {request.AccountAccessConsentId}.");
        BankRegistrationEntity bankRegistration = accountAccessConsent.BankRegistrationNavigation;
        string authorizationEndpoint = bankRegistration.AuthorizationEndpoint;
        SoftwareStatementEntity softwareStatement = bankRegistration.SoftwareStatementNavigation!;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

        string redirectUri = softwareStatement.GetRedirectUri(
            bankRegistration.DefaultResponseModeOverride ?? bankProfile.DefaultResponseMode,
            bankRegistration.DefaultFragmentRedirectUri,
            bankRegistration.DefaultQueryRedirectUri);

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Create auth URL
        string consentAuthGetAudClaim =
            customBehaviour?.AccountAccessConsentAuthGet?.AudClaim ??
            issuerUrl;

        (string authUrl, string state, string nonce, string? codeVerifier, string sessionId) = CreateAuthUrl.Create(
            accountAccessConsent.ExternalApiId,
            obSealKey,
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
            codeVerifier,
            sessionId,
            request.AccountAccessConsentId);

        // Add entity
        await _entityMethods.AddAsync(entity);

        var response =
            new AccountAccessConsentAuthContextCreateResponse
            {
                Id = entity.Id,
                Created = entity.Created,
                CreatedBy = entity.CreatedBy,
                Reference = entity.Reference,
                AccountAccessConsentId = entity.AccountAccessConsentId,
                State = state,
                AuthUrl = authUrl,
                AppSessionId = sessionId
            };

        return response;
    }
}
