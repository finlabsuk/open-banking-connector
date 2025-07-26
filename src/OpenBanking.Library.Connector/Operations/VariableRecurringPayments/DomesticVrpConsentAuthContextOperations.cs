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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.VariableRecurringPayments;

internal class
    DomesticVrpConsentAuthContextOperations : IObjectCreate<DomesticVrpConsentAuthContextRequest,
        DomesticVrpConsentAuthContextCreateResponse, LocalCreateParams>,
    IObjectRead<DomesticVrpConsentAuthContextReadResponse, LocalReadParams>
{
    private readonly IBankProfileService _bankProfileService;
    private readonly IDbMethods _dbSaveChangesMethod;
    private readonly DomesticVrpConsentCommon _domesticVrpConsentCommon;
    private readonly IDbEntityMethods<DomesticVrpConsentAuthContextPersisted> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly ObSealCertificateMethods _obSealCertificateMethods;
    private readonly ITimeProvider _timeProvider;

    public DomesticVrpConsentAuthContextOperations(
        IDbEntityMethods<DomesticVrpConsentAuthContextPersisted>
            entityMethods,
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IBankProfileService bankProfileService,
        ObSealCertificateMethods obSealCertificateMethods,
        DomesticVrpConsentCommon domesticVrpConsentCommon)
    {
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _bankProfileService = bankProfileService;
        _obSealCertificateMethods = obSealCertificateMethods;
        _domesticVrpConsentCommon = domesticVrpConsentCommon;
    }

    public async Task<(DomesticVrpConsentAuthContextCreateResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        CreateAsync(
            DomesticVrpConsentAuthContextRequest request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Load DomesticVrpConsent and related
        (DomesticVrpConsent domesticVrpConsent, BankRegistrationEntity bankRegistration,
                SoftwareStatementEntity softwareStatement, ExternalApiSecretEntity? _) =
            await _domesticVrpConsentCommon.GetDomesticVrpConsent(request.DomesticVrpConsentId, false);
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
            customBehaviour?.DomesticVrpConsentAuthGet?.AudClaim ??
            issuerUrl;

        string scope = customBehaviour?.DomesticVrpConsentAuthGet?.Scope ?? "payments";

        // Detect re-auth case
        bool authPreviouslySuccessfullyPerformed = domesticVrpConsent.AuthPreviouslySucceessfullyPerformed();
        bool reAuthNotInitialAuth = authPreviouslySuccessfullyPerformed;

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
            reAuthNotInitialAuth,
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

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    public async Task<(DomesticVrpConsentAuthContextReadResponse response, IList<IFluentResponseInfoOrWarningMessage>
        nonErrorMessages)> ReadAsync(LocalReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create persisted entity
        DomesticVrpConsentAuthContextPersisted persistedObject =
            await _entityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
            throw new KeyNotFoundException(
                $"No record found for DomesticVrpConsentAuthContext with ID {readParams.Id}.");

        // Create response
        DomesticVrpConsentAuthContextReadResponse response = persistedObject.PublicGetLocalResponse;

        return (response, nonErrorMessages);
    }
}
