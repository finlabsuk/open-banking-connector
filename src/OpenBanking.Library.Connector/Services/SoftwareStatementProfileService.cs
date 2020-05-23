// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Services
{
    public class SoftwareStatementProfileService
    {
        private readonly IKeySecretReadOnlyRepository<ActiveSoftwareStatementProfiles>
            _activeSoftwareStatementProfilesRepo;

        private readonly IKeySecretMultiItemReadOnlyRepository<SoftwareStatementProfile> _softwareStatementProfileRepo;

        public SoftwareStatementProfileService(
            IKeySecretMultiItemReadOnlyRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IKeySecretReadOnlyRepository<ActiveSoftwareStatementProfiles> activeSoftwareStatementProfilesRepo)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _activeSoftwareStatementProfilesRepo = activeSoftwareStatementProfilesRepo;

            // Get active software statement profile
            Task<ActiveSoftwareStatementProfiles> profiles = _activeSoftwareStatementProfilesRepo.GetAsync();
        }
    }
}
