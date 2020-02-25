// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.AccountTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FluentAssertions;
using TestStack.BDDfy.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.LocalMockTests.Interactions
{
    public class FullPaymentTests : BaseLocalMockTest
    {
        [BddfyFact]
        public async Task Submit_SoftwareStatement_ReturnsNonEmpty()
        {
            var builder = CreateOpenBankingRequestBuilder();

            // Where you see TestConfig.GetValue( .. )
            // these are injecting test data values. Here they're from test data, but can be anything else: database queries, Azure Key Vault configuration, etc.

            // Set up a Software Statement within our Servicing service. This should only be done once in the lifetime of Servicing.
            var statementResp = await builder.SoftwareStatementProfile()
                .SoftwareStatementProfileId("0")
                .SoftwareStatement(TestConfig.GetValue("softwarestatement"))
                .SigningKeyId(TestConfig.GetValue("signingkeyid"))
                .SigningCertificate(TestConfig.GetValue("signingcertificatekey"),
                    TestConfig.GetValue("signingcertificate"))
                .TransportCertificate(TestConfig.GetValue("transportcertificatekey"),
                    TestConfig.GetValue("transportcertificate"))
                .DefaultFragmentRedirectUrl(TestConfig.GetValue("defaultfragmentredirecturl"))
                .SubmitAsync();

            statementResp.Messages.Should().BeEmpty();
            statementResp.Data.Should().NotBeNull();
            statementResp.Data.Id.Should().NotBeNullOrWhiteSpace();

            // Register with a bank.  This takes the software statement, collects with other data (below) & transmits to the bank.
            // This is performed once per Bank.
            var clientResp = await builder.ClientProfile(statementResp.Data.Id)
                .IssuerUrl(TestConfig.GetValue("clientProfileIssuerUrl"))
                .SoftwareStatementProfileId(statementResp.Data.Id)
                .XFapiFinancialId(TestConfig.GetValue("xFapiFinancialId"))
                .AccountTransactionApi(TestConfig.GetEnumValue<AccountApiVersion>("accountApiVersion").Value,
                    TestConfig.GetValue("accountApiUrl"))
                .PaymentInitiationApi(TestConfig.GetEnumValue<ApiVersion>("paymentApiVersion").Value,
                    TestConfig.GetValue("paymentApiUrl"))
                .OpenBankingClientRegistrationClaimsOverrides(
                    TestConfig.GetOpenBankingClientRegistrationClaimsOverrides())
                .SubmitAsync();

            // These are test assertions to ensure the bank says "request successfully processed". "Messages" enumerates all warnings & errors, and so is empty for this scenario.
            clientResp.Should().NotBeNull();
            clientResp.Messages.Should().BeEmpty();
            clientResp.Data.Should().NotBeNull();

            // Create a user's consent to make a payment through the bank, then transmit. 
            // This is performed once per payment.
            var consentResp = await builder.DomesticPaymentConsent(clientResp.Data.Id)
                .Merchant(null, null, PaymentContextCode.EcommerceGoods)
                .CreditorAccount("BE56456394728288", "IBAN", "ACME DIY", "secondary-identif")
                .DeliveryAddress(new OBRiskDeliveryAddress
                {
                    BuildingNumber = "42",
                    StreetName = "Oxford Street",
                    TownName = "London",
                    Country = "UK",
                    PostCode = "SW1 1AA"
                })
                .Amount("GBP", 50)
                .InstructionIdentification("instr-identification")
                .EndToEndIdentification("e2e-identification")
                .Remittance("Tools", "Tools")
                .SubmitAsync();

            // After the consent is made, the bank will asynchronously call back to us with an authorisation code. 
            // We will need to provide and endpoint to receive this auth code, and complete the payment process (a flow that is significantly simpler than the above).

            consentResp.Should().NotBeNull();
            consentResp.Messages.Should().BeEmpty();
            consentResp.Data.Should().NotBeNull();
        }
    }
}
