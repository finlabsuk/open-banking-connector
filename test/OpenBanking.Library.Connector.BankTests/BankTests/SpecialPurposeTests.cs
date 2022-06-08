// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    [Collection("App context collection")]
    public class SpecialPurposeTests : AppTests
    {
        public SpecialPurposeTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture) : base(
            outputHelper,
            appContextFixture) { }

        [Fact(Skip = "This test may be unskipped and customised to delete a bank registration")]
        public async Task DeleteBankRegistration()
        {
            // Get request builder
            using IRequestBuilderContainer scopedRequestBuilder = new ScopedRequestBuilderContainer(_serviceProvider);
            IRequestBuilder requestBuilder = scopedRequestBuilder.RequestBuilder;

            // Get bank profile definitions
            var bankProfileDefinitions =
                _serviceProvider.GetRequiredService<BankProfileDefinitions>();

            // GET bank (example: NatWest)
            IQueryable<BankResponse> bankResp = await requestBuilder.BankConfiguration
                .Banks
                .ReadLocalAsync(x => x.FinancialId == bankProfileDefinitions.GetBankProfile(BankProfileEnum.NatWest).FinancialId);
            //bankResp.Messages.Should().BeEmpty();
            BankResponse bank = bankResp.Single();

            // GET bank registration (example: assume single registration for Lloyds)
            IQueryable<BankRegistrationResponse> registrationResp = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .ReadLocalAsync(x => x.BankId == bank.Id);
            //registrationResp.Messages.Should().BeEmpty();
            //registrationResp.Data.Should().NotBeNull();
            BankRegistrationResponse bankRegistration = registrationResp.Single();

            // DELETE bank registration
            ObjectDeleteResponse registrationResp2 = await requestBuilder
                .BankConfiguration
                .BankRegistrations
                .DeleteAsync(bankRegistration.Id, null, BankProfileEnum.NatWest);
            registrationResp2.Warnings.Should().BeEmpty();
        }
    }
}
