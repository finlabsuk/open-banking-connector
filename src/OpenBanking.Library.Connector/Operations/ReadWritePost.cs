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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
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
        ISupportsFluentReadWritePost<TPublicRequest, TPublicResponse, TApiRequest, TApiResponse>, new()
        where TPublicRequest : Base
        where TApiRequest : class, ISupportsValidation
        where TApiResponse : class, ISupportsValidation
    {
        public ReadWritePost(
            IDbReadWriteEntityMethods<TEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<DomesticPaymentConsent> domesticPaymentConsentMethods,
            IReadOnlyRepository<ProcessedSoftwareStatementProfile> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            domesticPaymentConsentMethods,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper) { }

        protected abstract string RelativePath { get; }

        protected override async Task<(TEntity persistedObject, TApiRequest apiRequest,
            IApiPostRequests<TApiRequest, TApiResponse> apiRequests, IApiClient apiClient, Uri uri,
            JsonSerializerSettings?
            jsonSerializerSettings, List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiPostData(
            TPublicRequest request,
            string? modifiedBy)
        {
            var persistedObject = new TEntity();
            persistedObject.Initialise(
                request,
                modifiedBy,
                _timeProvider);

            (
                var apiRequest,
                var bankApiInformation,
                var bankRegistration,
                var bankFinancialId,
                TokenEndpointResponse? userTokenEndpointResponse,
                var nonErrorMessages) = await ApiPostRequestData(request);

            // Get PISP API
            PaymentInitiationApi paymentInitiationApi =
                bankApiInformation.PaymentInitiationApi ??
                throw new NullReferenceException("Bank API Information record has null Payment Initiation API.");

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfile with ID {bankRegistration.SoftwareStatementProfileId}");
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get client credentials grant token if necessary
            TokenEndpointResponse tokenEndpointResponse =
                userTokenEndpointResponse ??
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    "payments",
                    bankRegistration,
                    null,
                    apiClient);

            // Create new Open Banking object by posting JWT
            Uri uri = new Uri(paymentInitiationApi.BaseUrl + RelativePath);
            JsonSerializerSettings? jsonSerializerSettings = null;

            IApiPostRequests<TApiRequest, TApiResponse> apiRequests = new TEntity().ApiPostRequests(
                paymentInitiationApi,
                bankFinancialId,
                tokenEndpointResponse,
                processedSoftwareStatementProfile,
                _instrumentationClient);

            return (persistedObject, apiRequest, apiRequests, apiClient, uri, jsonSerializerSettings,
                nonErrorMessages);
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
