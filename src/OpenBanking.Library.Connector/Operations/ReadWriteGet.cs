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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Newtonsoft.Json;
using BankApiInformationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankApiInformation;
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
            IDbReadOnlyEntityMethods<DomesticPaymentConsent> domesticPaymentConsentMethods,
            IReadOnlyRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            domesticPaymentConsentMethods,
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
            (var bankApiId, var persistedObject, var bankApiInformation, var bankRegistration,
                    var bankFinancialId, TokenEndpointResponse? userTokenEndpointResponse, var nonErrorMessages) =
                await ApiGetRequestData(id);

            // Get PISP API
            PaymentInitiationApi paymentInitiationApi =
                bankApiInformation.PaymentInitiationApi ??
                throw new NullReferenceException("Bank API Information record has null Payment Initiation API.");

            // Get software statement profile
            SoftwareStatementProfile softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankRegistration.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException(
                    $"No record found for SoftwareStatementProfile with ID {bankRegistration.SoftwareStatementProfileId}");
            IApiClient apiClient = softwareStatementProfile.ApiClient;

            // Get client credentials grant token if necessary
            TokenEndpointResponse tokenEndpointResponse =
                userTokenEndpointResponse ??
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    "payments",
                    bankRegistration,
                    null,
                    apiClient);

            // Create new Open Banking object by posting JWT
            Uri uri = new Uri(
                paymentInitiationApi.BaseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);
            JsonSerializerSettings? jsonSerializerSettings = null;

            IApiGetRequests<TApiResponse> apiRequests = new TEntity().ApiGetRequests(
                paymentInitiationApi,
                bankFinancialId,
                tokenEndpointResponse,
                softwareStatementProfile,
                _instrumentationClient);

            return (persistedObject, apiRequests, apiClient, uri, jsonSerializerSettings,
                nonErrorMessages);
        }

        protected abstract Task<(
                string bankApiId,
                TEntity persistedObject,
                BankApiInformationPersisted bankApiInformation,
                BankRegistrationPersisted bankRegistration,
                string bankFinancialId,
                TokenEndpointResponse? userTokenEndpointResponse,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGetRequestData(Guid id);
    }
}
