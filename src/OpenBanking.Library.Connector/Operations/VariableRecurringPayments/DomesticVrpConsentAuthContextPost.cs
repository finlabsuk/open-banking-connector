// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
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
    protected readonly IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> _domesticVrpConsentMethods;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;

    public DomesticVrpConsentAuthContextPost(
        IDbReadWriteEntityMethods<DomesticVrpConsentAuthContextPersisted>
            entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadOnlyEntityMethods<DomesticVrpConsentPersisted> domesticVrpConsentMethods,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObSealCertificateMethods obSealCertificateMethods) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        instrumentationClient)
    {
        _domesticVrpConsentMethods = domesticVrpConsentMethods;
        _bankProfileService = bankProfileService;
        _obSealCertificateMethods = obSealCertificateMethods;
    }

    protected override async Task<DomesticVrpConsentAuthContextCreateResponse> AddEntity(
        DomesticVrpConsentAuthContextRequest request,
        ITimeProvider timeProvider)
    {
        // Load relevant data objects
        DomesticVrpConsentPersisted domesticVrpConsent =
            _domesticVrpConsentMethods
                .DbSetNoTracking
                .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
                .SingleOrDefault(x => x.Id == request.DomesticVrpConsentId) ??
            throw new KeyNotFoundException(
                $"No record found for Domestic Payment Consent with ID {request.DomesticVrpConsentId}.");
        BankRegistrationEntity bankRegistration = domesticVrpConsent.BankRegistrationNavigation;
        string authorizationEndpoint =
            bankRegistration.AuthorizationEndpoint;
        SoftwareStatementEntity softwareStatement = bankRegistration.SoftwareStatementNavigation!;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;
        OAuth2ResponseType responseType = bankProfile.DefaultResponseType;

        string redirectUri = softwareStatement.GetRedirectUri(
            bankRegistration.DefaultResponseModeOverride ?? bankProfile.DefaultResponseMode,
            bankRegistration.DefaultFragmentRedirectUri,
            bankRegistration.DefaultQueryRedirectUri);

        // Get OBSeal key
        OBSealKey obSealKey =
            (await _obSealCertificateMethods.GetValue(softwareStatement.DefaultObSealCertificateId)).ObSealKey;

        // Create auth URL
        string consentAuthGetAudClaim =
            customBehaviour?.DomesticVrpConsentAuthGet?.AudClaim ??
            issuerUrl;

        string scope = customBehaviour?.DomesticVrpConsentAuthGet?.Scope ?? "payments";

        (string authUrl, string state, string nonce, string? codeVerifier, string sessionId) = CreateAuthUrl.Create(
            domesticVrpConsent.ExternalApiId,
            obSealKey,
            bankRegistration.ExternalApiId,
            bankProfile.UseOpenIdConnect,
            customBehaviour?.DomesticVrpConsentAuthGet,
            authorizationEndpoint,
            consentAuthGetAudClaim,
            supportsSca,
            redirectUri,
            scope,
            responseType,
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
            codeVerifier,
            sessionId,
            request.DomesticVrpConsentId);

        // Add entity
        await _entityMethods.AddAsync(entity);

        var response =
            new DomesticVrpConsentAuthContextCreateResponse
            {
                Id = entity.Id,
                Created = entity.Created,
                CreatedBy = entity.CreatedBy,
                Reference = entity.Reference,
                State = state,
                DomesticVrpConsentId = entity.DomesticVrpConsentId,
                AuthUrl = authUrl,
                AppSessionId = sessionId
            };

        return response;
    }
}
