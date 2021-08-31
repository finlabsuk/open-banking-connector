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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal abstract class
        EntityGet<TEntity, TPublicQuery, TPublicResponse, TApiResponse> :
            LocalEntityGet<TEntity, TPublicQuery, TPublicResponse>
        where TEntity : class, IEntity, ISupportsFluentEntityGet<TPublicResponse, TApiResponse>,
        new()
        where TApiResponse : class, ISupportsValidation
    {
        protected readonly IApiVariantMapper _mapper;

        public EntityGet(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent> domesticPaymentConsentMethods,
            IReadOnlyRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            domesticPaymentConsentMethods,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _mapper = mapper;
        }

        private static (string? readResonseFile, string? writeResponseFile) ProcessBankApiResponseFile(
            BankApiResponseFile? responseFile)
        {
            string? readResponseFile = null;
            string? writeResponseFile = null;
            if (!(responseFile is null))
            {
                if (responseFile.UseResponseFileAsApiResponse)
                {
                    readResponseFile = responseFile.ResponseFile;
                }
                else
                {
                    writeResponseFile = responseFile.ResponseFile;
                }
            }

            return (readResponseFile, writeResponseFile);
        }

        protected override async
            Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGet(GetRequestInfo requestInfo)
        {
            // Create persisted entity
            (var persistedObject, var apiRequests, var apiClient, var uri,
                    JsonSerializerSettings? jsonSerializerSettings, var nonErrorMessages) =
                await ApiGetData(requestInfo.Id);

            string? readResponseFile = requestInfo.ApiResponseOverrideFile;
            string? writeResponseFile = requestInfo.ApiResponseWriteFile;
            TApiResponse apiResponse;
            if (!(readResponseFile is null))
            {
                apiResponse = await DataFile.ReadFile<TApiResponse>(readResponseFile, jsonSerializerSettings);
            }
            else
            {
                IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages;

                (apiResponse, newNonErrorMessages) =
                    await apiRequests.GetAsync(
                        uri,
                        jsonSerializerSettings,
                        apiClient,
                        _mapper);

                nonErrorMessages.AddRange(newNonErrorMessages);
                if (!(writeResponseFile is null))
                {
                    await DataFile.WriteFile(apiResponse, writeResponseFile, jsonSerializerSettings);
                }
            }

            // Create and store persistent object
            persistedObject.UpdateAfterApiGet(apiResponse, "", _timeProvider);
            return (persistedObject, nonErrorMessages);
        }

        protected abstract Task<(
            TEntity persistedObject,
            IApiGetRequests<TApiResponse> apiRequests,
            IApiClient apiClient,
            Uri uri,
            JsonSerializerSettings? jsonSerializerSettings,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetData(Guid id);
    }
}
