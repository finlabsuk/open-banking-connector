// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using BankRegistrationPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal abstract class
        ConsentRead<TEntity, TPublicResponse, TApiResponse, TReadParams> : BaseRead<TEntity, TPublicResponse,
            TReadParams>
        where TEntity : class, IEntity
        where TApiResponse : class, ISupportsValidation
        where TReadParams : LocalReadParams
    {
        private readonly IGrantPost _grantPost;
        private readonly IApiVariantMapper _mapper;

        public ConsentRead(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IGrantPost grantPost) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _mapper = mapper;
            _grantPost = grantPost;
        }

        protected abstract string ClientCredentialsGrantScope { get; }

        protected abstract IApiGetRequests<TApiResponse> ApiRequests(
            BankApiSet2 bankApiSet,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient);

        protected override async
            Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGet(TReadParams readParams)
        {
            (Uri endpointUrl,
                    TEntity persistedObject,
                    BankApiSet2 bankApiInformation,
                    BankRegistrationPersisted bankRegistration,
                    string bankFinancialId,
                    string? accessToken,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await ApiGetRequestData(readParams.Id, readParams.ModifiedBy);

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementProfileOverride);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get client credentials grant token if necessary
            string accessTokenNew =
                accessToken ??
                (await _grantPost.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    bankRegistration.BankNavigation.TokenEndpoint,
                    null,
                    apiClient,
                    _instrumentationClient))
                .AccessToken;

            // Create new Open Banking object by posting JWT
            JsonSerializerSettings? jsonSerializerSettings = null;

            IApiGetRequests<TApiResponse> apiRequests = ApiRequests(
                bankApiInformation,
                bankFinancialId,
                accessTokenNew,
                processedSoftwareStatementProfile,
                _instrumentationClient);

            TApiResponse apiResponse;
            IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;
            (apiResponse, newNonErrorMessages) =
                await apiRequests.GetAsync(
                    endpointUrl,
                    jsonSerializerSettings,
                    apiClient,
                    _mapper);
            nonErrorMessages.AddRange(newNonErrorMessages);

            // Create response
            TPublicResponse response = GetReadResponse(persistedObject, apiResponse);

            return (response, nonErrorMessages);
        }

        protected abstract TPublicResponse GetReadResponse(
            TEntity persistedObject,
            TApiResponse apiResponse);

        protected abstract Task<(
                Uri endpointUrl,
                TEntity persistedObject,
                BankApiSet2 bankApiInformation,
                BankRegistrationPersisted bankRegistration,
                string bankFinancialId,
                string? accessToken,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGetRequestData(Guid id, string? modifiedBy);
    }
}
