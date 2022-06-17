// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.CustomBehaviour;
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
        AccountAccessConsentAuthContextPost : LocalEntityCreate<
            AccountAccessConsentAuthContextPersisted,
            AccountAccessConsentAuthContextRequest,
            AccountAccessConsentAuthContextCreateResponse>
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

        protected override async Task<AccountAccessConsentAuthContextCreateResponse> AddEntity(
            AccountAccessConsentAuthContextRequest request,
            ITimeProvider timeProvider)
        {
            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var nonce = Guid.NewGuid().ToString();
            var entity = new AccountAccessConsentAuthContextPersisted(
                Guid.NewGuid(),
                request.Reference,
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                nonce,
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
            CustomBehaviourClass? customBehaviour =
                accountAccessConsent.BankRegistrationNavigation.BankNavigation.CustomBehaviour;
            string authorizationEndpoint =
                accountAccessConsent.BankRegistrationNavigation.BankNavigation.AuthorizationEndpoint;
            string? issuerUrl = accountAccessConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl;
            bool supportsSca = accountAccessConsent.BankRegistrationNavigation.BankNavigation.SupportsSca;

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    accountAccessConsent.BankRegistrationNavigation.SoftwareStatementProfileId,
                    accountAccessConsent.BankRegistrationNavigation
                        .SoftwareStatementProfileOverride);

            // Create auth URL
            var state = entity.Id.ToString();

            string consentAuthGetAudClaim =
                customBehaviour?.AccountAccessConsentAuthGet?.AudClaim ??
                issuerUrl ?? throw new ArgumentException("No Issuer URL or custom behaviour Aud claim specified.");

            string authUrl = CreateAuthUrl.Create(
                accountAccessConsent.ExternalApiId,
                processedSoftwareStatementProfile,
                accountAccessConsent.BankRegistrationNavigation.ExternalApiObject.ExternalApiId,
                customBehaviour?.AccountAccessConsentAuthGet,
                authorizationEndpoint,
                consentAuthGetAudClaim,
                supportsSca,
                state,
                nonce,
                "accounts",
                _instrumentationClient);
            var response =
                new AccountAccessConsentAuthContextCreateResponse(
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
