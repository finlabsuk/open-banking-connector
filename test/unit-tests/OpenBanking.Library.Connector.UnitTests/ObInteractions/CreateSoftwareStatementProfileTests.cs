// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.ObInteractions
{
    public class CreateSoftwareStatementProfileTests
    {
        [Fact]
        public async Task Create_IdReturned()
        {
            var repo = Substitute.For<ISoftwareStatementProfileRepository>();
            var mapper = Substitute.For<IEntityMapper>();

            var resultProfile = new SoftwareStatementProfile();

            mapper.Map<SoftwareStatementProfile>(Arg.Any<Connector.Model.Public.SoftwareStatementProfile>())
                .Returns(resultProfile);

            var interaction = new CreateSoftwareStatementProfile(repo, mapper);

            var profile = new Connector.Model.Public.SoftwareStatementProfile
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
