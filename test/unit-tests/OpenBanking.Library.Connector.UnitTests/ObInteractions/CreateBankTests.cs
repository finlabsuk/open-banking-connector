// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
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
            IDbEntityRepository<Bank> repo =
                Substitute.For<IDbEntityRepository<Bank>>();
            IDbMultiEntityMethods dbMethods = Substitute.For<IDbMultiEntityMethods>();
            ITimeProvider timeProvider = Substitute.For<ITimeProvider>();

            // Bank resultProfile = new Bank();
            // (new Bank(Arg.Any<Models.Public.Request.Bank>())).Returns(resultProfile);

            CreateBank interaction =
                new CreateBank(bankRepo: repo, dbMultiEntityMethods: dbMethods, timeProvider: timeProvider);

            Models.Public.Request.Bank newBank = new Models.Public.Request.Bank
            {
                IssuerUrl = "a",
                FinancialId = "b",
                Name = "c"
            };

            BankResponse result = await interaction.CreateAsync(newBank, null);

            result.Should().NotBeNull();
        }
    }
}
