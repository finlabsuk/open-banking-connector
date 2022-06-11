// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class AuthContextUpdate :
        IObjectUpdate<AuthResult, AuthContextUpdateAuthResultResponse>
    {
        private readonly
            IDbReadWriteEntityMethods<AuthContext>
            _authContextMethods;

        private readonly IOpenIdConfigurationRead _configurationRead;
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
            IOpenIdConfigurationRead configurationRead,
            IGrantPost grantPost)
        {
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _authContextMethods = authContextMethods;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
            _configurationRead = configurationRead;
            _grantPost = grantPost;
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

            // Read auth context etc from DB
            var authContextId = new Guid(request.RedirectData.State);
            AuthContext authContext =
                _authContextMethods
                    .DbSet
                    .Include(
                        o => ((AccountAccessConsentAuthContext) o).AccountAccessConsentNavigation
                            .BankRegistrationNavigation.BankNavigation)
                    .Include(
                        o => ((DomesticPaymentConsentAuthContext) o).DomesticPaymentConsentNavigation
                            .BankRegistrationNavigation.BankNavigation)
                    .Include(
                        o => ((DomesticVrpConsentAuthContext) o).DomesticVrpConsentNavigation
                            .BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefault(x => x.Id == authContextId) ??
                throw new KeyNotFoundException($"No record found for Auth Context with ID {authContextId}.");
            string nonce = authContext.Nonce;

            // Get consent info
            (ConsentType consentType, Guid consentId, string externalApiConsentId, BaseConsent consent,
                    BankRegistration bankRegistration, string? requestObjectAudClaim) =
                authContext switch
                {
                    AccountAccessConsentAuthContext ac => (
                        ConsentType.AccountAccessConsent,
                        ac.AccountAccessConsentId,
                        ac.AccountAccessConsentNavigation.ExternalApiId,
                        ac.AccountAccessConsentNavigation,
                        ac.AccountAccessConsentNavigation.BankRegistrationNavigation,
                        ac.AccountAccessConsentNavigation.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                            ?.AccountAccessConsentAuthGet
                            ?.AudClaim),
                    DomesticPaymentConsentAuthContext ac => (
                        ConsentType.DomesticPaymentConsent,
                        ac.DomesticPaymentConsentId,
                        ac.DomesticPaymentConsentNavigation.ExternalApiId,
                        (BaseConsent) ac.DomesticPaymentConsentNavigation,
                        ac.DomesticPaymentConsentNavigation.BankRegistrationNavigation,
                        ac.DomesticPaymentConsentNavigation.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                            ?.DomesticPaymentConsentAuthGet
                            ?.AudClaim),
                    DomesticVrpConsentAuthContext ac => (
                        ConsentType.DomesticVrpConsent,
                        ac.DomesticVrpConsentId,
                        ac.DomesticVrpConsentNavigation.ExternalApiId,
                        (BaseConsent) ac.DomesticVrpConsentNavigation,
                        ac.DomesticVrpConsentNavigation.BankRegistrationNavigation,
                        ac.DomesticVrpConsentNavigation.BankRegistrationNavigation.BankNavigation.CustomBehaviour
                            ?.DomesticVrpConsentAuthGet
                            ?.AudClaim),
                    _ => throw new ArgumentOutOfRangeException()
                };
            string bankIssuerUrl =
                requestObjectAudClaim ??
                bankRegistration.BankNavigation.IssuerUrl ??
                throw new Exception("Cannot determine issuer URL for bank");
            string externalApiClientId = bankRegistration.ExternalApiObject.ExternalApiId;

            // Validate ID token
            bool doNotValidateIdToken =
                bankRegistration.BankNavigation.CustomBehaviour?.GrantPost?.DoNotValidateIdToken ?? false;
            if (doNotValidateIdToken is false)
            {
                string jwksUri = bankRegistration.BankNavigation.JwksUri;
                bool jwksGetResponseHasNoRootProperty =
                    bankRegistration.BankNavigation.CustomBehaviour?.JwksGet?.ResponseHasNoRootProperty ?? false;
                await _grantPost.ValidateIdTokenAuthEndpoint(
                    request.RedirectData,
                    jwksUri,
                    jwksGetResponseHasNoRootProperty,
                    bankIssuerUrl,
                    externalApiClientId,
                    externalApiConsentId,
                    nonce,
                    bankRegistration.BankNavigation.SupportsSca);
            }

            // if (request.RedirectData.Nonce is not null)
            // {
            //     throw new Exception("test");
            // }

            // Only accept redirects within 30 mins of auth context creation
            const int authContextExpiryIntervalInSeconds = 30 * 60;
            DateTimeOffset authContextExpiryTime = authContext.Created
                .AddSeconds(authContextExpiryIntervalInSeconds);
            if (_timeProvider.GetUtcNow() > authContextExpiryTime)
            {
                throw new InvalidOperationException(
                    "Auth context exists but now stale (more than 30 mins old) so will not process redirect. " +
                    "Please create a new auth context and authenticate again.");
            }

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);

            // Obtain token for consent
            string redirectUrl =
                processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;
            JsonSerializerSettings? jsonSerializerSettings = null;
            AuthCodeGrantResponse tokenEndpointResponse =
                await _grantPost.PostAuthCodeGrantAsync(
                    request.RedirectData.Code,
                    redirectUrl,
                    bankIssuerUrl,
                    externalApiClientId,
                    externalApiConsentId,
                    nonce,
                    bankRegistration,
                    jsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient);

            // Update consent with nonce, token
            consent.UpdateNonce(
                nonce,
                _timeProvider.GetUtcNow(),
                modifiedBy);
            consent.UpdateAccessToken(
                tokenEndpointResponse.AccessToken,
                tokenEndpointResponse.ExpiresIn,
                tokenEndpointResponse.RefreshToken,
                _timeProvider.GetUtcNow(),
                modifiedBy);

            // Delete auth context
            authContext.UpdateIsDeleted(true, _timeProvider.GetUtcNow(), modifiedBy);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            // Create response (may involve additional processing based on entity)
            var response =
                new AuthContextUpdateAuthResultResponse(
                    consentType,
                    consentId,
                    null);

            return (response, nonErrorMessages);
        }
    }
}
