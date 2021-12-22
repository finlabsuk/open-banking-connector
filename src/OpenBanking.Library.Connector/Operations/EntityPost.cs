// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Newtonsoft.Json;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal abstract class
        EntityPost<TEntity, TPublicRequest, TPublicPostResponse, TApiRequest, TApiResponse> :
            LocalEntityPost<TEntity, TPublicRequest, TPublicPostResponse>
        where TEntity : class, IEntity,
        ISupportsFluentEntityPost<TPublicRequest, TPublicPostResponse, TApiRequest, TApiResponse>,
        new()
        where TPublicRequest : Base
        where TApiResponse : class, ISupportsValidation
        where TApiRequest : class, ISupportsValidation
    {
        protected readonly IApiVariantMapper _mapper;

        public EntityPost(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
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
            Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo)
        {
            // Create persisted entity
            (TEntity persistedObject, TApiRequest apiRequest, IApiPostRequests<TApiRequest, TApiResponse> apiRequests,
                    IApiClient apiClient, Uri uri,
                    JsonSerializerSettings? requestJsonSerializerSettings,
                    JsonSerializerSettings? responseJsonSerializerSettings,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await ApiPostData(requestInfo.Request, requestInfo.ModifiedBy);
            persistedObject.UpdateBeforeApiPost(apiRequest);

            string? writeRequestFile = requestInfo.ApiRequestWriteFile;
            string? readResponseFile = requestInfo.ApiResponseOverrideFile;
            string? writeResponseFile = requestInfo.ApiResponseWriteFile;

            // Log request to file
            if (!(writeRequestFile is null))
            {
                await DataFile.WriteFile(apiRequest, writeRequestFile, requestJsonSerializerSettings);
            }

            // Either use API response override or call API
            TApiResponse apiResponse;
            if (!(readResponseFile is null))
            {
                apiResponse = await DataFile.ReadFile<TApiResponse>(readResponseFile, responseJsonSerializerSettings);
            }
            else
            {
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;

                (apiResponse, newNonErrorMessages) =
                    await apiRequests.PostAsync(
                        uri,
                        apiRequest,
                        requestJsonSerializerSettings,
                        responseJsonSerializerSettings,
                        apiClient,
                        _mapper);

                nonErrorMessages.AddRange(newNonErrorMessages);
            }

            // Log response to file
            if (!(writeResponseFile is null))
            {
                await DataFile.WriteFile(apiResponse, writeResponseFile, responseJsonSerializerSettings);
            }

            // Create and store persistent object
            persistedObject.UpdateAfterApiPost(apiResponse, "", _timeProvider);

            await _entityMethods.AddAsync(persistedObject);

            return (persistedObject, nonErrorMessages);
        }

        protected abstract Task<(TEntity persistedObject, TApiRequest apiRequest,
            IApiPostRequests<TApiRequest, TApiResponse> apiRequests, IApiClient apiClient, Uri uri,
            JsonSerializerSettings? requestJsonSerializerSettings,
            JsonSerializerSettings? responseJsonSerializerSettings, List<IFluentResponseInfoOrWarningMessage>
            nonErrorMessages)> ApiPostData(
            TPublicRequest request,
            string? modifiedBy);
    }
}
