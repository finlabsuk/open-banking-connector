// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
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
            IOpenBankingRequestBuilder builder = CreateOpenBankingRequestBuilder();

            string issuerUrl = "http://aaa.com/";
            string xfapi = "xfapi";
            string softwareStatementId = "softwareStatement";

            BankClientProfileContext ctx = builder.BankClientProfile();

            BankClientProfileFluentResponse result = await ctx
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

            Connector.Persistence.IDbEntityRepository<Models.Persistent.BankClientProfile> repo = ctx.Context.ClientProfileRepository;
            IQueryable<Models.Persistent.BankClientProfile> instances = await repo.GetAllAsync();
            Models.Persistent.BankClientProfile persistedResult = instances.FirstOrDefault();

            persistedResult.IssuerUrl.Should().Be(issuerUrl);
            persistedResult.XFapiFinancialId.Should().Be(xfapi);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId);
        }

        [BddfyFact]
        public async Task Repository_ClientUpserted()
        {
            BankClientProfileContext ctx = CreateOpenBankingRequestBuilder().BankClientProfile();

            string issuerUrl = "http://aaa.com/";
            string xfapi2 = "xfapi2";
            string softwareStatementId2 = "softwareStatementId2";


            BankClientProfileFluentResponse result1 = await ctx
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

            BankClientProfileFluentResponse result2 = await ctx
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


            Connector.Persistence.IDbEntityRepository<Models.Persistent.BankClientProfile> repo = ctx.Context.ClientProfileRepository;

            IQueryable<Models.Persistent.BankClientProfile> instances = await repo.GetAllAsync();
            Models.Persistent.BankClientProfile persistedResult = instances.FirstOrDefault();


            persistedResult.IssuerUrl.Should().Be(issuerUrl);
            persistedResult.XFapiFinancialId.Should().Be(xfapi2);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId2);
        }
    }
}
