// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    internal class AccountAccessConsentDelete : EntityDelete<AccountAccessConsent>
    {
        public AccountAccessConsentDelete(
            IDbReadWriteEntityMethods<AccountAccessConsent> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient) { }

        protected string RelativePathBeforeId => "/account-access-consents";

        protected string RelativePathAfterId => "";

        protected override async
            Task<(AccountAccessConsent persistedObject, IApiClient apiClient, Uri uri, IDeleteRequestProcessor
                deleteRequestProcessor, List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiDeleteData(
                Guid id,
                bool useRegistrationAccessToken)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            AccountAccessConsent persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankApiSetNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Account Access Consent with ID {id}.");
            BankApiSet bankApiSet = persistedObject.BankApiSetNavigation;
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankApiId = persistedObject.ExternalApiId;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            // Get software statement profile
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    bankRegistration.SoftwareStatementProfileId,
                    bankRegistration.SoftwareStatementAndCertificateProfileOverrideCase);
            IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

            // Determine endpoint URL
            string baseUrl =
                bankApiSet.AccountAndTransactionApi?.BaseUrl ??
                throw new NullReferenceException("Bank API Set has null Account and Transaction API.");
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            // Get client credentials grant token
            TokenEndpointResponse tokenEndpointResponse =
                await PostTokenRequest.PostClientCredentialsGrantAsync(
                    "accounts",
                    processedSoftwareStatementProfile,
                    bankRegistration,
                    null,
                    apiClient,
                    _instrumentationClient);
            IDeleteRequestProcessor deleteRequestProcessor =
                new ApiDeleteRequestProcessor(tokenEndpointResponse, bankFinancialId);

            return (persistedObject, apiClient, endpointUrl, deleteRequestProcessor, nonErrorMessages);
        }
    }
}
