// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
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
        ReadWritePost<TEntity, TPublicRequest, TPublicResponse, TApiRequest, TApiResponse> : EntityPost<
            TEntity,
            TPublicRequest, TPublicResponse, TApiRequest, TApiResponse>
        where TEntity : class, IEntity
        where TPublicRequest : ConsentRequestBase
        where TApiRequest : class, ISupportsValidation
        where TApiResponse : class, ISupportsValidation
    {
        private readonly IGrantPost _grantPost;

        public ReadWritePost(
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
            instrumentationClient,
            mapper)
        {
            _grantPost = grantPost;
        }

        protected abstract string ClientCredentialsGrantScope { get; }

        protected abstract Task<TPublicResponse> AddEntity(
            TPublicRequest request,
            TApiResponse? apiResponse,
            ITimeProvider timeProvider);

        protected abstract IApiPostRequests<TApiRequest, TApiResponse> ApiRequests(
            BankApiSet2 bankApiSet,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient);

        protected override async
            Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo)
        {
            (
                    TApiRequest apiRequest,
                    Uri endpointUrl,
                    BankApiSet2 bankApiInformation,
                    BankRegistrationPersisted bankRegistration,
                    string bankFinancialId,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await ApiPostRequestData(requestInfo.Request);

            TApiResponse? externalApiResponse = null;
            IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages2;
            if (requestInfo.Request.ExternalApiObject is null)
            {
                // Get software statement profile
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                    await _softwareStatementProfileRepo.GetAsync(
                        bankRegistration.SoftwareStatementProfileId,
                        bankRegistration.SoftwareStatementProfileOverride);
                IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

                // Get client credentials grant token
                string accessTokenNew =
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
                JsonSerializerSettings? requestJsonSerializerSettings = null;
                JsonSerializerSettings? responseJsonSerializerSettings = null;

                IApiPostRequests<TApiRequest, TApiResponse> apiRequests =
                    ApiRequests(
                        bankApiInformation,
                        bankFinancialId,
                        accessTokenNew,
                        processedSoftwareStatementProfile,
                        _instrumentationClient);

                (externalApiResponse, nonErrorMessages2) =
                    await EntityPostCommon(
                        requestInfo,
                        apiRequest,
                        apiRequests,
                        apiClient,
                        endpointUrl,
                        requestJsonSerializerSettings,
                        responseJsonSerializerSettings,
                        nonErrorMessages);
            }
            else
            {
                nonErrorMessages2 = nonErrorMessages;
            }

            // Create persisted entity and return response
            TPublicResponse response = await AddEntity(
                requestInfo.Request,
                externalApiResponse,
                _timeProvider);

            // Persist updates (this happens last so as not to happen if there are any previous errors)
            await _dbSaveChangesMethod.SaveChangesAsync();

            return (response, nonErrorMessages2);
        }

        protected abstract
            Task<(
                TApiRequest apiRequest,
                Uri endpointUrl,
                BankApiSet2 bankApiInformation,
                BankRegistrationPersisted bankRegistration,
                string bankFinancialId,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPostRequestData(TPublicRequest request);
    }
}
