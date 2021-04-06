// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.ObInteractions
{
    public class CreateBankTests
    {
        [Fact]
        public async Task Create_IdReturned()
        {
            IDbEntityMethods<Bank> repo =
                Substitute.For<IDbEntityMethods<Bank>>();
            IDbSaveChangesMethod dbMethods = Substitute.For<IDbSaveChangesMethod>();
            ITimeProvider timeProvider = Substitute.For<ITimeProvider>();

            // Bank resultProfile = new Bank();
            // (new Bank(Arg.Any<Models.Public.Request.Bank>())).Returns(resultProfile);

            PostBank interaction =
                new PostBank(repo, dbMethods, timeProvider);

            Models.Public.Request.Bank newBank = new Models.Public.Request.Bank
            {
                IssuerUrl = "a",
                FinancialId = "b",
                Name = "c"
            };

            var (response, nonErrorMessages) =
                await interaction.PostAsync(
                    newBank,
                    null);

            response.Should().NotBeNull();
        }
    }
}
