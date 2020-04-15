// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.ObInteractions
{
    public class CreateSoftwareStatementProfileTests
    {
        [Fact]
        public async Task Create_IdReturned()
        {
            var repo = Substitute.For<IDbEntityRepository<Models.Persistent.SoftwareStatementProfile>>();
            var mapper = Substitute.For<IEntityMapper>();
            
            var resultProfile = new SoftwareStatementProfile();

            mapper.Map<SoftwareStatementProfile>(Arg.Any<Models.Public.SoftwareStatementProfile>())
                .Returns(resultProfile);

            var interaction = new CreateSoftwareStatementProfile(mapper, repo);

            var profile = new Models.Public.SoftwareStatementProfile
            {
                DefaultFragmentRedirectUrl = "http://test.com",
                SigningKeySecretName = "a",
                SigningKeyId = "b",
                SigningCertificate = "e30=",
                TransportKeySecretName = "a",
                TransportCertificate = "a",
                SoftwareStatement = "e30=.e30=.e30="
            };

            var result = await interaction.CreateAsync(profile);

            result.Should().NotBeNull();
        }
    }
}
