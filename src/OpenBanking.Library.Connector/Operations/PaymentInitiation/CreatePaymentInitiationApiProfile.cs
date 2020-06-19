// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using PaymentInitiationApiProfilePublic =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.PaymentInitiationApiProfile;


namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.PaymentInitiation
{
    public interface ICreatePaymentInitiationApiProfile
    {
        Task<PaymentInitiationApiProfileResponse> CreateAsync(PaymentInitiationApiProfilePublic apiProfile);
    }

    public class CreatePaymentInitiationApiProfile : ICreatePaymentInitiationApiProfile
    {
        private readonly IDbEntityRepository<ApiProfile> _apiProfileRepo;
        private readonly IDbMultiEntityMethods _dbContextService;

        public CreatePaymentInitiationApiProfile(
            IDbMultiEntityMethods dbContextService,
            IDbEntityRepository<ApiProfile> apiProfileRepo)
        {
            _dbContextService = dbContextService;
            _apiProfileRepo = apiProfileRepo;
        }

        public async Task<PaymentInitiationApiProfileResponse> CreateAsync(PaymentInitiationApiProfilePublic apiProfile)
        {
            // Check for existing API profile.
            // ApiProfile existingProfile = await _apiProfileRepo
            //     .GetAsync(apiProfile.Id);
            // if (!(existingProfile is null))
            // {
            //     throw new Exception("There is already a API Profile with specified ID.");
            // }

            // Create and store persistent object
            ApiProfile persistentApiProfile = new ApiProfile(
                id: apiProfile.Id,
                bankClientProfileId: apiProfile.BankClientProfileId,
                apiVersion: apiProfile.ApiVersion,
                baseUrl: apiProfile.BaseUrl);
            await _apiProfileRepo.UpsertAsync(persistentApiProfile);
            await _dbContextService.SaveChangesAsync();

            // Return response object
            return new PaymentInitiationApiProfileResponse(
                id: persistentApiProfile.Id,
                bankClientProfileId: persistentApiProfile.BankClientProfileId,
                apiVersion: persistentApiProfile.ApiVersion,
                baseUrl: persistentApiProfile.BaseUrl);
        }
    }
}
