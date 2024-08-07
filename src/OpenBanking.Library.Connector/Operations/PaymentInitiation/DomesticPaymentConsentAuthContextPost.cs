// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
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
    private readonly ObSealCertificateMethods _obSealCertificateMethods;

    public DomesticPaymentConsentAuthContextPost(
        IDbReadWriteEntityMethods<DomesticPaymentConsentAuthContextPersisted>
            entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadOnlyEntityMethods<DomesticPaymentConsentPersisted> domesticPaymentConsentMethods,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObSealCertificateMethods obSealCertificateMethods) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        instrumentationClient)
    {
        _domesticPaymentConsentMethods = domesticPaymentConsentMethods;
        _bankProfileService = bankProfileService;
        _obSealCertificateMethods = obSealCertificateMethods;
    }

    protected override async Task<DomesticPaymentConsentAuthContextCreateResponse> AddEntity(
        DomesticPaymentConsentAuthContextRequest request,
        ITimeProvider timeProvider)
    {
        // Load relevant data objects
        DomesticPaymentConsentPersisted domesticPaymentConsent =
            _domesticPaymentConsentMethods
                .DbSetNoTracking
                .Include(o => o.BankRegistrationNavigation.SoftwareStatementNavigation)
                .SingleOrDefault(x => x.Id == request.DomesticPaymentConsentId) ??
            throw new KeyNotFoundException(
                $"No record found for Domestic Payment Consent with ID {request.DomesticPaymentConsentId}.");
        BankRegistrationEntity bankRegistration = domesticPaymentConsent.BankRegistrationNavigation;
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
            customBehaviour?.DomesticPaymentConsentAuthGet?.AudClaim ??
            issuerUrl;

        string scope = customBehaviour?.DomesticPaymentConsentAuthGet?.Scope ?? "payments";

        (string authUrl, string state, string nonce, string? codeVerifier, string sessionId) = CreateAuthUrl.Create(
            domesticPaymentConsent.ExternalApiId,
            obSealKey,
            bankRegistration.ExternalApiId,
            bankProfile.UseOpenIdConnect,
            customBehaviour?.DomesticPaymentConsentAuthGet,
            authorizationEndpoint,
            consentAuthGetAudClaim,
            supportsSca,
            redirectUri,
            scope,
            responseType,
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
            codeVerifier,
            sessionId,
            request.DomesticPaymentConsentId);

        // Add entity
        await _entityMethods.AddAsync(entity);

        var response =
            new DomesticPaymentConsentAuthContextCreateResponse
            {
                Id = entity.Id,
                Created = entity.Created,
                CreatedBy = entity.CreatedBy,
                Reference = entity.Reference,
                State = state,
                DomesticPaymentConsentId = entity.DomesticPaymentConsentId,
                AuthUrl = authUrl,
                AppSessionId = sessionId
            };

        return response;
    }
}
