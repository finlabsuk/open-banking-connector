// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class BankRegistrationDelete : EntityDelete<BankRegistration>
    {
        public BankRegistrationDelete(
            IDbReadWriteEntityMethods<BankRegistration> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileRepo) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo) { }

        protected override async
            Task<(BankRegistration persistedObject, IApiClient apiClient, Uri uri, IDeleteRequestProcessor
                deleteRequestProcessor, List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiDeleteData(
                Guid id,
                bool useRegistrationAccessToken)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            BankRegistration persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Bank Registration with ID {id}.");

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(persistedObject.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfile with ID {persistedObject.SoftwareStatementProfileId}");
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            string bankApiId = persistedObject.ExternalApiId;

            Uri uri = new Uri(persistedObject.OpenIdConfiguration.RegistrationEndpoint.TrimEnd('/') + $"/{bankApiId}");

            // Get appropriate token
            TokenEndpointResponse tokenEndpointResponse;
            if (useRegistrationAccessToken)
            {
                string accessToken =
                    persistedObject.BankApiResponse.Data.RegistrationAccessToken ??
                    throw new InvalidOperationException("No registration access token available");
                tokenEndpointResponse = new TokenEndpointResponse
                {
                    AccessToken = accessToken,
                };
            }
            else
            {
                tokenEndpointResponse =
                    await PostTokenRequest.PostClientCredentialsGrantAsync(
                        null, // no scope - not clear this is correct yet...
                        persistedObject,
                        null,
                        apiClient);
            }

            IDeleteRequestProcessor deleteRequestProcessor = new ApiDeleteRequestProcessor(tokenEndpointResponse);

            return (persistedObject, apiClient, uri, deleteRequestProcessor, nonErrorMessages);
        }
    }
}
