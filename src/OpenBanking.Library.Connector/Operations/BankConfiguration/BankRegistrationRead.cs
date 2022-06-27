// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
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
    internal class
        BankRegistrationRead : BaseRead<BankRegistrationPersisted, BankRegistrationResponse, BankRegistrationReadParams>
    {
        private readonly IBankProfileDefinitions _bankProfileDefinitions;
        private readonly IGrantPost _grantPost;
        private readonly IApiVariantMapper _mapper;

        public BankRegistrationRead(
            IDbReadWriteEntityMethods<BankRegistrationPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IBankProfileDefinitions bankProfileDefinitions,
            IGrantPost grantPost) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _mapper = mapper;
            _bankProfileDefinitions = bankProfileDefinitions;
            _grantPost = grantPost;
        }

        protected override async
            Task<(BankRegistrationResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGet(BankRegistrationReadParams readParams)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            BankProfile? bankProfile = readParams.BankProfileEnum is not null
                ? _bankProfileDefinitions.GetBankProfile(readParams.BankProfileEnum.Value)
                : null;

            bool includeExternalApiOperationValue =
                readParams.IncludeExternalApiOperation ??
                bankProfile?.BankConfigurationApiSettings.UseReadEndpoint ??
                throw new ArgumentNullException(
                    null,
                    "includeExternalApiOperation specified as null and cannot be obtained using specified BankProfile (also null).");
            includeExternalApiOperationValue = false;

            // Load object
            BankRegistrationPersisted entity =
                await _entityMethods
                    .DbSetNoTracking
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistration with ID {readParams.ModifiedBy}.");
            CustomBehaviourClass? customBehaviour = entity.BankNavigation.CustomBehaviour;
            DynamicClientRegistrationApiVersion dynamicClientRegistrationApiVersion =
                entity.BankNavigation.DcrApiVersion;

            ClientRegistrationModelsPublic.OBClientRegistration1Response? apiResponse = null;
            if (includeExternalApiOperationValue)
            {
                bool useRegistrationAccessTokenValue =
                    readParams.UseRegistrationAccessToken ??
                    bankProfile?.BankConfigurationApiSettings.UseRegistrationAccessToken ??
                    throw new ArgumentNullException(
                        null,
                        "useRegistrationAccessToken specified as null and cannot be obtained using specified BankProfile (also null).");

                // Get software statement profile
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                    await _softwareStatementProfileRepo.GetAsync(
                        entity.SoftwareStatementProfileId,
                        entity.SoftwareStatementProfileOverride);
                IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

                // Determine endpoint URL
                string registrationEndpoint = entity.BankNavigation.RegistrationEndpoint;
                string bankApiId = entity.ExternalApiObject.ExternalApiId;
                var apiRequestUrl = new Uri(registrationEndpoint + $"/{bankApiId}");

                // Get client credentials grant token if necessary
                string accessToken = useRegistrationAccessTokenValue
                    ? entity.ExternalApiObject.RegistrationAccessToken ??
                      throw new InvalidOperationException("No registration access token available")
                    : (await _grantPost.PostClientCredentialsGrantAsync(
                        null,
                        processedSoftwareStatementProfile,
                        entity,
                        entity.BankNavigation.TokenEndpoint,
                        null,
                        apiClient,
                        _instrumentationClient))
                    .AccessToken;

                // Create new Open Banking object by posting JWT
                JsonSerializerSettings? responseJsonSerializerSettings = null;
                if (!(customBehaviour?.BankRegistrationPost is null))
                {
                    var optionsDict = new Dictionary<JsonConverterLabel, int>();

                    DateTimeOffsetConverter? clientIdIssuedAtClaimResponseJsonConverter = customBehaviour
                        .BankRegistrationPost
                        .ClientIdIssuedAtClaimResponseJsonConverter;
                    if (clientIdIssuedAtClaimResponseJsonConverter is not null)
                    {
                        optionsDict.Add(
                            JsonConverterLabel.DcrRegClientIdIssuedAt,
                            (int) clientIdIssuedAtClaimResponseJsonConverter);
                    }

                    DelimitedStringConverterOptions? scopeClaimJsonConverter = customBehaviour
                        .BankRegistrationPost
                        .ScopeClaimResponseJsonConverter;
                    if (scopeClaimJsonConverter is not null)
                    {
                        optionsDict.Add(JsonConverterLabel.DcrRegScope, (int) scopeClaimJsonConverter);
                    }

                    responseJsonSerializerSettings = new JsonSerializerSettings
                    {
                        Context = new StreamingContext(
                            StreamingContextStates.All,
                            optionsDict)
                    };
                }

                var irrelevantVar = false;
                IApiGetRequests<ClientRegistrationModelsPublic.OBClientRegistration1Response> apiRequests =
                    dynamicClientRegistrationApiVersion
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
                                nameof(dynamicClientRegistrationApiVersion),
                                dynamicClientRegistrationApiVersion,
                                null)
                        };

                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;

                (apiResponse, newNonErrorMessages) =
                    await apiRequests.GetAsync(
                        apiRequestUrl,
                        responseJsonSerializerSettings,
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

            BankRegistrationResponse response = new(
                entity.Id,
                entity.Created,
                entity.CreatedBy,
                entity.Reference,
                new ExternalApiObjectResponse(entity.ExternalApiObject.ExternalApiId),
                apiResponse,
                null,
                entity.SoftwareStatementProfileId,
                entity.SoftwareStatementProfileOverride,
                entity.TokenEndpointAuthMethod,
                entity.RegistrationScope,
                entity.BankId,
                entity.DefaultResponseMode);

            return (response, nonErrorMessages);
        }
    }
}
