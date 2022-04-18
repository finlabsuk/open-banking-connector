// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class VariableRecurringPaymentsApiPost : LocalEntityPost<VariableRecurringPaymentsApiEntity,
        VariableRecurringPaymentsApiRequest, VariableRecurringPaymentsApiResponse>
    {
        public VariableRecurringPaymentsApiPost(
            IDbReadWriteEntityMethods<VariableRecurringPaymentsApiEntity> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient) { }

        protected override async Task<VariableRecurringPaymentsApiResponse> AddEntity(
            VariableRecurringPaymentsApiRequest request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new VariableRecurringPaymentsApiEntity(
                request.Reference,
                Guid.NewGuid(),
                false,
                utcNow,
                createdBy,
                utcNow,
                createdBy,
                request.BankId,
                request.ApiVersion,
                request.BaseUrl);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Create response
            return entity.PublicGetLocalResponse;
        }
    }
}
