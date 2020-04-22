// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class CreateSoftwareStatementProfile
    {
        private readonly IDbMultiEntityMethods _dbMultiEntityMethods;
        private readonly IEntityMapper _mapper;
        private readonly IDbEntityRepository<SoftwareStatementProfile> _softwareStatementProfileRepo;

        public CreateSoftwareStatementProfile(
            IEntityMapper mapper,
            IDbMultiEntityMethods dbMultiEntityMethods,
            IDbEntityRepository<SoftwareStatementProfile> repo)
        {
            _mapper = mapper;
            _dbMultiEntityMethods = dbMultiEntityMethods;
            _softwareStatementProfileRepo = repo;
        }

        public async Task<SoftwareStatementProfileResponse> CreateAsync(
            Models.Public.Request.SoftwareStatementProfile profile)
        {
            profile.ArgNotNull(nameof(profile));

            SoftwareStatementProfile value = _mapper.Map<SoftwareStatementProfile>(profile);

            value.State = "ok";

            string[] softwareStatementComponentsBase64 = profile.SoftwareStatement.Split(new[] { '.' });
            if (softwareStatementComponentsBase64.Length != 3)
            {
                throw new ArgumentException("softwareStatementComponentsBase64 needs 3 components.");
            }

            value.SoftwareStatementHeaderBase64 = softwareStatementComponentsBase64[0];
            value.SoftwareStatementPayloadBase64 = softwareStatementComponentsBase64[1];
            value.SoftwwareStatementSignatureBase64 = softwareStatementComponentsBase64[2];

            byte[] payloadData
                = Convert.FromBase64String(value.SoftwareStatementPayloadBase64);
            string payloadString = Encoding.UTF8.GetString(payloadData);
            value.SoftwareStatementPayload = JsonConvert.DeserializeObject<SoftwareStatementPayload>(payloadString);

            await _softwareStatementProfileRepo.UpsertAsync(value);

            await _dbMultiEntityMethods.SaveChangesAsync();

            return new SoftwareStatementProfileResponse(value.Id);
        }
    }
}
