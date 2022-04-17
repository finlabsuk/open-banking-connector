// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
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
        IObjectCreate<AuthResult, AuthContextResponse>
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
            Task<(AuthContextResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> CreateAsync(
                AuthResult request,
                string? createdBy = null,
                string? apiRequestWriteFile = null,
                string? apiResponseWriteFile = null,
                string? apiResponseOverrideFile = null)
        {
            request.ArgNotNull(nameof(request));

            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data
            var authContextId = new Guid(request.State);
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
                    .SingleOrDefault(x => x.Id == authContextId) ??
                throw new KeyNotFoundException($"No record found for Auth Context with ID {authContextId}.");

            // Get consent
            BaseConsent consent = authContext switch
            {
                AccountAccessConsentAuthContext ac => ac.AccountAccessConsentNavigation,
                DomesticPaymentConsentAuthContext ac => ac.DomesticPaymentConsentNavigation,
                DomesticVrpConsentAuthContext ac => ac.DomesticVrpConsentNavigation,
                _ => throw new ArgumentOutOfRangeException()
            };

            // Checks
            if (consent.AccessToken is not null)
            {
                throw new InvalidOperationException("Token already supplied for auth context so aborting.");
            }
            const int authContextExpiryIntervalInSeconds = 3600;
            var authContextExpiryTime = authContext.Created
                .AddSeconds(authContextExpiryIntervalInSeconds);
            if (_timeProvider.GetUtcNow() > authContextExpiryTime)
            {
                throw new InvalidOperationException("Auth context exists but now expired so will not process redirect.");
            }

            BankRegistration bankRegistration = authContext switch
            {
                AccountAccessConsentAuthContext ac => ac.AccountAccessConsentNavigation.BankRegistrationNavigation,
                DomesticPaymentConsentAuthContext ac => ac.DomesticPaymentConsentNavigation.BankRegistrationNavigation,
                DomesticVrpConsentAuthContext ac => ac.DomesticVrpConsentNavigation.BankRegistrationNavigation,
                _ => throw new ArgumentOutOfRangeException()
            };
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);

            // Obtain token for consent
            string redirectUrl = processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;
            JsonSerializerSettings? jsonSerializerSettings = null;
            TokenEndpointResponse tokenEndpointResponse =
                await PostTokenRequest.PostAuthCodeGrantAsync(
                    request.Code,
                    redirectUrl,
                    bankRegistration,
                    jsonSerializerSettings,
                    processedSoftwareStatementProfile.ApiClient);

            // Check token endpoint response
            bool isBearerTokenType = string.Equals(
                tokenEndpointResponse.TokenType,
                "bearer",
                StringComparison.OrdinalIgnoreCase);
            if (!isBearerTokenType)
            {
                throw new InvalidDataException(
                    "Access token received does not have token type equal to Bearer or bearer.");
            }

            // Update auth context with token
            consent.UpdateAccessToken(
                tokenEndpointResponse.AccessToken,
                tokenEndpointResponse.ExpiresIn,
                tokenEndpointResponse.RefreshToken,
                _timeProvider.GetUtcNow(),
                createdBy);
            
            // Delete auth context
            authContext.UpdateIsDeleted(true, _timeProvider.GetUtcNow(), createdBy);

            // Create response (may involve additional processing based on entity)
            var response =
                new AuthContextResponse(
                    authContext.Id,
                    authContext.Name,
                    authContext.Created,
                    authContext.CreatedBy);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (response, nonErrorMessages);
        }
    }
}
