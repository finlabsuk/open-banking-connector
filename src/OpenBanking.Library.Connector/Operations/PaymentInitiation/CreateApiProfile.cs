// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateApiProfile
    {
        private readonly IDbEntityRepository<ApiProfile> _apiProfileRepo;
        private readonly IDbEntityRepository<BankClientProfile> _openBankingClientRepo;
        private readonly IDbEntityRepository<SoftwareStatementProfile> _softwareStatementProfileRepo;

        public CreateApiProfile(IDbEntityRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IDbEntityRepository<BankClientProfile> openBankingClientRepo, IDbEntityRepository<ApiProfile> apiProfileRepo)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _openBankingClientRepo = openBankingClientRepo;
            _apiProfileRepo = apiProfileRepo;
        }

        public async Task<PaymentInitiationApiProfileResponse> CreateAsync(Models.Public.PaymentInitiation.Request.PaymentInitiationApiProfile apiProfile)
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
            await _apiProfileRepo.SaveChangesAsync();

            // Return response object
            return new PaymentInitiationApiProfileResponse(persistentApiProfile.Id, persistentApiProfile.BankClientProfileId,
                persistentApiProfile.ApiVersion, persistentApiProfile.BaseUrl);
        }
    }
}
