// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using SoftwareStatementProfilePublic =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.SoftwareStatementProfile;


namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public interface ICreateSoftwareStatementProfile
    {
        Task<SoftwareStatementProfileResponse> CreateAsync(SoftwareStatementProfilePublic profile);
    }

    public class CreateSoftwareStatementProfile : ICreateSoftwareStatementProfile
    {
        private readonly ISoftwareStatementProfileService _softwareStatementProfileService;

        public CreateSoftwareStatementProfile(ISoftwareStatementProfileService softwareStatementProfileService)
        {
            _softwareStatementProfileService = softwareStatementProfileService;
        }

        public Task<SoftwareStatementProfileResponse> CreateAsync(SoftwareStatementProfilePublic profile)
        {
            profile.ArgNotNull(nameof(profile));

            _softwareStatementProfileService.SetSoftwareStatementProfile(profile);

            SoftwareStatementProfileResponse response = new SoftwareStatementProfileResponse(profile.Id);

            return Task.FromResult(response);
        }
    }
}
