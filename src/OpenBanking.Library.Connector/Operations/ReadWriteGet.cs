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
        ReadWriteGet<TEntity, TPublicResponse, TApiResponse> : EntityGet<TEntity, TPublicResponse, TApiResponse>
        where TEntity : class, IEntity
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

        protected abstract string ClientCredentialsGrantScope { get; }

        protected abstract IApiGetRequests<TApiResponse> ApiRequests(
            BankApiSetPersisted bankApiSet,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient);

        protected override async Task<(
            TEntity persistedObject,
            IApiGetRequests<TApiResponse> apiRequests,
            IApiClient apiClient,
            Uri uri,
            JsonSerializerSettings? jsonSerializerSettings,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetData(Guid id, string? modifiedBy)
        {
            (string bankApiId,
                    Uri endpointUrl,
                    TEntity persistedObject,
                    BankApiSetPersisted bankApiInformation,
                    BankRegistrationPersisted bankRegistration,
                    string bankFinancialId,
                    string? accessToken,
                    List<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await ApiGetRequestData(id, modifiedBy);

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Get client credentials grant token if necessary
            string accessTokenNew =
                accessToken ??
                (await PostTokenRequest.PostClientCredentialsGrantAsync(
                    ClientCredentialsGrantScope,
                    processedSoftwareStatementProfile,
                    bankRegistration,
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

            return (persistedObject, apiRequests, apiClient, endpointUrl, jsonSerializerSettings,
                nonErrorMessages);
        }

        protected abstract Task<(
                string bankApiId,
                Uri endpointUrl,
                TEntity persistedObject,
                BankApiSetPersisted bankApiInformation,
                BankRegistrationPersisted bankRegistration,
                string bankFinancialId,
                string? accessToken,
                List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            ApiGetRequestData(Guid id, string? modifiedBy);
    }
}
