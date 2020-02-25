// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
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

            var ctx = builder.Client();

            var result = await ctx
                .IssuerUrl(new Uri(issuerUrl))
                .XFapiFinancialId(xfapi)
                .SoftwareStatementProfileId(softwareStatementId)
                .HttpMtlsOverrides(new HttpClientMtlsConfigurationOverrides
                {
                    TlsCertificateVerification = "aaa",
                    TlsRenegotiationSupport = "bbb"
                })
                .RegistrationClaimsOverrides(new OpenBankingClientRegistrationClaimsOverrides())
                .OpenIdOverrides(new OpenIdConfigurationOverrides
                {
                    RegistrationEndpointUrl = "http://bbb.com"
                })
                .RegistrationResponseOverrides(new ClientRegistrationResponseOverrides())
                .UpsertAsync();

            result.Messages.Should().HaveCount(0);

            var repo = ctx.Context.ClientRepository;
            var ids = await repo.GetIdsAsync();

            var persistedResult = await repo.GetAsync(ids[0]);

            persistedResult.IssuerUrl.Should().Be(issuerUrl);
            persistedResult.XFapiFinancialId.Should().Be(xfapi);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId);
        }

        [BddfyFact]
        public async Task Repository_ClientUpserted()
        {
            var ctx = CreateOpenBankingRequestBuilder().Client();

            var issuerUrl = "http://aaa.com/";
            var xfapi2 = "xfapi2";
            var softwareStatementId2 = "softwareStatementId2";


            var result1 = await ctx
                .IssuerUrl(new Uri(issuerUrl))
                .XFapiFinancialId("xfapi")
                .SoftwareStatementProfileId("softwareStatement")
                .HttpMtlsOverrides(new HttpClientMtlsConfigurationOverrides
                {
                    TlsCertificateVerification = "aaa",
                    TlsRenegotiationSupport = "bbb"
                })
                .RegistrationClaimsOverrides(new OpenBankingClientRegistrationClaimsOverrides())
                .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
                .RegistrationResponseOverrides(new ClientRegistrationResponseOverrides())
                .UpsertAsync();

            result1.Messages.Should().HaveCount(0);

            var result2 = await ctx
                .IssuerUrl(new Uri(issuerUrl))
                .XFapiFinancialId(xfapi2)
                .SoftwareStatementProfileId(softwareStatementId2)
                .HttpMtlsOverrides(new HttpClientMtlsConfigurationOverrides
                {
                    TlsCertificateVerification = "aaa",
                    TlsRenegotiationSupport = "bbb"
                })
                .RegistrationClaimsOverrides(new OpenBankingClientRegistrationClaimsOverrides())
                .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
                .RegistrationResponseOverrides(new ClientRegistrationResponseOverrides())
                .UpsertAsync();
            result2.Messages.Should().HaveCount(0);


            var repo = ctx.Context.ClientRepository;
            var ids = await repo.GetIdsAsync();

            var persistedResult = await repo.GetAsync(ids[0]);

            persistedResult.IssuerUrl.Should().Be(issuerUrl);
            persistedResult.XFapiFinancialId.Should().Be(xfapi2);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId2);
        }
    }
}
