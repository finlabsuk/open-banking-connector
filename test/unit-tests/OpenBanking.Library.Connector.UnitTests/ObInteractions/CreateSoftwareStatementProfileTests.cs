// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
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
            IDbEntityRepository<SoftwareStatementProfile>? repo =
                Substitute.For<IDbEntityRepository<SoftwareStatementProfile>>();
            ISoftwareStatementProfileService? service = Substitute.For<ISoftwareStatementProfileService>();
            IDbMultiEntityMethods? dbMethods = Substitute.For<IDbMultiEntityMethods>();
            IEntityMapper? mapper = Substitute.For<IEntityMapper>();

            SoftwareStatementProfile? resultProfile = new SoftwareStatementProfile();

            mapper.Map<SoftwareStatementProfile>(Arg.Any<Models.Public.Request.SoftwareStatementProfile>())
                .Returns(resultProfile);

            CreateSoftwareStatementProfile? interaction =
                new CreateSoftwareStatementProfile(softwareStatementProfileService: service);

            Models.Public.Request.SoftwareStatementProfile? profile = new Models.Public.Request.SoftwareStatementProfile
            {
                DefaultFragmentRedirectUrl = "http://test.com",
                SigningKey = "a",
                SigningKeyId = "b",
                SigningCertificate = "e30=",
                TransportKey = "a",
                TransportCertificate = "a",
                SoftwareStatement = "e30=.e30=.e30="
            };

            SoftwareStatementProfileResponse? result = await interaction.CreateAsync(profile);

            result.Should().NotBeNull();
        }
    }
}
