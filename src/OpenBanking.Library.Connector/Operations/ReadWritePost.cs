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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using BankApiSetPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiSet;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal abstract class
        ReadWritePost<TEntity, TPublicRequest, TPublicResponse, TApiRequest, TApiResponse> : EntityPost<
            TEntity,
            TPublicRequest, TPublicResponse, TApiRequest, TApiResponse>
        where TEntity : class, IEntity,
        ISupportsFluentReadWritePost<TPublicRequest, TPublicResponse, TApiRequest, TApiResponse, TEntity>, new()
        where TPublicRequest : Base
        where TApiRequest : class, ISupportsValidation
        where TApiResponse : class, ISupportsValidation
    {
        public ReadWritePost(
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
            instrumentationClient,
            mapper) { }

        protected abstract string RelativePath { get; }

        protected override async
            Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPost(PostRequestInfo requestInfo)
        {
            (
                    TApiRequest apiRequest,
                    BankApiSetPersisted bankApiInformation,
                    BankRegistrationPersisted bankRegistration,
                    string bankFinancialId,
                    TokenEndpointResponse? userTokenEndpointResponse,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await ApiPostRequestData(requestInfo.Request);

            // Check API specified and get base URL
            string baseUrl = new TEntity().GetReadWriteApiType() switch
            {
                ReadWriteApiType.PaymentInitiation =>
                    bankApiInformation.PaymentInitiationApi?.BaseUrl ??
                    throw new NullReferenceException("Bank API Set has null Payment Initiation API."),
                ReadWriteApiType.VariableRecurringPayments =>
                    bankApiInformation.VariableRecurringPaymentsApi?.BaseUrl ??
                    throw new NullReferenceException("Bank API Set has null Variable Recurring Payments API."),
                _ => throw new ArgumentOutOfRangeException()
            };

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get client credentials grant token if necessary
            TokenEndpointResponse tokenEndpointResponse =
                userTokenEndpointResponse ??
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    "payments",
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    null,
                    apiClient,
                    _instrumentationClient);

            // Create new Open Banking object by posting JWT
            var uri = new Uri(baseUrl + RelativePath);
            JsonSerializerSettings? requestJsonSerializerSettings = null;
            JsonSerializerSettings? responseJsonSerializerSettings = null;

            IApiPostRequests<TApiRequest, TApiResponse> apiRequests = new TEntity().ApiPostRequests(
                bankApiInformation.PaymentInitiationApi,
                bankApiInformation.VariableRecurringPaymentsApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                _instrumentationClient);


            (TApiResponse apiResponse, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages2) =
                await EntityPostCommon(
                    requestInfo,
                    apiRequest,
                    apiRequests,
                    apiClient,
                    uri,
                    requestJsonSerializerSettings,
                    responseJsonSerializerSettings,
                    nonErrorMessages);

            // Create persisted entity
            var persistedObject = new TEntity();
            persistedObject.Initialise(
                requestInfo.Request,
                requestInfo.ModifiedBy,
                _timeProvider);

            persistedObject.Create(
                requestInfo.Request,
                requestInfo.ModifiedBy,
                _timeProvider,
                apiRequest);
            
            persistedObject.UpdateBeforeApiPost(apiRequest);
            
            // Update with results of POST
            persistedObject.UpdateAfterApiPost(apiResponse, "", _timeProvider);

            return (persistedObject, nonErrorMessages2);
        }

        protected abstract
            Task<(
                TApiRequest apiRequest,
                BankApiSetPersisted bankApiInformation,
                BankRegistrationPersisted bankRegistration,
                string bankFinancialId,
                TokenEndpointResponse? userTokenEndpointResponse,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiPostRequestData(TPublicRequest request);
    }
}
