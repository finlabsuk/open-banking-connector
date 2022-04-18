// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    internal class
        AccountAccessConsentPost : ReadWritePost<AccountAccessConsentPersisted,
            AccountAccessConsent,
            AccountAccessConsentReadResponse,
            AccountAndTransactionModelsPublic.OBReadConsent1,
            AccountAndTransactionModelsPublic.OBReadConsentResponse1>
    {
        private readonly IDbReadOnlyEntityMethods<AccountAndTransactionApiEntity> _bankApiSetMethods;
        private readonly IDbReadOnlyEntityMethods<BankRegistration> _bankRegistrationMethods;

        public AccountAccessConsentPost(
            IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IApiVariantMapper mapper,
            IDbReadOnlyEntityMethods<AccountAndTransactionApiEntity> bankApiSetMethods,
            IDbReadOnlyEntityMethods<BankRegistration> bankRegistrationMethods) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient,
            mapper)
        {
            _bankApiSetMethods = bankApiSetMethods;
            _bankRegistrationMethods = bankRegistrationMethods;
        }

        protected override string RelativePath => "/account-access-consents";

        protected override string ClientCredentialsGrantScope => "accounts";

        protected override async Task<AccountAccessConsentReadResponse> AddEntity(
            AccountAccessConsent request,
            AccountAndTransactionModelsPublic.OBReadConsent1 apiRequest,
            AccountAndTransactionModelsPublic.OBReadConsentResponse1 apiResponse,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var persistedObject = new AccountAccessConsentPersisted(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                createdBy,
                utcNow,
                createdBy,
                null,
                0,
                null,
                utcNow,
                createdBy,
                request.BankRegistrationId,
                request.AccountAndTransactionApiId,
                apiResponse.Data.ConsentId);

            // Save entity
            await _entityMethods.AddAsync(persistedObject);

            // Create response (may involve additional processing based on entity)
            var response =
                new AccountAccessConsentReadResponse(
                    persistedObject.Id,
                    persistedObject.Created,
                    persistedObject.CreatedBy,
                    persistedObject.BankRegistrationId,
                    persistedObject.AccountAndTransactionApiId,
                    persistedObject.ExternalApiId,
                    apiResponse);

            return response;
        }

        protected override
            IApiPostRequests<AccountAndTransactionModelsPublic.OBReadConsent1,
                AccountAndTransactionModelsPublic.OBReadConsentResponse1> ApiRequests(
                BankApiSet2 bankApiSet,
                string bankFinancialId,
                string accessToken,
                ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
                IInstrumentationClient instrumentationClient) =>
            bankApiSet.AccountAndTransactionApi?.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p9 => new ApiRequests<
                    AccountAndTransactionModelsPublic.OBReadConsent1,
                    AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                    AccountAndTransactionModelsPublic.OBReadConsent1,
                    AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken),
                    new AccountAndTransactionPostRequestProcessor<
                        AccountAndTransactionModelsPublic.OBReadConsent1>(
                        bankFinancialId,
                        accessToken,
                        instrumentationClient)),
                null => throw new NullReferenceException("No AISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {bankApiSet.AccountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };

        protected override async
            Task<(
                AccountAndTransactionModelsPublic.OBReadConsent1 apiRequest,
                Uri endpointUrl,
                BankApiSet2 bankApiInformation,
                BankRegistration bankRegistration,
                string bankFinancialId,
                string? accessToken,
                List<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages)> ApiPostRequestData(AccountAccessConsent request)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load relevant data and checks
            Guid bankRegistrationId = request.BankRegistrationId;
            BankRegistration bankRegistration =
                await _bankRegistrationMethods
                    .DbSetNoTracking
                    .Include(o => o.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == bankRegistrationId) ??
                throw new KeyNotFoundException(
                    $"No record found for BankRegistrationId {bankRegistrationId} specified by request.");
            string bankFinancialId = bankRegistration.BankNavigation.FinancialId;

            Guid accountAndTransactionApiId = request.AccountAndTransactionApiId;
            AccountAndTransactionApiEntity accountAndTransactionApiEntity =
                await _bankApiSetMethods
                    .DbSetNoTracking
                    .SingleOrDefaultAsync(x => x.Id == accountAndTransactionApiId) ??
                throw new KeyNotFoundException(
                    $"No record found for AccountAndTransactionApi {accountAndTransactionApiId} specified by request.");
            var bankApiSet2 = new BankApiSet2
            {
                AccountAndTransactionApi = new AccountAndTransactionApi
                {
                    AccountAndTransactionApiVersion = accountAndTransactionApiEntity.ApiVersion,
                    BaseUrl = accountAndTransactionApiEntity.BaseUrl
                }
            };

            if (accountAndTransactionApiEntity.BankId != bankRegistration.BankId)
            {
                throw new ArgumentException(
                    "Specified AccountAndTransactionApi and BankRegistration objects do not share same BankId.");
            }

            // Create request
            AccountAndTransactionModelsPublic.OBReadConsent1 apiRequest = request.ExternalApiRequest;

            // Determine endpoint URL
            string baseUrl = accountAndTransactionApiEntity.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePath);

            return (apiRequest, endpointUrl, bankApiSet2, bankRegistration, bankFinancialId, null,
                nonErrorMessages);
        }
    }
}
