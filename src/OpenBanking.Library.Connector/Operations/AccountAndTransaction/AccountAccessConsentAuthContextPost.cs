// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
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
        AccountAccessConsentAuthContextPost : LocalEntityPostBase<
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

        protected override AccountAccessConsentAuthContextPersisted Create(
            AccountAccessConsentAuthContextRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            var tokenEndpointResponse = new ReadWriteProperty<TokenEndpointResponse?>(
                null,
                timeProvider,
                createdBy);

            var output = new AccountAccessConsentAuthContextPersisted(
                Guid.NewGuid(),
                request.Name,
                tokenEndpointResponse,
                request.AccountAccessConsentId,
                createdBy,
                timeProvider);

            return output;
        }

        protected override async Task<AccountAccessConsentAuthContextCreateLocalResponse> CreateResponse(
            AccountAccessConsentAuthContextPersisted persistedObject)
        {
            // Load relevant data objects
            AccountAccessConsentPersisted accountAccessConsent =
                _accountAccessConsentMethods
                    .DbSetNoTracking
                    .Include(o => o.BankRegistrationNavigation)
                    .Include(o => o.BankRegistrationNavigation.BankNavigation)
                    .SingleOrDefault(x => x.Id == persistedObject.AccountAccessConsentId) ??
                throw new KeyNotFoundException(
                    $"No record found for Account Access Consent with ID {persistedObject.AccountAccessConsentId}.");

            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(
                    accountAccessConsent.BankRegistrationNavigation.SoftwareStatementProfileId,
                    accountAccessConsent.BankRegistrationNavigation
                        .SoftwareStatementAndCertificateProfileOverrideCase);

            // Create auth URL
            var state = persistedObject.Id.ToString();
            string authUrl = CreateAuthUrl.Create(
                accountAccessConsent.ExternalApiId,
                processedSoftwareStatementProfile,
                accountAccessConsent.BankRegistrationNavigation,
                accountAccessConsent.BankRegistrationNavigation.BankNavigation.IssuerUrl,
                state,
                "accounts",
                _instrumentationClient);
            var response =
                new AccountAccessConsentAuthContextCreateLocalResponse(
                    persistedObject.Id,
                    persistedObject.Name,
                    persistedObject.Created,
                    persistedObject.CreatedBy,
                    persistedObject.AccountAccessConsentId,
                    authUrl);

            return response;
        }
    }
}
