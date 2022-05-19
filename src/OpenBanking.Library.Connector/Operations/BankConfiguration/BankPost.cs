// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.RequestObjects.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class BankPost : LocalEntityPost<Bank, Models.Public.BankConfiguration.Request.Bank, BankResponse>
    {
        private readonly IBankProfileDefinitions _bankProfileDefinitions;

        public BankPost(
            IDbReadWriteEntityMethods<Bank> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient,
            IBankProfileDefinitions bankProfileDefinitions) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient)
        {
            _bankProfileDefinitions = bankProfileDefinitions;
        }

        protected override async Task<BankResponse> AddEntity(
            Models.Public.BankConfiguration.Request.Bank request,
            ITimeProvider timeProvider)
        {
            if (request.BankProfile is not null)
            {
                request = _bankProfileDefinitions.GetBankProfile(request.BankProfile.Value)
                    .GetBankRequest();
            }

            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new Bank(
                request.Reference,
                Guid.NewGuid(),
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                request.IssuerUrl,
                request.FinancialId);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Create response
            return entity.PublicGetLocalResponse;
        }
    }
}
