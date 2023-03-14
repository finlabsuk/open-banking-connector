// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.ObInteractions;

public class CreateBankTests
{
    [Fact]
    public async Task Create_IdReturned()
    {
        var interaction =
            new AccountAndTransactionApiPost(
                Substitute.For<IDbEntityMethods<AccountAndTransactionApiEntity>>(),
                Substitute.For<IDbSaveChangesMethod>(),
                Substitute.For<ITimeProvider>(),
                Substitute.For<IProcessedSoftwareStatementProfileStore>(),
                Substitute.For<IInstrumentationClient>(),
                Substitute.For<IBankProfileService>());

        var newBank = new AccountAndTransactionApiRequest
        {
            Reference = "c",
            BankId = default,
            ApiVersion = AccountAndTransactionApiVersion.Version3p1p7,
            BaseUrl = "www.example.com"
        };

        (AccountAndTransactionApiResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) =
            await interaction.CreateAsync(newBank, new LocalCreateParams());

        response.Should().NotBeNull();
    }
}
