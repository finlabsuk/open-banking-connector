// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Newtonsoft.Json;
using SoftwareStatementProfile = FinnovationLabs.OpenBanking.Library.Connector.Model.Public.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations
{
    public class CreateSoftwareStatementProfile
    {
        private readonly IEntityMapper _mapper;
        private readonly ISoftwareStatementProfileRepository _repo;

        public CreateSoftwareStatementProfile(ISoftwareStatementProfileRepository repo, IEntityMapper mapper)
        {
            _repo = repo.ArgNotNull(nameof(repo));
            _mapper = mapper.ArgNotNull(nameof(mapper));
        }

        public async Task<SoftwareStatementProfileResponse> CreateAsync(SoftwareStatementProfile profile)
        {
            profile.ArgNotNull(nameof(profile));

            var value = _mapper.Map<Model.Persistent.SoftwareStatementProfile>(profile);

            value.State = "ok";

            var softwareStatementComponentsBase64 = profile.SoftwareStatement.Split(new[] { '.' });
            if (softwareStatementComponentsBase64.Length != 3)
            {
                throw new ArgumentException("softwareStatementComponentsBase64 needs 3 components.");
            }

            value.SoftwareStatementHeaderBase64 = softwareStatementComponentsBase64[0];
            value.SoftwareStatementPayloadBase64 = softwareStatementComponentsBase64[1];
            value.SoftwwareStatementSignatureBase64 = softwareStatementComponentsBase64[2];

            var payloadData
                = Convert.FromBase64String(value.SoftwareStatementPayloadBase64);
            var payloadString = Encoding.UTF8.GetString(payloadData);
            value.SoftwareStatementPayload = JsonConvert.DeserializeObject<SoftwareStatementPayload>(payloadString);

            await _repo.SetAsync(value);

            return new SoftwareStatementProfileResponse(value.Id);
        }
    }
}
