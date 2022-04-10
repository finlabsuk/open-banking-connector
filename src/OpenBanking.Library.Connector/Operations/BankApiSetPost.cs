// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    internal class BankApiSetPost : LocalEntityPost<BankApiSet, Models.Public.Request.BankApiSet, BankApiSetResponse>
    {
        public BankApiSetPost(
            IDbReadWriteEntityMethods<BankApiSet> entityMethods,
            IDbSaveChangesMethod dbSaveChangesMethod,
            ITimeProvider timeProvider,
            IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
            IInstrumentationClient instrumentationClient) : base(
            entityMethods,
            dbSaveChangesMethod,
            timeProvider,
            softwareStatementProfileRepo,
            instrumentationClient) { }

        protected override async Task<BankApiSetResponse> AddEntity(
            Models.Public.Request.BankApiSet request,
            string? createdBy,
            ITimeProvider timeProvider)
        {
            // Create persisted entity
            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new BankApiSet(
                request.Name,
                request.Reference,
                Guid.NewGuid(),
                false,
                utcNow,
                createdBy,
                utcNow,
                createdBy,
                request.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion,
                request.VariableRecurringPaymentsApi?.BaseUrl ?? "",
                request.PaymentInitiationApi?.PaymentInitiationApiVersion,
                request.PaymentInitiationApi?.BaseUrl ?? "",
                request.AccountAndTransactionApi?.AccountAndTransactionApiVersion,
                request.AccountAndTransactionApi?.BaseUrl ?? "",
                request.BankId);

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Create response
            return entity.PublicGetLocalResponse;
        }
    }
}
