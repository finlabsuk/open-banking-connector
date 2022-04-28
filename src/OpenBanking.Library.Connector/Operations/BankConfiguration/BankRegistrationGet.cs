// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using ClientRegistrationModelsV3p2 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p2.Models;
using ClientRegistrationModelsV3p1 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class BankRegistrationGet : GetBase<BankRegistrationPersisted, BankRegistrationReadResponse>
    {
        protected readonly IApiVariantMapper _mapper;


        public BankRegistrationGet(
            IDbReadWriteEntityMethods<BankRegistrationPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _mapper = mapper;
        }


        protected override async
            Task<(BankRegistrationReadResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGet(GetRequestInfo requestInfo)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            BankRegistrationPersisted entity =
                await _entityMethods
                    .DbSetNoTracking
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == requestInfo.Id) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistration with ID {requestInfo.ModifiedBy}.");

            var useExternalApiGet = false;
            ClientRegistrationModelsPublic.OBClientRegistration1Response? apiResponse = null;
            if (useExternalApiGet)
            {
                // Determine endpoint URL
                string bankApiId = entity.ExternalApiObject.ExternalApiId;
                var endpointUrl = new Uri(entity.RegistrationEndpoint + $"/{bankApiId}");

                // Get software statement profile
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                    await _softwareStatementProfileRepo.GetAsync(
                        entity.SoftwareStatementProfileId,
                        entity.SoftwareStatementAndCertificateProfileOverrideCase);
                IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

                // Get client credentials grant token if necessary
                string accessToken = (await PostTokenRequest.PostClientCredentialsGrantAsync(
                        null,
                        processedSoftwareStatementProfile,
                        entity,
                        null,
                        apiClient,
                        _instrumentationClient))
                    .AccessToken;

                // Create new Open Banking object by posting JWT
                JsonSerializerSettings? jsonSerializerSettings = null;
                if (!(entity.CustomBehaviour?.BankRegistrationResponseJsonOptions is null))
                {
                    var optionsDict = new Dictionary<JsonConverterLabel, int>
                    {
                        {
                            JsonConverterLabel.DcrRegClientIdIssuedAt,
                            (int) entity.CustomBehaviour.BankRegistrationResponseJsonOptions
                                .ClientIdIssuedAtConverterOptions
                        },
                        {
                            JsonConverterLabel.DcrRegScope,
                            (int) entity.CustomBehaviour.BankRegistrationResponseJsonOptions.ScopeConverterOptions
                        }
                    };
                    jsonSerializerSettings = new JsonSerializerSettings
                    {
                        Context = new StreamingContext(
                            StreamingContextStates.All,
                            optionsDict)
                    };
                }

                var irrelevantVar = false;
                IApiGetRequests<ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                    entity.DynamicClientRegistrationApiVersion
                        switch
                        {
                            DynamicClientRegistrationApiVersion.Version3p1 =>
                                new ApiGetRequests<
                                    ClientRegistrationModelsPublic.OBClientRegistration1Response,
                                    ClientRegistrationModelsV3p1.OBClientRegistration1>(
                                    new JwtRequestProcessor<ClientRegistrationModelsV3p1.OBClientRegistration1>(
                                        processedSoftwareStatementProfile,
                                        _instrumentationClient,
                                        irrelevantVar)),
                            DynamicClientRegistrationApiVersion.Version3p2 =>
                                new ApiGetRequests<ClientRegistrationModelsPublic.OBClientRegistration1Response,
                                    ClientRegistrationModelsV3p2.OBClientRegistration1>(
                                    new JwtRequestProcessor<ClientRegistrationModelsV3p2.OBClientRegistration1>(
                                        processedSoftwareStatementProfile,
                                        _instrumentationClient,
                                        irrelevantVar)),
                            DynamicClientRegistrationApiVersion.Version3p3 =>
                                new ApiGetRequests<ClientRegistrationModelsPublic.OBClientRegistration1Response,
                                    ClientRegistrationModelsPublic.OBClientRegistration1Response>(
                                    new JwtRequestProcessor<ClientRegistrationModelsPublic.OBClientRegistration1>(
                                        processedSoftwareStatementProfile,
                                        _instrumentationClient,
                                        irrelevantVar)),
                            _ => throw new ArgumentOutOfRangeException(
                                nameof(entity.DynamicClientRegistrationApiVersion),
                                entity.DynamicClientRegistrationApiVersion,
                                null)
                        };

                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;

                (apiResponse, newNonErrorMessages) =
                    await apiRequests.GetAsync(
                        endpointUrl,
                        jsonSerializerSettings,
                        apiClient,
                        _mapper);

                nonErrorMessages.AddRange(newNonErrorMessages);
            }

            // Create response
            if (apiResponse is not null)
            {
                apiResponse.ClientSecret = null;
                apiResponse.RegistrationAccessToken = null;
            }

            BankRegistrationReadResponse response = new(
                entity.Id,
                entity.Created,
                entity.CreatedBy,
                entity.BankId,
                entity.SoftwareStatementProfileId,
                entity.SoftwareStatementAndCertificateProfileOverrideCase,
                entity.DynamicClientRegistrationApiVersion,
                entity.RegistrationScope,
                entity.RegistrationEndpoint,
                entity.TokenEndpoint,
                entity.AuthorizationEndpoint,
                entity.TokenEndpointAuthMethod,
                entity.CustomBehaviour,
                new ExternalApiObjectResponse(entity.ExternalApiObject.ExternalApiId),
                apiResponse);

            return (response, nonErrorMessages);
        }
    }
}
