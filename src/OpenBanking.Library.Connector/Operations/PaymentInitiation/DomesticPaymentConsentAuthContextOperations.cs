// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation;

internal class
    DomesticPaymentConsentAuthContextOperations : IObjectCreate<DomesticPaymentConsentAuthContextRequest,
        DomesticPaymentConsentAuthContextCreateResponse, LocalCreateParams>,
    IObjectRead<DomesticPaymentConsentAuthContextReadResponse, LocalReadParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly IDbMethods _dbMethods;
    private readonly DomesticPaymentConsentCommon _domesticPaymentConsentCommon;
    private readonly IDbEntityMethods<DomesticPaymentConsentAuthContext> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public DomesticPaymentConsentAuthContextOperations(
        IDbEntityMethods<DomesticPaymentConsentAuthContext>
            entityMethods,
        IDbMethods dbMethods,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObSealCertificateMethods obSealCertificateMethods,
        DomesticPaymentConsentCommon domesticPaymentConsentCommon)
    {
        _entityMethods = entityMethods;
        _dbMethods = dbMethods;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _bankProfileService = bankProfileService;
        _obSealCertificateMethods = obSealCertificateMethods;
        _domesticPaymentConsentCommon = domesticPaymentConsentCommon;
    }

    public async Task<(DomesticPaymentConsentAuthContextCreateResponse response,
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        CreateAsync(
            DomesticPaymentConsentAuthContextRequest request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticPaymentConsent and related
        (DomesticPaymentConsentPersisted domesticPaymentConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? _) =
            await _domesticPaymentConsentCommon.GetDomesticPaymentConsent(request.DomesticPaymentConsentId, false);
        string authorizationEndpoint =
            bankRegistration.AuthorizationEndpoint;

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

        // Detect re-auth case
        bool authPreviouslySuccessfullyPerformed = domesticPaymentConsent.AuthPreviouslySucceessfullyPerformed();
        bool reAuthNotInitialAuth = authPreviouslySuccessfullyPerformed;
        if (reAuthNotInitialAuth)
        {
            throw new InvalidOperationException("Re-auth is not supported for domestic payment consents.");
        }

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
            reAuthNotInitialAuth,
            _instrumentationClient);

        // Create persisted entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();

        var entity = new DomesticPaymentConsentAuthContext(
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

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbMethods.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async Task<(DomesticPaymentConsentAuthContextReadResponse response,
        IList<IFluentResponseInfoOrWarningMessage>
        nonErrorMessages)> ReadAsync(LocalReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create persisted entity
        DomesticPaymentConsentAuthContext persistedObject;
        if (_dbMethods.DbProvider is not DbProvider.MongoDb)
        {
            persistedObject =
                await _entityMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
                throw new KeyNotFoundException(
                    $"No record found for  DomesticPaymentConsentAuthContext with ID {readParams.Id}.");
        }
        else
        {
            persistedObject =
                await _entityMethods
                    .DbSetNoTracking
                    .Where(
                        x =>
                            EF.Property<string>(x, "_t") == nameof(DomesticPaymentConsentAuthContext))
                    .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
                throw new KeyNotFoundException(
                    $"No record found for DomesticPaymentConsentAuthContext with ID {readParams.Id}.");
        }

        // Create response
        DomesticPaymentConsentAuthContextReadResponse response = persistedObject.PublicGetLocalResponse;

        return (response, nonErrorMessages);
    }
}
