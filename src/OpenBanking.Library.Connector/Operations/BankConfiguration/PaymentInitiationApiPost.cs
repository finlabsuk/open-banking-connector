// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration
{
    internal class PaymentInitiationApiPost : LocalEntityCreate<PaymentInitiationApiEntity,
        PaymentInitiationApiRequest, PaymentInitiationApiResponse>
    {
        private readonly IBankProfileDefinitions _bankProfileDefinitions;

        public PaymentInitiationApiPost(
            IDbReadWriteEntityMethods<PaymentInitiationApiEntity> entityMethods,
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

        protected override async Task<PaymentInitiationApiResponse> AddEntity(
            PaymentInitiationApiRequest request,
            ITimeProvider timeProvider)
        {
            // Get bank profile if available
            BankProfile? bankProfile = null;
            if (request.BankProfile is not null)
            {
                bankProfile = _bankProfileDefinitions.GetBankProfile(request.BankProfile.Value);
            }

            // Get API version
            PaymentInitiationApiVersion apiVersion =
                request.ApiVersion ??
                bankProfile?.PaymentInitiationApi?.PaymentInitiationApiVersion ??
                throw new InvalidOperationException(
                    "ApiVersion specified as null and cannot be obtained from specified BankProfile.");

            // Get base URL
            string baseUrl =
                request.BaseUrl ??
                bankProfile?.PaymentInitiationApi?.BaseUrl ??
                throw new InvalidOperationException(
                    "BaseUrl specified as null and cannot be obtained from specified BankProfile.");

            DateTimeOffset utcNow = _timeProvider.GetUtcNow();
            var entity = new PaymentInitiationApiEntity(
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
