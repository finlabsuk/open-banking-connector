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

        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IProcessedSoftwareStatementProfileStore _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;


        public AuthContextUpdate(
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadWriteEntityMethods<AuthContext>
                authContextMethods,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _authContextMethods = authContextMethods;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
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

            // Load relevant data
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

            // Get consent info
            (ConsentType consentType, BaseConsent consent, Guid consentId, BankRegistration bankRegistration) =
                authContext switch
                {
                    AccountAccessConsentAuthContext ac => (
                        ConsentType.AccountAccessConsent,
                        ac.AccountAccessConsentNavigation,
                        ac.AccountAccessConsentId,
                        ac.AccountAccessConsentNavigation.BankRegistrationNavigation),
                    DomesticPaymentConsentAuthContext ac => (
                        ConsentType.DomesticPaymentConsent,
                        (BaseConsent) ac.DomesticPaymentConsentNavigation,
                        ac.DomesticPaymentConsentId,
                        ac.DomesticPaymentConsentNavigation.BankRegistrationNavigation),
                    DomesticVrpConsentAuthContext ac => (
                        ConsentType.DomesticVrpConsent,
                        (BaseConsent) ac.DomesticVrpConsentNavigation,
                        ac.DomesticVrpConsentId,
                        ac.DomesticVrpConsentNavigation.BankRegistrationNavigation),
                    _ => throw new ArgumentOutOfRangeException()
                };

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
            TokenEndpointResponse tokenEndpointResponse =
                await PostTokenRequest.PostAuthCodeGrantAsync(
                    request.RedirectData.Code,
                    redirectUrl,
                    bankRegistration,
                    jsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient);

            // Update auth context with token
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
