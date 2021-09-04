// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    [Collection("App context collection")]
    public class SpecialOperationTests : AppTests
    {
        public SpecialOperationTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture) : base(
            outputHelper,
            appContextFixture) { }

        [Fact(Skip = "This test may be unskipped and customised to delete a bank registration")]
        public async Task DeleteBankRegistration()
        {
            // Get request builder
            using IScopedRequestBuilder scopedRequestBuilder = new ScopedRequestBuilder(_serviceProvider);
            IRequestBuilder requestBuilder = scopedRequestBuilder.RequestBuilder;

            // Get bank profile definitions
            BankProfileDefinitions bankProfileDefinitions =
                _serviceProvider.GetRequiredService<BankProfileDefinitions>();

            // GET bank (example: NatWest)
            IFluentResponse<IQueryable<BankResponse>> bankResp = await requestBuilder.ClientRegistration
                .Banks
                .GetLocalAsync(x => x.FinancialId == bankProfileDefinitions.NatWest.FinancialId);
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            BankResponse bank = bankResp.Data!.Single();

            // GET bank registration (example: assume single registration for Lloyds)
            IFluentResponse<IQueryable<BankRegistrationResponse>> registrationResp = await requestBuilder
                .ClientRegistration
                .BankRegistrations
                .GetLocalAsync(x => x.BankId == bank.Id);
            registrationResp.Messages.Should().BeEmpty();
            registrationResp.Data.Should().NotBeNull();
            BankRegistrationResponse bankRegistration = registrationResp.Data!.Single();

            // DELETE bank registration
            IFluentResponse registrationResp2 = await requestBuilder.ClientRegistration
                .BankRegistrations
                .DeleteAsync(bankRegistration.Id, null, true);
            registrationResp2.Messages.Should().BeEmpty();
        }
    }
}