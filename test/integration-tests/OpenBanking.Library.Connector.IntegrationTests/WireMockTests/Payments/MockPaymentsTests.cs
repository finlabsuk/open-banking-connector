// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
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
            IRequestBuilder requestBuilder = _mockData.CreateMockRequestBuilder();

            FluentResponse<SoftwareStatementProfileResponse> softwareStatementProfileResp = requestBuilder
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

            FluentResponse<BankRegistrationResponse> bankClientProfileResp = requestBuilder.BankRegistrations
                .PostAsync(
                    new BankRegistration
                    {
                        SoftwareStatementProfileId = softwareStatementProfileResp.Data.Id,
                        //BankName = ,
                    })
                //.Id("MyBankClientProfileId")
                //.IssuerUrl(new Uri(MockRoutes.Url))
                //.XFapiFinancialId(_mockData.GetFapiHeader())
                .Result;

            bankClientProfileResp.Should().NotBeNull();
            bankClientProfileResp.Messages.Should().BeEmpty();
            bankClientProfileResp.Data.Should().NotBeNull();

            BankProfile bankProfile = new BankProfile
            {
                BankRegistrationId = bankClientProfileResp.Data.Id,
                //BankName = ,
                //UseStagingBankRegistration = ,
                //ReplaceDefaultBankProfile = ,
                //ReplaceStagingBankProfile = ,
                PaymentInitiationApi = new PaymentInitiationApi
                {
                    ApiVersion = ApiVersion.V3P1P4,
                    BaseUrl = MockRoutes.Url
                }
            };

            FluentResponse<BankProfileResponse> paymentInitiationApiProfileResp = requestBuilder
                .BankProfiles
                .PostAsync(bankProfile)
                //.Id("NewPaymentInitiationProfile")
                .Result;

            paymentInitiationApiProfileResp.Should().NotBeNull();
            paymentInitiationApiProfileResp.Messages.Should().BeEmpty();
            paymentInitiationApiProfileResp.Data.Should().NotBeNull();

            _mockPaymentsServer.SetupTokenEndpointMock();
            _mockPaymentsServer.SetupPaymentEndpointMock();

            FluentResponse<DomesticPaymentConsentResponse> consentResp = requestBuilder
                .DomesticPaymentConsents
                .PostAsync(
                    new DomesticPaymentConsent
                    {
                        Merchant = new OBRisk1
                        {
                            PaymentContextCode = OBRisk1.PaymentContextCodeEnum.EcommerceGoods,
                            DeliveryAddress = new OBRisk1DeliveryAddress
                            {
                                BuildingNumber = "42",
                                StreetName = "Oxford Street",
                                TownName = "London",
                                Country = "UK",
                                PostCode = "SW1 1AA"
                            }
                        },
                        CreditorAccount = new OBWriteDomestic2DataInitiationCreditorAccount
                        {
                            SchemeName = "ACME DIY",
                            Identification = "BE56456394728288",
                            Name = "IBAN",
                            SecondaryIdentification = "secondary-identif"
                        },
                        InstructedAmount = new OBWriteDomestic2DataInitiationInstructedAmount
                        {
                            Amount = "50.00",
                            Currency = "GBP"
                        },
                        InstructionIdentification = "instr-identification",
                        EndToEndIdentification = "e2e-identification",
                        RemittanceInformation = new OBWriteDomestic2DataInitiationRemittanceInformation
                        {
                            Unstructured = "Tools",
                            Reference = "Tools"
                        },
                        BankProfileId = paymentInitiationApiProfileResp.Data.Id,
                        //BankName = ,
                        //UseStagingBankProfile = 
                    })
                .Result;

            consentResp.Should().NotBeNull();
            consentResp.Messages.Should().BeEmpty();
            consentResp.Data.Should().NotBeNull();

            // Check redirect matches above.
            // Check scope = "openid payments"
            // Check response type = "code id_token"
            // Check client ID matches above

            // Call mock bank with auth URL and check.... then return at least auth code and state....

            // Call Fluent method to pass auth code and state (set up mock bank token endpoint)
            FluentResponse<AuthorisationRedirectObjectResponse> authCallbackDataResp = requestBuilder
                .AuthorisationRedirectObjects
                .PostAsync(
                    new AuthorisationRedirectObject(
                        responseMode: "fragment",
                        response: new AuthorisationCallbackPayload
                        {
                            Code = "TODO: place code from mock bank here",
                            State = "TODO: extract state from consentResp.Data.AuthUrl and place here",
                            Nonce = "TODO: extract nonce from consentResp.Data.AuthUrl and place here"
                        }))
                .Result;

            //authCallbackDataResp.Should().NotBeNull();
            //authCallbackDataResp.Messages.Should().BeEmpty();
            //authCallbackDataResp.Data.Should().NotBeNull();

            // Call Fluent method to make payment (set up mock bank payment endpoint)
            FluentResponse<DomesticPaymentResponse> paymentResp = requestBuilder.DomesticPayments
                .PostAsync(
                    new DomesticPayment
                    {
                        ConsentId = consentResp.Data.Id
                    })
                .Result;

            //paymentResp.Should().NotBeNull();
            //paymentResp.Messages.Should().BeEmpty();
            //paymentResp.Data.Should().NotBeNull();

            // All done!
        }
    }
}
