// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class AuthContextAuthResult :
        IObjectPost<AuthResult, DomesticPaymentConsentAuthContextResponse>
    {
        private readonly
            IDbReadWriteEntityMethods<AuthContext>
            _authContextMethods;

        private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly IReadOnlyRepository<ProcessedSoftwareStatementProfile> _softwareStatementProfileRepo;
        private readonly ITimeProvider _timeProvider;


        public AuthContextAuthResult(
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadWriteEntityMethods<AuthContext>
                authContextMethods,
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient)
        {
            _dbSaveChangesMethod = dbSaveChangesMethod;
            _timeProvider = timeProvider;
            _authContextMethods = authContextMethods;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _instrumentationClient = instrumentationClient;
        }

        public async
            Task<(DomesticPaymentConsentAuthContextResponse response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> PostAsync(
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
                        o => ((DomesticPaymentConsentAuthContext) o).DomesticPaymentConsentNavigation
                            .BankRegistrationNavigation)
                    .Include(
                        o => ((DomesticVrpConsentAuthContext) o).DomesticVrpConsentNavigation
                            .BankRegistrationNavigation)
                    .SingleOrDefault(x => x.Id == authContextId) ??
                throw new KeyNotFoundException($"No record found for Auth Context with ID {authContextId}.");

            // Checks
            if (!(authContext.TokenEndpointResponse.Data is null))
            {
                throw new InvalidOperationException("Auth context already has token so aborting.");
            }

            BankRegistration bankRegistration = authContext switch
            {
                DomesticPaymentConsentAuthContext ac => ac.DomesticPaymentConsentNavigation.BankRegistrationNavigation,
                DomesticVrpConsentAuthContext ac => ac.DomesticVrpConsentNavigation.BankRegistrationNavigation,
                _ => throw new ArgumentOutOfRangeException()
            };
            string softwareStatementProfileId = bankRegistration.SoftwareStatementProfileId;
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(softwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfile with ID {softwareStatementProfileId}.");

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

            // Update auth context with token
            authContext.TokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                tokenEndpointResponse,
                _timeProvider,
                createdBy);

            // Create response (may involve additional processing based on entity)
            DomesticPaymentConsentAuthContextResponse response =
                ((DomesticPaymentConsentAuthContext) authContext).PublicGetResponse;

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (response, nonErrorMessages);
        }
    }
}
