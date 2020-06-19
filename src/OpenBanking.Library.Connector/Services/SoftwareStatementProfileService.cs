// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

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

        public IEnumerable<X509Certificate2> GetCertificates()
        {
            List<X509Certificate2> certificates = new List<X509Certificate2>();

            string cert = _defaultSoftwareStatementProfile.TransportCertificate;
            string key = _defaultSoftwareStatementProfile.TransportKey;
            if (cert != null && key != null)
            {
                X509Certificate2 certificate = CertificateFactories.GetCertificate2FromPem(privateKey: key, pem: cert);
                if (certificate != null)
                {
                    certificates.Add(certificate);
                }
            }

            return certificates;
        }

        public async Task SetSoftwareStatementProfileFromSecrets()
        {
            // Get active software statement profile
            IEnumerable<string> profiles =
                await _activeSoftwareStatementProfilesRepo.GetListAsync(
                    nameof(ActiveSoftwareStatementProfiles.ProfileIds));

            if (!profiles.Any())
            {
                throw new KeyNotFoundException("No active software statement profiles found.");
            }

            string defaultProfileId = profiles.ElementAt(0);

            SoftwareStatementProfile softwareStatementProfile =
                await _softwareStatementProfileRepo.GetAsync(defaultProfileId);

            SetSoftwareStatementProfile(softwareStatementProfile);
        }

        public void SetSoftwareStatementProfileFromSecretsSync()
        {
            SetSoftwareStatementProfileFromSecrets().GetAwaiter().GetResult();
        }
    }
}
