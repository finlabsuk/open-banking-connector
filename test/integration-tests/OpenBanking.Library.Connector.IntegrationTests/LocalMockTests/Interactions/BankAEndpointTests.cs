// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FluentAssertions;
using TestStack.BDDfy.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.LocalMockTests.Interactions
{
    public class BankAEndpointTests : BaseLocalMockTest
    {
        [BddfyFact]
        public async Task Submit_DomesticPayment_Accepted()
        {
            IOpenBankingRequestBuilder? builder = CreateOpenBankingRequestBuilder();

            // Where you see TestConfig.GetValue( .. )
            // these are injecting test data values. Here they're from test data, but can be anything else: database queries, Azure Key Vault configuration, etc.

            // Set up a Software Statement and associated configuration/preferences (a "software statement profile") within our Servicing service. Configuration
            // includes the transport and signing keys associated with the software statement.
            // This is performed once in the lifetime of Servicing or in the case of using multiple identities to connect to banks, once per identity.
            OpenBankingSoftwareStatementResponse? softwareStatementProfileResp = await builder
                .SoftwareStatementProfile()
                .Id("0")
                .SoftwareStatement(TestConfig.GetValue("softwarestatement"))
                .SigningKeyInfo(
                    keyId: TestConfig.GetValue("signingkeyid"),
                    keySecretName: TestConfig.GetValue("signingcertificatekey"),
                    certificate: TestConfig.GetValue("signingcertificate"))
                .TransportKeyInfo(
                    keySecretName: TestConfig.GetValue("transportcertificatekey"),
                    certificate: TestConfig.GetValue("transportcertificate"))
                .DefaultFragmentRedirectUrl(TestConfig.GetValue("defaultfragmentredirecturl"))
                .SubmitAsync();

            softwareStatementProfileResp.Should().NotBeNull();
            softwareStatementProfileResp.Messages.Should().BeEmpty();
            softwareStatementProfileResp.Data.Should().NotBeNull();
            softwareStatementProfileResp.Data.Id.Should().NotBeNullOrWhiteSpace();

            // Set up a bank registration (client) and associated configuration/preferences (a "bank client profile"). The bank client profile will only
            // be created if bank registration is successful.
            // Bank registration uses a software statement to establish our identity in the eyes of the bank. This and other data are transmitted to the bank.
            // This is performed once per Bank.
            BankClientProfileFluentResponse? bankClientProfileResp = await builder.BankClientProfile()
                .Id("MyBankClientProfileId")
                .SoftwareStatementProfileId(softwareStatementProfileResp.Data.Id)
                .IssuerUrl(new Uri(TestConfig.GetValue("clientProfileIssuerUrl")))
                .XFapiFinancialId(TestConfig.GetValue("xFapiFinancialId"))
                .BankClientRegistrationClaimsOverrides(TestConfig.GetOpenBankingClientRegistrationClaimsOverrides())
                .SubmitAsync();

            // These are test assertions to ensure the bank says "request successfully processed". "Messages" enumerates all warnings & errors, and so is empty for this scenario.
            bankClientProfileResp.Should().NotBeNull();
            bankClientProfileResp.Messages.Should().BeEmpty();
            bankClientProfileResp.Data.Should().NotBeNull();

            // Set up an Open Banking Read/Write API and associated configuration/preferences (an "API profile"). This should be done separately for Payment Initiation and Account Transaction
            // APIs and specifies the functional endpoints, spec version and
            // any preferences related to that API including the bank client to be used.
            // This is performed once per Bank per Read/Write API type (e.g. Payment Initiation, Account Transaction, etc).
            PaymentInitiationApiProfileFluentResponse? paymentInitiationApiProfileResp = await builder
                .PaymentInitiationApiProfile()
                .Id("NewPaymentInitiationProfile")
                .BankClientProfileId(bankClientProfileResp.Data.Id)
                .PaymentInitiationApiInfo(
                    apiVersion: TestConfig.GetEnumValue<ApiVersion>("paymentApiVersion").Value,
                    baseUrl: TestConfig.GetValue("paymentApiUrl"))
                .SubmitAsync();

            paymentInitiationApiProfileResp.Should().NotBeNull();
            paymentInitiationApiProfileResp.Messages.Should().BeEmpty();
            paymentInitiationApiProfileResp.Data.Should().NotBeNull();

            // Create a consent object and transmit to the bank to support user authorisation of an intended domestic payment.
            // This is performed once per payment.
            DomesticPaymentConsentFluentResponse? consentResp = await builder
                .DomesticPaymentConsent(paymentInitiationApiProfileResp.Data.Id)
                .Merchant(
                    merchantCategory: null,
                    merchantCustomerId: null,
                    paymentContextCode: OBRisk1.PaymentContextCodeEnum.EcommerceGoods)
                .CreditorAccount(
                    identification: "BE56456394728288",
                    schema: "IBAN",
                    name: "ACME DIY",
                    secondaryId: "secondary-identif")
                .DeliveryAddress(new OBRisk1DeliveryAddress
                    {
                        BuildingNumber = "42",
                        StreetName = "Oxford Street",
                        TownName = "London",
                        Country = "UK",
                        PostCode = "SW1 1AA"
                    })
                .Amount(currency: "GBP", value: 50)
                .InstructionIdentification("instr-identification")
                .EndToEndIdentification("e2e-identification")
                .Remittance(new OBWriteDomestic2DataInitiationRemittanceInformation
                    {
                        Unstructured = "Tools",
                        Reference = "Tools"
                    })
                .SubmitAsync();

            consentResp.Should().NotBeNull();
            consentResp.Messages.Should().BeEmpty();
            consentResp.Data.Should().NotBeNull();

            // After successful consent creation, a URL is generated which may be called to allow the user to authorise the payment.
            // After authorisation, the bank will asynchronously call back (redirect) to us with an authorisation code. 
            // We provide the endpoint to receive this auth code, and then can complete the payment process (a flow that is significantly simpler than the above).
        }
    }
}
