// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.ClientRegistration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FluentAssertions;
using WireMock.Logging;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.OpenBankingUk.ReadWriteApi.V3p1p6.PaymentInitiation.Models;

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
            string softwareStatementProfileId = "0";

            // Use default OBC settings
            var obcSettingsProvider =
                new DefaultSettingsProvider<OpenBankingConnectorSettings>(
                    new OpenBankingConnectorSettings
                    {
                        SoftwareStatementProfileIds = softwareStatementProfileId
                    });

            // Create software statement profiles
            SoftwareStatementProfile softwareStatementProfile = new SoftwareStatementProfile
            {
                SoftwareStatement =
                    "header.ewogICJzb2Z0d2FyZV9pZCI6ICJpZCIsCiAgInNvZnR3YXJlX2NsaWVudF9pZCI6ICJjbGllbnRfaWQiLAogICJzb2Z0d2FyZV9jbGllbnRfbmFtZSI6ICJUUFAgQ2xpZW50IiwKICAic29mdHdhcmVfY2xpZW50X2Rlc2NyaXB0aW9uIjogIkNsaWVudCBkZXNjcmlwdGlvbiIsCiAgInNvZnR3YXJlX3ZlcnNpb24iOiAxLAogICJzb2Z0d2FyZV9jbGllbnRfdXJpIjogImh0dHBzOi8vZXhhbXBsZS5jb20iLAogICJzb2Z0d2FyZV9yZWRpcmVjdF91cmlzIjogWwogICAgImh0dHBzOi8vZXhhbXBsZS5jb20iCiAgXSwKICAic29mdHdhcmVfcm9sZXMiOiBbCiAgICAiQUlTUCIsCiAgICAiUElTUCIsCiAgICAiQ0JQSUkiCiAgXSwKICAib3JnX2lkIjogIm9yZ19pZCIsCiAgIm9yZ19uYW1lIjogIk9yZyBOYW1lIiwKICAic29mdHdhcmVfb25fYmVoYWxmX29mX29yZyI6ICJPcmcgTmFtZSIKfQ==.signature",
                SigningKeyId = "signingkeyid",
                SigningKey = _mockData.GetMockPrivateKey(),
                SigningCertificate = _mockData.GetMockCertificate(),
                TransportKey = _mockData.GetMockPrivateKey(),
                TransportCertificate = _mockData.GetMockCertificate(),
                DefaultFragmentRedirectUrl = "http://redirecturl.com",
            };
            var softwareStatementProfilesSettingsProvider =
                new DefaultSettingsProvider<SoftwareStatementProfilesSettings>(
                    new SoftwareStatementProfilesSettings
                        { [softwareStatementProfileId] = softwareStatementProfile });

            // Set up request builder
            var softwareStatementProfilesRepository = new SoftwareStatementProfileCache(
                obcSettingsProvider,
                softwareStatementProfilesSettingsProvider,
                new ConsoleInstrumentationClient());
            IRequestBuilder requestBuilder = _mockData.CreateMockRequestBuilder(softwareStatementProfilesRepository);

            // softwareStatementProfileResp.Messages.Should().BeEmpty();
            // softwareStatementProfileResp.Data.Should().NotBeNull();
            // softwareStatementProfileResp.Data.Id.Should().NotBeNullOrWhiteSpace();

            _mockPaymentsServer.SetUpOpenIdMock();
            _mockPaymentsServer.SetupRegistrationMock();

            // Create bank
            Bank bankRequest = new Bank
            {
                IssuerUrl = MockRoutes.Url,
                FinancialId = _mockData.GetFapiHeader(),
                Name = "MyBank"
            };
            IFluentResponse<BankPostResponse> bankResp = requestBuilder.ClientRegistration
                .Banks
                .PostAsync(bankRequest)
                .Result;
            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            BankRegistration registrationRequest = new BankRegistration
            {
                SoftwareStatementProfileId = softwareStatementProfileId,
                ClientRegistrationApi = ClientRegistrationApiVersion.Version3p3,
                BankId = bankId,
                RegistrationScope = RegistrationScope.PaymentInitiation,
                AllowMultipleRegistrations = false
            };
            IFluentResponse<BankRegistrationPostResponse> bankRegistrationResp = requestBuilder.ClientRegistration
                .BankRegistrations
                .PostAsync(registrationRequest)
                .Result;

            bankRegistrationResp.Should().NotBeNull();
            bankRegistrationResp.Messages.Should().BeEmpty();
            bankRegistrationResp.Data.Should().NotBeNull();
            Guid bankRegistrationId = bankRegistrationResp.Data!.Id;

            // Create bank API information
            BankApiInformation apiInformationRequest = new BankApiInformation
            {
                BankId = bankId,
                PaymentInitiationApi = new PaymentInitiationApi
                {
                    PaymentInitiationApiVersion = PaymentInitiationApiVersion.Version3p1p6,
                    BaseUrl = MockRoutes.Url
                }
            };
            IFluentResponse<BankApiInformationPostResponse> bankApiInformationResponse = requestBuilder
                .ClientRegistration
                .BankApiInformationObjects
                .PostAsync(apiInformationRequest)
                .Result;

            bankApiInformationResponse.Should().NotBeNull();
            bankApiInformationResponse.Messages.Should().BeEmpty();
            bankApiInformationResponse.Data.Should().NotBeNull();
            Guid bankApiInformationId = bankApiInformationResponse.Data!.Id;

            _mockPaymentsServer.SetupTokenEndpointMock();
            _mockPaymentsServer.SetupPaymentEndpointMock();

            DomesticPaymentConsent domesticConsentPaymentRequest = new DomesticPaymentConsent
            {
                WriteDomesticConsent = DomesticPaymentFunctionalSubtest.DomesticPaymentConsent(
                    DomesticPaymentFunctionalSubtestEnum.PersonToMerchantSubtest,
                    "placeholder: OBC consent ID",
                    "placeholder: random GUID"),
                BankRegistrationId = Guid.Empty,
                BankApiInformationId = Guid.Empty
            };
            domesticConsentPaymentRequest.BankRegistrationId = bankRegistrationId;
            domesticConsentPaymentRequest.BankApiInformationId = bankApiInformationId;
            IFluentResponse<DomesticPaymentConsentPostResponse> consentResp;
            try
            {
                consentResp = requestBuilder.PaymentInitiation
                    .DomesticPaymentConsents
                    .PostAsync(domesticConsentPaymentRequest)
                    .Result;
                consentResp.Should().NotBeNull();
                consentResp.Messages.Should().BeEmpty();
                consentResp.Data.Should().NotBeNull();
            }
            catch
            {
                IEnumerable<ILogEntry> x = _mockPaymentsServer.GetLogEntries();
                throw;
            }

            // Check redirect matches above.
            // Check scope = "openid payments"
            // Check response type = "code id_token"
            // Check client ID matches above

            // Call mock bank with auth URL and check.... then return at least auth code and state....

            // Call Fluent method to pass auth code and state (set up mock bank token endpoint)
            IFluentResponse<AuthorisationRedirectObjectResponse> authCallbackDataResp = requestBuilder
                .PaymentInitiation
                .AuthorisationRedirectObjects
                .PostAsync(
                    new AuthorisationRedirectObject(
                        "fragment",
                        new AuthorisationCallbackPayload(
                            "TODO: place idToken from mock bank here",
                            "TODO: place code from mock bank here",
                            "TODO: extract state from consentResp.Data.AuthUrl and place here",
                            "TODO: extract nonce from consentResp.Data.AuthUrl and place here")))
                .Result;

            //authCallbackDataResp.Should().NotBeNull();
            //authCallbackDataResp.Messages.Should().BeEmpty();
            //authCallbackDataResp.Data.Should().NotBeNull();

            // Call Fluent method to make payment (set up mock bank payment endpoint)
            IFluentResponse<DomesticPaymentPostResponse> paymentResp = requestBuilder.PaymentInitiation
                .DomesticPayments
                .PostAsync(
                    new DomesticPayment
                    {
                        DomesticPaymentConsentId = consentResp.Data!.Id
                    })
                .Result;

            //paymentResp.Should().NotBeNull();
            //paymentResp.Messages.Should().BeEmpty();
            //paymentResp.Data.Should().NotBeNull();

            // All done!
        }
    }
}
