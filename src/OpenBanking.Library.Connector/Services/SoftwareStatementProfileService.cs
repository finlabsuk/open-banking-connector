// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Services
{
    public class SoftwareStatementProfileService : ISoftwareStatementProfileService
    {
        private readonly IKeySecretReadRepository<ActiveSoftwareStatementProfiles>
            _activeSoftwareStatementProfilesRepo;

        private readonly IEntityMapper _mapper;

        private readonly IKeySecretMultiItemReadRepository<SoftwareStatementProfile> _softwareStatementProfileRepo;
        private Models.Persistent.SoftwareStatementProfile _defaultSoftwareStatementProfile;

        public SoftwareStatementProfileService(
            IKeySecretMultiItemReadRepository<SoftwareStatementProfile> softwareStatementProfileRepo,
            IKeySecretReadRepository<ActiveSoftwareStatementProfiles> activeSoftwareStatementProfilesRepo,
            IEntityMapper mapper)
        {
            _softwareStatementProfileRepo = softwareStatementProfileRepo;
            _activeSoftwareStatementProfilesRepo = activeSoftwareStatementProfilesRepo;
            _mapper = mapper;

            // Get active software statement profile
            // Task<ActiveSoftwareStatementProfiles> profiles = _activeSoftwareStatementProfilesRepo.GetAsync();
        }

        public void SetSoftwareStatementProfile(SoftwareStatementProfile profile)
        {
            profile.ArgNotNull(nameof(profile));

            Models.Persistent.SoftwareStatementProfile value =
                _mapper.Map<Models.Persistent.SoftwareStatementProfile>(profile);

            value.State = "ok";

            string[] softwareStatementComponentsBase64 = profile.SoftwareStatement.Split(new[] { '.' });
            if (softwareStatementComponentsBase64.Length != 3)
            {
                throw new ArgumentException("softwareStatementComponentsBase64 needs 3 components.");
            }

            value.SoftwareStatementHeaderBase64 = softwareStatementComponentsBase64[0];
            value.SoftwareStatementPayloadBase64 = softwareStatementComponentsBase64[1];
            value.SoftwareStatementPayload =
                value.SoftwareStatementPayloadFromBase64(softwareStatementComponentsBase64[1]);
            value.SoftwwareStatementSignatureBase64 = softwareStatementComponentsBase64[2];

            _defaultSoftwareStatementProfile = value;
        }

        public Models.Persistent.SoftwareStatementProfile GetSoftwareStatementProfile(string id)
        {
            if (id != _defaultSoftwareStatementProfile.Id)
            {
                throw new KeyNotFoundException($"Software statement profile with ID {id} not available.");
            }

            return _defaultSoftwareStatementProfile;
        }
    }
}
