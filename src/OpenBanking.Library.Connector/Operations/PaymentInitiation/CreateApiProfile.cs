// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using PaymentInitiationApiProfilePublic = FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.PaymentInitiationApiProfile;


namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public class CreateApiProfile
    {
        private readonly IDbEntityRepository<ApiProfile> _apiProfileRepo;
        private readonly IDbMultiEntityMethods _dbContextService;

        public CreateApiProfile(IDbMultiEntityMethods dbContextService, IDbEntityRepository<ApiProfile> apiProfileRepo)
        {
            _dbContextService = dbContextService;
            _apiProfileRepo = apiProfileRepo;
        }

        public async Task<PaymentInitiationApiProfileResponse> CreateAsync(PaymentInitiationApiProfilePublic apiProfile)
        {
            // Create and store persistent object
            ApiProfile persistentApiProfile = new ApiProfile(
                apiProfile.Id,
                apiProfile.BankClientProfileId,
                apiProfile.ApiVersion,
                apiProfile.BaseUrl);
            await _apiProfileRepo.UpsertAsync(persistentApiProfile);
            await _dbContextService.SaveChangesAsync();

            // Return response object
            return new PaymentInitiationApiProfileResponse(persistentApiProfile.Id,
                persistentApiProfile.BankClientProfileId,
                persistentApiProfile.ApiVersion, persistentApiProfile.BaseUrl);
        }
    }
}
