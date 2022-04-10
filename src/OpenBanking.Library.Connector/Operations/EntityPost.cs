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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
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
    /// <summary>
    ///     Create operations on entities (objects stored in external (i.e. bank) database and local database).
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicPostResponse"></typeparam>
    /// <typeparam name="TApiRequest"></typeparam>
    /// <typeparam name="TApiResponse"></typeparam>
    internal abstract class
        EntityPost<TEntity, TPublicRequest, TPublicPostResponse, TApiRequest, TApiResponse> :
            PostBase<TPublicRequest, TPublicPostResponse>
        where TEntity : class, IEntity
        where TPublicRequest : Base
        where TApiResponse : class, ISupportsValidation
        where TApiRequest : class, ISupportsValidation
    {
        protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;
        protected readonly IApiVariantMapper _mapper;

        public EntityPost(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _mapper = mapper;
            _entityMethods = entityMethods;
        }

        protected async
            Task<(TApiResponse apiResponse, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            EntityPostCommon(
                PostRequestInfo requestInfo,
                TApiRequest apiRequest,
                IApiPostRequests<TApiRequest, TApiResponse> apiRequests,
                IApiClient apiClient,
                Uri uri,
                JsonSerializerSettings? requestJsonSerializerSettings,
                JsonSerializerSettings? responseJsonSerializerSettings,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)
        {
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


            return (apiResponse, nonErrorMessages);
        }
    }
}
