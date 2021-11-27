// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
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
            // Bank resultProfile = new Bank();
            // (new Bank(Arg.Any<Models.Public.Request.Bank>())).Returns(resultProfile);

            var interaction =
                new LocalEntityPost<Bank, Models.Public.Request.Bank, BankResponse>(
                    Substitute.For<IDbEntityMethods<Bank>>(),
                    Substitute.For<IDbSaveChangesMethod>(),
                    Substitute.For<ITimeProvider>(),
                    Substitute.For<IProcessedSoftwareStatementProfileStore>(),
                    Substitute.For<IInstrumentationClient>());

            var newBank = new Models.Public.Request.Bank
            {
                IssuerUrl = "a",
                FinancialId = "b",
                Name = "c"
            };

            (BankResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await interaction.PostAsync(newBank);

            response.Should().NotBeNull();
        }
    }
}
