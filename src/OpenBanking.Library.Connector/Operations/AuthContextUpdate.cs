// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BankRegistration =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class AuthContextUpdate :
    IObjectUpdate<AuthResult, AuthContextUpdateAuthResultResponse>
{
    private readonly
        IDbReadWriteEntityMethods<AuthContext>
        _authContextMethods;

    private readonly IBankProfileService _bankProfileService;
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    private readonly IGrantPost _grantPost;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
    private readonly ITimeProvider _timeProvider;

    public AuthContextUpdate(
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IDbReadWriteEntityMethods<AuthContext>
            authContextMethods,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient,
        IGrantPost grantPost,
        IBankProfileService bankProfileService)
    {
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _authContextMethods = authContextMethods;
        _softwareStatementProfileRepo = softwareStatementProfileRepo;
        _instrumentationClient = instrumentationClient;
        _grantPost = grantPost;
        _bankProfileService = bankProfileService;
    }

    public async
        Task<(AuthContextUpdateAuthResultResponse response, IList<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)>
        CreateAsync(
            AuthResult request,
            string? modifiedBy)
    {
        request.ArgNotNull(nameof(request));

        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        _instrumentationClient.Trace("Received ID token: " + request.RedirectData.IdToken);

        // Read auth context etc from DB
        string state = request.RedirectData.State;
        AuthContext authContext =
            _authContextMethods
                .DbSet
                .Include(
                    o => ((AccountAccessConsentAuthContext) o).AccountAccessConsentNavigation
                        .BankRegistrationNavigation)
                .Include(
                    o => ((DomesticPaymentConsentAuthContext) o).DomesticPaymentConsentNavigation
                        .BankRegistrationNavigation)
                .Include(
                    o => ((DomesticVrpConsentAuthContext) o).DomesticVrpConsentNavigation
                        .BankRegistrationNavigation)
                .SingleOrDefault(x => x.State == state) ??
            throw new KeyNotFoundException($"No record found for Auth Context with state {state}.");
        string currentAuthContextNonce = authContext.Nonce;

        // Get consent info
        (ConsentType consentType, Guid consentId, string externalApiConsentId, BaseConsent consent,
                BankRegistration bankRegistration, string? requestScope) =
            authContext switch
            {
                AccountAccessConsentAuthContext ac1 => (
                    ConsentType.AccountAccessConsent,
                    ac1.AccountAccessConsentId,
                    ac1.AccountAccessConsentNavigation.ExternalApiId,
                    ac1.AccountAccessConsentNavigation,
                    ac1.AccountAccessConsentNavigation.BankRegistrationNavigation,
                    "openid accounts"),
                DomesticPaymentConsentAuthContext ac2 => (
                    ConsentType.DomesticPaymentConsent,
                    ac2.DomesticPaymentConsentId,
                    ac2.DomesticPaymentConsentNavigation.ExternalApiId,
                    (BaseConsent) ac2.DomesticPaymentConsentNavigation,
                    ac2.DomesticPaymentConsentNavigation.BankRegistrationNavigation,
                    "openid payments"),
                DomesticVrpConsentAuthContext ac3 => (
                    ConsentType.DomesticVrpConsent,
                    ac3.DomesticVrpConsentId,
                    ac3.DomesticVrpConsentNavigation.ExternalApiId,
                    (BaseConsent) ac3.DomesticVrpConsentNavigation,
                    ac3.DomesticVrpConsentNavigation.BankRegistrationNavigation,
                    "openid payments"),
                _ => throw new ArgumentOutOfRangeException()
            };
        string tokenEndpoint = bankRegistration.TokenEndpoint;
        string externalApiClientId = bankRegistration.ExternalApiObject.ExternalApiId;

        // Get bank profile
        BankProfile bankProfile = _bankProfileService.GetBankProfile(bankRegistration.BankProfile);
        TokenEndpointAuthMethod tokenEndpointAuthMethod =
            bankProfile.BankConfigurationApiSettings.TokenEndpointAuthMethod;
        OAuth2ResponseMode defaultResponseMode = bankProfile.DefaultResponseMode;
        bool supportsSca = bankProfile.SupportsSca;
        string issuerUrl = bankProfile.IssuerUrl;
        IdTokenSubClaimType idTokenSubClaimType = bankProfile.BankConfigurationApiSettings.IdTokenSubClaimType;
        CustomBehaviourClass? customBehaviour = bankProfile.CustomBehaviour;

        // Only accept redirects within 10 mins of auth context (session) creation
        const int authContextExpiryIntervalInSeconds = 10 * 60;
        DateTimeOffset authContextExpiryTime = authContext.Created
            .AddSeconds(authContextExpiryIntervalInSeconds);
        if (_timeProvider.GetUtcNow() > authContextExpiryTime)
        {
            throw new InvalidOperationException(
                "Auth context exists but now stale (more than 10 mins old) so will not process redirect. " +
                "Please create a new auth context and authenticate again.");
        }

        // Determine nonce
        ConsentAuthGetCustomBehaviour? consentAuthGetCustomBehaviour = authContext switch
        {
            AccountAccessConsentAuthContext ac => customBehaviour?.AccountAccessConsentAuthGet,
            DomesticPaymentConsentAuthContext ac => customBehaviour?.DomesticPaymentConsentAuthGet,
            DomesticVrpConsentAuthContext ac => customBehaviour?.DomesticVrpConsentAuthGet,
            _ => throw new ArgumentOutOfRangeException()
        };
        bool nonceClaimIsInitialValue = consentAuthGetCustomBehaviour?.IdTokenNonceClaimIsPreviousValue ?? false;
        string nonce;
        if (nonceClaimIsInitialValue && consent.AuthContextNonce is not null)
        {
            nonce = consent.AuthContextNonce;
        }
        else
        {
            nonce = currentAuthContextNonce;
        }

        // Validate ID token including nonce
        string? requestObjectAudClaim = consentAuthGetCustomBehaviour?.AudClaim;
        string bankTokenIssuerClaim =
            requestObjectAudClaim ??
            issuerUrl;
        DateTimeOffset modified = _timeProvider.GetUtcNow();
        bool doNotValidateIdToken = consentAuthGetCustomBehaviour?.DoNotValidateIdToken ?? false;
        if (doNotValidateIdToken is false)
        {
            string jwksUri = bankRegistration.JwksUri;
            string? newExternalApiUserId = await _grantPost.ValidateIdTokenAuthEndpoint(
                request.RedirectData,
                consentAuthGetCustomBehaviour,
                jwksUri,
                customBehaviour?.JwksGet,
                bankTokenIssuerClaim,
                externalApiClientId,
                externalApiConsentId,
                nonce,
                supportsSca,
                idTokenSubClaimType,
                consent.ExternalApiUserId);
            if (newExternalApiUserId != consent.ExternalApiUserId)
            {
                consent.UpdateExternalApiUserId(
                    newExternalApiUserId,
                    modified,
                    modifiedBy);
            }
        }

        // Valid ID token means nonce has been validated so we delete auth context to ensure nonce can only be used once
        authContext.UpdateIsDeleted(true, modified, modifiedBy);

        // Wrap remaining processing in try block to ensure DB changes persisted
        try
        {
            // Validate redirect URL
            string redirectUrl = bankRegistration.DefaultRedirectUri;
            if (request.RedirectUrl is not null)
            {
                if (!string.Equals(request.RedirectUrl, redirectUrl))
                {
                    throw new Exception("Redirect URL supplied does not match that which was expected");
                }
            }

            // Validate response mode
            if (request.ResponseMode != defaultResponseMode)
            {
                throw new Exception("Response mode supplied does not match that which was expected");
            }

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);

            // Obtain token for consent
            JsonSerializerSettings? jsonSerializerSettings = null;
            TokenEndpointResponseAuthCodeGrant tokenEndpointResponse =
                await _grantPost.PostAuthCodeGrantAsync(
                    request.RedirectData.Code,
                    redirectUrl,
                    bankTokenIssuerClaim,
                    externalApiClientId,
                    externalApiConsentId,
                    consent.ExternalApiUserId,
                    nonce,
                    requestScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    tokenEndpointAuthMethod,
                    tokenEndpoint,
                    supportsSca,
                    idTokenSubClaimType,
                    jsonSerializerSettings,
                    customBehaviour?.AuthCodeGrantPost,
                    customBehaviour?.JwksGet,
                    processedSoftwareStatementProfile.ApiClient,
                    _instrumentationClient);

            // Update consent with nonce, token
            consent.UpdateAuthContext(
                authContext.State,
                nonce,
                modified,
                modifiedBy);
            consent.UpdateAccessToken(
                tokenEndpointResponse.AccessToken,
                //0,
                tokenEndpointResponse.ExpiresIn,
                tokenEndpointResponse.RefreshToken,
                modified,
                modifiedBy);

            // Create response (may involve additional processing based on entity)
            var response =
                new AuthContextUpdateAuthResultResponse(
                    consentType,
                    consentId,
                    null);

            return (response, nonErrorMessages);
        }
        catch
        {
            throw;
        }
        finally
        {
            await _dbSaveChangesMethod.SaveChangesAsync();
        }
    }
}
