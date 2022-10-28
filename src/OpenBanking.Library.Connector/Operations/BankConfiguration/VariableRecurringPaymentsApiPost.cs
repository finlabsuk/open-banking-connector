// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class VariableRecurringPaymentsApiPost : LocalEntityCreate<VariableRecurringPaymentsApiEntity,
        VariableRecurringPaymentsApiRequest, VariableRecurringPaymentsApiResponse>
    {
        private readonly IBankProfileDefinitions _bankProfileDefinitions;

        public VariableRecurringPaymentsApiPost(
            IDbReadWriteEntityMethods<VariableRecurringPaymentsApiEntity> entityMethods,
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

        protected override async Task<VariableRecurringPaymentsApiResponse> AddEntity(
            VariableRecurringPaymentsApiRequest request,
            ITimeProvider timeProvider)
        {
            // Get bank profile if available
            BankProfile? bankProfile = null;
            if (request.BankProfile is not null)
            {
                bankProfile = _bankProfileDefinitions.GetBankProfile(request.BankProfile.Value);
            }

            // Get API version
            VariableRecurringPaymentsApiVersion apiVersion =
                request.ApiVersion ??
                bankProfile?.VariableRecurringPaymentsApi?.VariableRecurringPaymentsApiVersion ??
                throw new InvalidOperationException(
                    "ApiVersion specified as null and cannot be obtained from specified BankProfile.");

            // Get base URL
            string baseUrl =
                request.BaseUrl ??
                bankProfile?.VariableRecurringPaymentsApi?.BaseUrl ??
                throw new InvalidOperationException(
                    "BaseUrl specified as null and cannot be obtained from specified BankProfile.");

            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new VariableRecurringPaymentsApiEntity(
                request.Reference,
                Guid.NewGuid(),
                false,
                utcNow,
                request.CreatedBy,
                utcNow,
                request.CreatedBy,
                request.BankId,
                apiVersion,
                baseUrl.TrimEnd('/'));

            // Add entity
            await _entityMethods.AddAsync(entity);

            // Create response
            return entity.PublicGetLocalResponse;
        }
    }
}
