// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using AccountAccessConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.
    AccountAccessConsentAuthContext;
using AccountAccessConsentPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.AccountAccessConsent;
using AccountAccessConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction.
    AccountAccessConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.AccountAndTransaction
{
    internal class
        AccountAccessConsentAuthContextPost : LocalEntityPost<
            AccountAccessConsentAuthContextPersisted,
            AccountAccessConsentAuthContextRequest,
            AccountAccessConsentAuthContextCreateLocalResponse>
    {
        protected readonly IDbReadOnlyEntityMethods<AccountAccessConsentPersisted> _accountAccessConsentMethods;

        public AccountAccessConsentAuthContextPost(
            IDbReadWriteEntityMethods<AccountAccessConsentAuthContextPersisted>
                entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IDbReadOnlyEntityMethods<AccountAccessConsentPersisted> accountAccessConsentMethods,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _accountAccessConsentMethods = accountAccessConsentMethods;
        }

        protected override async Task<AccountAccessConsentAuthContextCreateLocalResponse> AddEntity(
            AccountAccessConsentAuthContextRequest request,
            ITimeProvider timeProvider)
        {
            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new AccountAccessConsentAuthContextPersisted(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                request.AccountAccessConsentId);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Load relevant data objects
            AccountAccessConsentPersisted accountAccessConsent =
                _accountAccessConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefault(x => x.Id == entity.AccountAccessConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Account Access Consent with ID {entity.AccountAccessConsentId}.");

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    accountAccessConsent.BankRegistrationNavigation.SoftwareStatementProfileId,
                    accountAccessConsent.BankRegistrationNavigation
                        .SoftwareStatementAndCertificateProfileOverrideCase);

            // Create auth URL
            var state = entity.Id.ToString();
            string authUrl = CreateAuthUrl.Create(
                accountAccessConsent.ExternalApiId,
                processedSoftwareStatementProfile,
                accountAccessConsent.BankRegistrationNavigation.ExternalApiObject.ExternalApiId,
                accountAccessConsent.BankRegistrationNavigation.CustomBehaviour?.AccountAccessConsentAuthGet,
                accountAccessConsent.BankRegistrationNavigation.AuthorizationEndpoint,
                accountAccessConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl,
                state,
                "accounts",
                _instrumentationClient);
            var response =
                new AccountAccessConsentAuthContextCreateLocalResponse(
                    entity.Id,
                    entity.Created,
                    entity.CreatedBy,
                    entity.Reference,
                    entity.AccountAccessConsentId,
                    authUrl);

            return response;
        }
    }
}
