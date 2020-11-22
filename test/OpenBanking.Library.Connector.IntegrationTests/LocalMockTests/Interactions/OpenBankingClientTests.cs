// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using TestStack.BDDfy.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.LocalMockTests.Interactions
{
    public class OpenBankingClientTests : BaseLocalMockTest
    {
        [BddfyFact(Skip = "Upset not appropriate for BankClientProfile")]
        public async Task Repository_ClientInserted()
        {
            //IRequestBuilder builder = CreateOpenBankingRequestBuilder();

            string issuerUrl = "http://aaa.com/";
            string xfapi = "xfapi";
            string softwareStatementId = "softwareStatement";

            // BankRegistrationContext ctx = builder.BankRegistrations();
            //
            // FluentResponse<BankRegistrationResponse> result = await ctx
            //     //.Id("BankClientProfileId")
            //     //.IssuerUrl(new Uri(issuerUrl))
            //     //.XFapiFinancialId(xfapi)
            //     .SoftwareStatementProfileId(softwareStatementId)
            //     .HttpMtlsOverrides(
            //         new HttpMtlsConfigurationOverrides
            //         {
            //             TlsCertificateVerification = "aaa",
            //             TlsRenegotiationSupport = "bbb"
            //         })
            //     .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
            //     .OpenIdOverrides(
            //         new OpenIdConfigurationOverrides
            //         {
            //             RegistrationEndpointUrl = "http://bbb.com"
            //         })
            //     .RegistrationResponseOverrides(new RegistrationResponseJsonOptions())
            //     .UpsertAsync();
            //
            // result.Messages.Should().HaveCount(0);
            //
            // IDbEntityRepository<BankRegistration> repo = ctx.Context.BankRegistrationRepository;
            // IQueryable<BankRegistration> instances = await repo.GetAsync(x => true);
            // BankRegistration persistedResult = instances.FirstOrDefault();

            //persistedResult.IssuerUrl.Should().Be(issuerUrl);
            //persistedResult.XFapiFinancialId.Should().Be(xfapi);
            //persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId);
        }

        [BddfyFact(Skip = "Upset not appropriate for BankClientProfile")]
        public async Task Repository_ClientUpserted()
        {
            // BankRegistrationContext ctx = CreateOpenBankingRequestBuilder().BankRegistrations();
            //
            // string issuerUrl = "http://aaa.com/";
            // string xfapi2 = "xfapi2";
            // string softwareStatementId2 = "softwareStatementId2";
            //
            //
            // FluentResponse<BankRegistrationResponse> result1 = await ctx
            //     //.Id("BankClientProfileId")
            //     //.IssuerUrl(new Uri(issuerUrl))
            //     //.XFapiFinancialId("xfapi")
            //     .SoftwareStatementProfileId("softwareStatement")
            //     .HttpMtlsOverrides(
            //         new HttpMtlsConfigurationOverrides
            //         {
            //             TlsCertificateVerification = "aaa",
            //             TlsRenegotiationSupport = "bbb"
            //         })
            //     .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
            //     .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
            //     .RegistrationResponseOverrides(new RegistrationResponseJsonOptions())
            //     .UpsertAsync();
            //
            // result1.Messages.Should().HaveCount(0);
            //
            // FluentResponse<BankRegistrationResponse> result2 = await ctx
            //     //.IssuerUrl(new Uri(issuerUrl))
            //     //.XFapiFinancialId(xfapi2)
            //     .SoftwareStatementProfileId(softwareStatementId2)
            //     .HttpMtlsOverrides(
            //         new HttpMtlsConfigurationOverrides
            //         {
            //             TlsCertificateVerification = "aaa",
            //             TlsRenegotiationSupport = "bbb"
            //         })
            //     .RegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
            //     .OpenIdRegistrationEndpointUrl(new Uri("http://bbb.com"))
            //     .RegistrationResponseOverrides(new RegistrationResponseJsonOptions())
            //     .UpsertAsync();
            // result2.Messages.Should().HaveCount(0);
            //
            //
            // IDbEntityRepository<BankRegistration> repo = ctx.Context.BankRegistrationRepository;
            //
            // IQueryable<BankRegistration> instances = await repo.GetAsync(x => true);
            // BankRegistration persistedResult = instances.FirstOrDefault();
            //

            //persistedResult.IssuerUrl.Should().Be(issuerUrl);
            //persistedResult.XFapiFinancialId.Should().Be(xfapi2);
            //persistedResult.SoftwareStatementProfileId.Should().Be(softwareStatementId2);
        }
    }
}
