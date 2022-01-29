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
        ReadWriteGet<TEntity, TPublicQuery, TPublicResponse, TApiResponse> : EntityGet<
            TEntity, TPublicQuery, TPublicResponse, TApiResponse>
        where TEntity : class, IEntity,
        ISupportsFluentReadWriteGet<TPublicResponse, TApiResponse>, new()
        where TApiResponse : class, ISupportsValidation
    {
        public ReadWriteGet(
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

        protected abstract string RelativePathBeforeId { get; }

        protected virtual string RelativePathAfterId => "";

        protected override async Task<(
            TEntity persistedObject,
            IApiGetRequests<TApiResponse> apiRequests,
            IApiClient apiClient,
            Uri uri,
            JsonSerializerSettings? jsonSerializerSettings,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetData(Guid id)
        {
            (string bankApiId, TEntity persistedObject, BankApiSetPersisted bankApiInformation,
                    BankRegistrationPersisted bankRegistration,
                    string bankFinancialId, TokenEndpointResponse? userTokenEndpointResponse,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await ApiGetRequestData(id);

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
            var uri = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);
            JsonSerializerSettings? jsonSerializerSettings = null;

            IApiGetRequests<TApiResponse> apiRequests = new TEntity().ApiGetRequests(
                bankApiInformation.PaymentInitiationApi,
                bankApiInformation.VariableRecurringPaymentsApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                _instrumentationClient);

            return (persistedObject, apiRequests, apiClient, uri, jsonSerializerSettings,
                nonErrorMessages);
        }

        protected abstract Task<(
                string bankApiId,
                TEntity persistedObject,
                BankApiSetPersisted bankApiInformation,
                BankRegistrationPersisted bankRegistration,
                string bankFinancialId,
                TokenEndpointResponse? userTokenEndpointResponse,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGetRequestData(Guid id);
    }
}
