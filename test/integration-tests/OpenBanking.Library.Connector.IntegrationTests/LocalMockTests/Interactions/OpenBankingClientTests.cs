// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FluentAssertions;
using TestStack.BDDfy.Xunit;
using BankRegistration = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.LocalMockTests.Interactions
{
    public class OpenBankingClientTests : BaseLocalMockTest
    {
        [BddfyFact(Skip = "Upset not appropriate for BankClientProfile")]
        public async Task Repository_ClientInserted()
        {
            IOpenBankingRequestBuilder builder = CreateOpenBankingRequestBuilder();

            string issuerUrl = "http://aaa.com/";
            string xfapi = "xfapi";
            string softwareStatementId = "softwareStatement";

            BankRegistrationContext ctx = builder.BankClientProfile();

            FluentResponse<BankRegistrationResponse> result = await ctx
                //.Id("BankClientProfileId")
                //.IssuerUrl(new Uri(issuerUrl))
                //.XFapiFinancialId(xfapi)
                .SoftwareStatementProfileId(softwareStatementId)
                .HttpMtlsOverrides(
                    new HttpMtlsConfigurationOverrides
                    {
                        TlsCertificateVerification = "aaa",
                        TlsRenegotiationSupport = "bbb"
                    })
                .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
                .OpenIdOverrides(
                    new OpenIdConfigurationOverrides
                    {
                        RegistrationEndpointUrl = "http://bbb.com"
                    })
                .RegistrationResponseOverrides(new RegistrationResponseJsonOptions())
                .UpsertAsync();

            result.Messages.Should().HaveCount(0);

            IDbEntityRepository<BankRegistration> repo = ctx.Context.BankRegistrationRepository;
            IQueryable<BankRegistration> instances = await repo.GetAllAsync();
            BankRegistration persistedResult = instances.FirstOrDefault();

            //persistedResult.IssuerUrl.Should().Be(issuerUrl);
            //persistedResult.XFapiFinancialId.Should().Be(xfapi);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId);
        }

        [BddfyFact(Skip = "Upset not appropriate for BankClientProfile")]
        public async Task Repository_ClientUpserted()
        {
            BankRegistrationContext ctx = CreateOpenBankingRequestBuilder().BankClientProfile();

            string issuerUrl = "http://aaa.com/";
            string xfapi2 = "xfapi2";
            string softwareStatementId2 = "softwareStatementId2";


            FluentResponse<BankRegistrationResponse> result1 = await ctx
                //.Id("BankClientProfileId")
                //.IssuerUrl(new Uri(issuerUrl))
                //.XFapiFinancialId("xfapi")
                .SoftwareStatementProfileId("softwareStatement")
                .HttpMtlsOverrides(
                    new HttpMtlsConfigurationOverrides
                    {
                        TlsCertificateVerification = "aaa",
                        TlsRenegotiationSupport = "bbb"
                    })
                .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
                .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
                .RegistrationResponseOverrides(new RegistrationResponseJsonOptions())
                .UpsertAsync();

            result1.Messages.Should().HaveCount(0);

            FluentResponse<BankRegistrationResponse> result2 = await ctx
                //.IssuerUrl(new Uri(issuerUrl))
                //.XFapiFinancialId(xfapi2)
                .SoftwareStatementProfileId(softwareStatementId2)
                .HttpMtlsOverrides(
                    new HttpMtlsConfigurationOverrides
                    {
                        TlsCertificateVerification = "aaa",
                        TlsRenegotiationSupport = "bbb"
                    })
                .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
                .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
                .RegistrationResponseOverrides(new RegistrationResponseJsonOptions())
                .UpsertAsync();
            result2.Messages.Should().HaveCount(0);


            IDbEntityRepository<BankRegistration> repo = ctx.Context.BankRegistrationRepository;

            IQueryable<BankRegistration> instances = await repo.GetAllAsync();
            BankRegistration persistedResult = instances.FirstOrDefault();


            //persistedResult.IssuerUrl.Should().Be(issuerUrl);
            //persistedResult.XFapiFinancialId.Should().Be(xfapi2);
            persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId2);
        }
    }
}
