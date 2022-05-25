// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;
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
                new BankPost(
                    Substitute.For<IDbEntityMethods<Bank>>(),
                    Substitute.For<IDbSaveChangesMethod>(),
                    Substitute.For<ITimeProvider>(),
                    Substitute.For<IProcessedSoftwareStatementProfileStore>(),
                    Substitute.For<IInstrumentationClient>(),
                    Substitute.For<IBankProfileDefinitions>(),
                    Substitute.For<IApiClient>());

            var newBank = new Models.Public.BankConfiguration.Request.Bank
            {
                IssuerUrl = "a",
                FinancialId = "b",
                Reference = "c"
            };

            (BankResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
                await interaction.CreateAsync(newBank);

            response.Should().NotBeNull();
        }
    }
}
