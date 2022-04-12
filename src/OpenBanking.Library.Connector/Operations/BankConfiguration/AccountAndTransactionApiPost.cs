// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class AccountAndTransactionApiPost : LocalEntityPost<AccountAndTransactionApiEntity,
        AccountAndTransactionApiRequest, AccountAndTransactionApiResponse>
    {
        public AccountAndTransactionApiPost(
            IDbReadWriteEntityMethods<AccountAndTransactionApiEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient) { }

        protected override async Task<AccountAndTransactionApiResponse> AddEntity(
            AccountAndTransactionApiRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new AccountAndTransactionApiEntity(
                request.Name,
                request.Reference,
                Guid.NewGuid(),
                false,
                utcNow,
                createdBy,
                utcNow,
                createdBy,
                request.ApiVersion,
                request.BaseUrl,
                request.BankId);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Create response
            return entity.PublicGetLocalResponse;
        }
    }
}
