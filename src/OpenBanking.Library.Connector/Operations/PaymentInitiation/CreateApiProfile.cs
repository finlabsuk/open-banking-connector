// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateApiProfile
    {
        private readonly IApiClient _apiClient;
        private readonly IApiProfileRepository _apiProfileRepo;
        private readonly IOpenBankingClientProfileRepository _openBankingClientRepo;
        private readonly ISoftwareStatementProfileRepository _softwareStatementProfileRepo;

        public CreateApiProfile(IApiClient apiClient, ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IOpenBankingClientProfileRepository openBankingClientRepo, IApiProfileRepository apiProfileRepo)
        {
            _apiClient = apiClient;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _openBankingClientRepo = openBankingClientRepo;
            _apiProfileRepo = apiProfileRepo;
        }

        public async Task<ApiProfile> CreateAsync(Models.Public.PaymentInitiation.Request.ApiProfile apiProfile)
        {
            // Load relevant objects
            var bankClient = await _openBankingClientRepo.GetAsync(apiProfile.BankClientProfileId) ??
                             throw new KeyNotFoundException("The OB Client Profile does not exist.");
            var softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankClient.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException("The Software statement does not exist.");

            // Create and store persistent object
            var persistentApiProfile = new Models.Persistent.PaymentInitiation.ApiProfile(apiProfile.Id,
                apiProfile.BankClientProfileId,
                apiProfile.ApiVersion, apiProfile.BaseUrl);
            await _apiProfileRepo.SetAsync(persistentApiProfile);

            // Return response object
            return new ApiProfile(persistentApiProfile.Id, persistentApiProfile.BankClientProfileId,
                persistentApiProfile.ApiVersion, persistentApiProfile.BaseUrl);
        }
    }
}
