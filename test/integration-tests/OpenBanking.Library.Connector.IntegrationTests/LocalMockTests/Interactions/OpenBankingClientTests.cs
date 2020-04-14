// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FluentAssertions;
using TestStack.BDDfy.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.LocalMockTests.Interactions
{
    public class OpenBankingClientTests : BaseLocalMockTest
    {
        [BddfyFact]
        public async Task Repository_ClientInserted()
        {
            var builder = CreateOpenBankingRequestBuilder();

            var issuerUrl = "http://aaa.com/";
            var xfapi = "xfapi";
            var softwareStatementId = "softwareStatement";

            var ctx = builder.BankClientProfile();

            var result = await ctx
                .Id("BankClientProfileId")
                .IssuerUrl(new Uri(issuerUrl))
                .XFapiFinancialId(xfapi)
                .SoftwareStatementProfileId(softwareStatementId)
                .HttpMtlsOverrides(new HttpMtlsConfigurationOverrides
                {
                    TlsCertificateVerification = "aaa",
                    TlsRenegotiationSupport = "bbb"
                })
                .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
                .OpenIdOverrides(new OpenIdConfigurationOverrides
                {
                    RegistrationEndpointUrl = "http://bbb.com"
                })
                .RegistrationResponseOverrides(new BankClientRegistrationDataOverrides())
                .UpsertAsync();

            result.Messages.Should().HaveCount(0);

            var repo = ctx.Context.ClientProfileRepository;
            var ids = await repo.GetIdsAsync();

            var persistedResult = await repo.GetAsync(ids[0]);

            persistedResult.IssuerUrl.Should().Be(issuerUrl);
            persistedResult.XFapiFinancialId.Should().Be(xfapi);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId);
        }

        [BddfyFact]
        public async Task Repository_ClientUpserted()
        {
            var ctx = CreateOpenBankingRequestBuilder().BankClientProfile();

            var issuerUrl = "http://aaa.com/";
            var xfapi2 = "xfapi2";
            var softwareStatementId2 = "softwareStatementId2";


            var result1 = await ctx
                .Id("BankClientProfileId")
                .IssuerUrl(new Uri(issuerUrl))
                .XFapiFinancialId("xfapi")
                .SoftwareStatementProfileId("softwareStatement")
                .HttpMtlsOverrides(new HttpMtlsConfigurationOverrides
                {
                    TlsCertificateVerification = "aaa",
                    TlsRenegotiationSupport = "bbb"
                })
                .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
                .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
                .RegistrationResponseOverrides(new BankClientRegistrationDataOverrides())
                .UpsertAsync();

            result1.Messages.Should().HaveCount(0);

            var result2 = await ctx
                .IssuerUrl(new Uri(issuerUrl))
                .XFapiFinancialId(xfapi2)
                .SoftwareStatementProfileId(softwareStatementId2)
                .HttpMtlsOverrides(new HttpMtlsConfigurationOverrides
                {
                    TlsCertificateVerification = "aaa",
                    TlsRenegotiationSupport = "bbb"
                })
                .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
                .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
                .RegistrationResponseOverrides(new BankClientRegistrationDataOverrides())
                .UpsertAsync();
            result2.Messages.Should().HaveCount(0);


            var repo = ctx.Context.ClientProfileRepository;
            var ids = await repo.GetIdsAsync();

            var persistedResult = await repo.GetAsync(ids[0]);

            persistedResult.IssuerUrl.Should().Be(issuerUrl);
            persistedResult.XFapiFinancialId.Should().Be(xfapi2);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId2);
        }
    }
}
