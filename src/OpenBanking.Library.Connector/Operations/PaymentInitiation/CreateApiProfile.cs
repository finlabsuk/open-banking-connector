// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.Response.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Security.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateApiProfile
    {
        private readonly IApiClient _apiClient;
        private readonly IApiProfileRepository _apiProfileRepo;
        private readonly IOpenBankingClientProfileRepository _openBankingClientProfileRepo;
        private readonly IOpenBankingClientRepository _openBankingClientRepo;
        private readonly ISoftwareStatementProfileRepository _softwareStatementProfileRepo;

        public CreateApiProfile(IApiClient apiClient, ISoftwareStatementProfileRepository softwareStatementProfileRepo,
            IOpenBankingClientProfileRepository openBankingClientProfileRepo,
            IOpenBankingClientRepository openBankingClientRepo, IApiProfileRepository apiProfileRepo)
        {
            _apiClient = apiClient;
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _openBankingClientProfileRepo = openBankingClientProfileRepo;
            _openBankingClientRepo = openBankingClientRepo;
            _apiProfileRepo = apiProfileRepo;
        }

        public async Task<ApiProfile> CreateAsync(Model.Public.Request.PaymentInitiation.ApiProfile apiProfile)
        {
            // Load relevant objects
            var bankClientProfile = await _openBankingClientProfileRepo.GetAsync(apiProfile.BankClientProfileId) ??
                                    throw new KeyNotFoundException("The OB Client Profile does not exist.");
            var bankClient = await _openBankingClientRepo.GetAsync(bankClientProfile.BankClientId) ??
                             throw new KeyNotFoundException("The OB Client Profile does not exist.");
            var softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(bankClient.SoftwareStatementProfileId) ??
                throw new KeyNotFoundException("The Software statement does not exist.");

            // Create and store persistent object
            var persistentApiProfile = new Model.Persistent.PaymentInitiation.ApiProfile(apiProfile.Id,
                apiProfile.BankClientProfileId,
                apiProfile.ApiVersion, apiProfile.BaseUrl);
            await _apiProfileRepo.SetAsync(persistentApiProfile);

            // Return response object
            return new ApiProfile(persistentApiProfile.Id, persistentApiProfile.BankClientProfileId,
                persistentApiProfile.ApiVersion, persistentApiProfile.BaseUrl);
        }
    }
}
