// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
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
using AccountAndTransactionModelsV3p1p7 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    internal class
        AccountAccessConsentGet : ReadWriteGet<AccountAccessConsentPersisted, AccountAccessConsentResponse,
            AccountAndTransactionModelsPublic.OBReadConsentResponse1>
    {
        public AccountAccessConsentGet(
            IDbReadWriteEntityMethods<AccountAccessConsentPersisted> entityMethods,
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

        protected override string RelativePathBeforeId => "/account-access-consents";

        protected override string ClientCredentialsGrantScope => "accounts";

        protected override IApiGetRequests<AccountAndTransactionModelsPublic.OBReadConsentResponse1> ApiRequests(
            BankApiSet2 bankApiSet,
            string bankFinancialId,
            string accessToken,
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            IInstrumentationClient instrumentationClient) =>
            bankApiSet.AccountAndTransactionApi?.AccountAndTransactionApiVersion switch
            {
                AccountAndTransactionApiVersion.Version3p1p7 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                    AccountAndTransactionModelsV3p1p7.OBReadConsentResponse1>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                AccountAndTransactionApiVersion.Version3p1p9 => new ApiGetRequests<
                    AccountAndTransactionModelsPublic.OBReadConsentResponse1,
                    AccountAndTransactionModelsPublic.OBReadConsentResponse1>(
                    new AccountAndTransactionGetRequestProcessor(bankFinancialId, accessToken)),
                null => throw new NullReferenceException("No AISP API specified for this bank."),
                _ => throw new ArgumentOutOfRangeException(
                    $"AISP API version {bankApiSet.AccountAndTransactionApi.AccountAndTransactionApiVersion} not supported.")
            };

        protected override async Task<(
            string bankApiId,
            Uri endpointUrl,
            AccountAccessConsentPersisted persistedObject,
            BankApiSet2 bankApiInformation,
            BankRegistration bankRegistration,
            string bankFinancialId,
            string? accessToken,
            List<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGetRequestData(Guid id, string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            // Load object
            AccountAccessConsentPersisted persistedObject =
                await _entityMethods
                    .DbSet
                    .Include(o => o.AccountAccessConsentAuthContextsNavigation)
                    .Include(o => o.AccountAndTransactionApiNavigation)
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefaultAsync(x => x.Id == id) ??
                throw new KeyNotFoundException($"No record found for Account Access Consent with ID {id}.");
            AccountAndTransactionApiEntity accountAndTransactionApi =
                persistedObject.AccountAndTransactionApiNavigation;
            var bankApiSet2 = new BankApiSet2
            {
                AccountAndTransactionApi = new AccountAndTransactionApi
                {
                    AccountAndTransactionApiVersion = accountAndTransactionApi.ApiVersion,
                    BaseUrl = accountAndTransactionApi.BaseUrl
                }
            };
            BankRegistration bankRegistration = persistedObject.BankRegistrationNavigation;
            string bankFinancialId = persistedObject.BankRegistrationNavigation.BankNavigation.FinancialId;

            string bankApiId = persistedObject.ExternalApiId;

            // Determine endpoint URL
            string baseUrl = accountAndTransactionApi.BaseUrl;
            var endpointUrl = new Uri(baseUrl + RelativePathBeforeId + $"/{bankApiId}" + RelativePathAfterId);

            return (bankApiId, endpointUrl, persistedObject, bankApiInformation: bankApiSet2, bankRegistration,
                bankFinancialId,
                null,
                nonErrorMessages);
        }

        protected override AccountAccessConsentResponse GetReadResponse(
            AccountAccessConsentPersisted persistedObject,
            AccountAndTransactionModelsPublic.OBReadConsentResponse1 apiResponse)
        {
            return new AccountAccessConsentResponse(
                persistedObject.Id,
                persistedObject.Created,
                persistedObject.CreatedBy,
                persistedObject.Reference,
                persistedObject.BankRegistrationId,
                persistedObject.AccountAndTransactionApiId,
                persistedObject.ExternalApiId,
                apiResponse,
                null);
        }
    }
}
