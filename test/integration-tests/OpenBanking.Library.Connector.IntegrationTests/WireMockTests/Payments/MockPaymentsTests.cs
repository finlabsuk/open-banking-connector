// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FluentAssertions;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests.Payments
{
    public class MockPaymentsTests
    {
        private readonly IMockPaymentData _mockData;
        private readonly IMockPaymentServer _mockPaymentsServer;

        public MockPaymentsTests(IMockPaymentData mockData, IMockPaymentServer mockPaymentsServer)
        {
            _mockData = mockData;
            _mockPaymentsServer = mockPaymentsServer;
        }

        public void RunMockPaymentTest()
        {
            IOpenBankingRequestBuilder requestBuilder = _mockData.CreateMockRequestBuilder();

            OpenBankingSoftwareStatementResponse softwareStatementProfileResp = requestBuilder
                .SoftwareStatementProfile()
                .Id("0")
                .SoftwareStatement(
                    "header.ewogICJzb2Z0d2FyZV9pZCI6ICJpZCIsCiAgInNvZnR3YXJlX2NsaWVudF9pZCI6ICJjbGllbnRfaWQiLAogICJzb2Z0d2FyZV9jbGllbnRfbmFtZSI6ICJUUFAgQ2xpZW50IiwKICAic29mdHdhcmVfY2xpZW50X2Rlc2NyaXB0aW9uIjogIkNsaWVudCBkZXNjcmlwdGlvbiIsCiAgInNvZnR3YXJlX3ZlcnNpb24iOiAxLAogICJzb2Z0d2FyZV9jbGllbnRfdXJpIjogImh0dHBzOi8vZXhhbXBsZS5jb20iLAogICJzb2Z0d2FyZV9yZWRpcmVjdF91cmlzIjogWwogICAgImh0dHBzOi8vZXhhbXBsZS5jb20iCiAgXSwKICAic29mdHdhcmVfcm9sZXMiOiBbCiAgICAiQUlTUCIsCiAgICAiUElTUCIsCiAgICAiQ0JQSUkiCiAgXSwKICAib3JnX2lkIjogIm9yZ19pZCIsCiAgIm9yZ19uYW1lIjogIk9yZyBOYW1lIiwKICAic29mdHdhcmVfb25fYmVoYWxmX29mX29yZyI6ICJPcmcgTmFtZSIKfQ==.signature")
                .SigningKeyInfo(
                    keyId: "signingkeyid",
                    keySecretName: _mockData.GetMockPrivateKey(),
                    certificate: _mockData.GetMockCertificate())
                .TransportKeyInfo(
                    keySecretName: _mockData.GetMockPrivateKey(),
                    certificate: _mockData.GetMockCertificate())
                .DefaultFragmentRedirectUrl("http://redirecturl.com")
                .SubmitAsync().Result;

            softwareStatementProfileResp.Messages.Should().BeEmpty();
            softwareStatementProfileResp.Data.Should().NotBeNull();
            softwareStatementProfileResp.Data.Id.Should().NotBeNullOrWhiteSpace();

            _mockPaymentsServer.SetUpOpenIdMock();
            _mockPaymentsServer.SetupRegistrationMock();

            BankClientProfileFluentResponse bankClientProfileResp = requestBuilder.BankClientProfile()
                .Id("MyBankClientProfileId")
                .SoftwareStatementProfileId(softwareStatementProfileResp.Data.Id)
                .IssuerUrl(new Uri(MockRoutes.Url))
                .XFapiFinancialId(_mockData.GetFapiHeader())
                .BankClientRegistrationClaimsOverrides(new BankClientRegistrationClaimsOverrides())
                .SubmitAsync().Result;

            bankClientProfileResp.Should().NotBeNull();
            bankClientProfileResp.Messages.Should().BeEmpty();
            bankClientProfileResp.Data.Should().NotBeNull();

            PaymentInitiationApiProfileFluentResponse paymentInitiationApiProfileResp = requestBuilder
                .PaymentInitiationApiProfile()
                .Id("NewPaymentInitiationProfile")
                .BankClientProfileId(bankClientProfileResp.Data.Id)
                .PaymentInitiationApiInfo(apiVersion: ApiVersion.V3P1P4, baseUrl: MockRoutes.Url)
                .SubmitAsync().Result;

            paymentInitiationApiProfileResp.Should().NotBeNull();
            paymentInitiationApiProfileResp.Messages.Should().BeEmpty();
            paymentInitiationApiProfileResp.Data.Should().NotBeNull();

            _mockPaymentsServer.SetupTokenEndpointMock();
            _mockPaymentsServer.SetupPaymentEndpointMock();

            DomesticPaymentConsentFluentResponse consentResp = requestBuilder
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
                .SubmitAsync().Result;

            consentResp.Should().NotBeNull();
            consentResp.Messages.Should().BeEmpty();
            consentResp.Data.Should().NotBeNull();

            // Check redirect matches above.
            // Check scope = "openid payments"
            // Check response type = "code id_token"
            // Check client ID matches above

            // Call mock bank with auth URL and check.... then return at least auth code and state....

            // Call Fluent method to pass auth code and state (set up mock bank token endpoint)
            AuthorisationCallbackDataFluentResponse authCallbackDataResp = requestBuilder.AuthorisationCallbackData()
                .ResponseMode("fragment")
                .Response(
                    new AuthorisationCallbackPayload(
                        authorisationCode: "TODO: place code from mock bank here",
                        state: "TODO: extract state from consentResp.Data.AuthUrl and place here")
                    {
                        Nonce = "TODO: extract nonce from consentResp.Data.AuthUrl and place here"
                    })
                .SubmitAsync().Result;

            //authCallbackDataResp.Should().NotBeNull();
            //authCallbackDataResp.Messages.Should().BeEmpty();
            //authCallbackDataResp.Data.Should().NotBeNull();

            // Call Fluent method to make payment (set up mock bank payment endpoint)
            DomesticPaymentFluentResponse paymentResp = requestBuilder.DomesticPayment()
                .ConsentId(consentResp.Data.ConsentId)
                .SubmitAsync().Result;

            //paymentResp.Should().NotBeNull();
            //paymentResp.Messages.Should().BeEmpty();
            //paymentResp.Data.Should().NotBeNull();

            // All done!
        }
    }
}
