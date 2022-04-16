﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FluentAssertions;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

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
            var softwareStatementProfileId = "0";
            var obTransportCertificatProfileId = "0";
            var obSigningCertificatProfileId = "0";

            // Use default OBC settings
            var obcSettingsProvider =
                new DefaultSettingsProvider<SoftwareStatementAndCertificateProfileOverridesSettings>(
                    new SoftwareStatementAndCertificateProfileOverridesSettings
                    {
                        //SoftwareStatementProfileIds = softwareStatementProfileId
                    });

            // Create software statement profiles
            var softwareStatementProfile = new SoftwareStatementProfileWithOverrideProperties
            {
                SoftwareStatement =
                    "header.ewogICJzb2Z0d2FyZV9pZCI6ICJpZCIsCiAgInNvZnR3YXJlX2NsaWVudF9pZCI6ICJjbGllbnRfaWQiLAogICJzb2Z0d2FyZV9jbGllbnRfbmFtZSI6ICJUUFAgQ2xpZW50IiwKICAic29mdHdhcmVfY2xpZW50X2Rlc2NyaXB0aW9uIjogIkNsaWVudCBkZXNjcmlwdGlvbiIsCiAgInNvZnR3YXJlX3ZlcnNpb24iOiAxLAogICJzb2Z0d2FyZV9jbGllbnRfdXJpIjogImh0dHBzOi8vZXhhbXBsZS5jb20iLAogICJzb2Z0d2FyZV9yZWRpcmVjdF91cmlzIjogWwogICAgImh0dHBzOi8vZXhhbXBsZS5jb20iCiAgXSwKICAic29mdHdhcmVfcm9sZXMiOiBbCiAgICAiQUlTUCIsCiAgICAiUElTUCIsCiAgICAiQ0JQSUkiCiAgXSwKICAib3JnX2lkIjogIm9yZ19pZCIsCiAgIm9yZ19uYW1lIjogIk9yZyBOYW1lIiwKICAic29mdHdhcmVfb25fYmVoYWxmX29mX29yZyI6ICJPcmcgTmFtZSIKfQ==.signature",
                TransportCertificateProfileId = obTransportCertificatProfileId,
                SigningCertificateProfileId = obSigningCertificatProfileId,
                DefaultFragmentRedirectUrl = "http://redirecturl.com",
            };
            var transportCertificateProfile = new TransportCertificateProfileWithOverrideProperties
            {
                AssociatedKey = _mockData.GetMockPrivateKey(),
                Certificate = _mockData.GetMockCertificate(),
                CertificateType = TransportCertificateType.OBLegacy
            };
            var obSigningCertificateProfile = new SigningCertificateProfile
            {
                AssociatedKeyId = "signingkeyid",
                AssociatedKey = _mockData.GetMockPrivateKey(),
                Certificate = _mockData.GetMockCertificate(),
                CertificateType = SigningCertificateType.OBLegacy
            };

            var softwareStatementProfilesSettingsProvider =
                new DefaultSettingsProvider<SoftwareStatementProfilesSettings>(
                    new SoftwareStatementProfilesSettings
                        { [softwareStatementProfileId] = softwareStatementProfile });
            var obTransportCertificateProfilesSettingsProvider =
                new DefaultSettingsProvider<TransportCertificateProfilesSettings>(
                    new TransportCertificateProfilesSettings
                        { [obTransportCertificatProfileId] = transportCertificateProfile });
            var obSigningCertificateProfilesSettingsProvider =
                new DefaultSettingsProvider<SigningCertificateProfilesSettings>(
                    new SigningCertificateProfilesSettings
                        { [obSigningCertificatProfileId] = obSigningCertificateProfile });

            // Set up request builder
            var softwareStatementProfilesRepository = new ProcessedSoftwareStatementProfileStore(
                obcSettingsProvider,
                softwareStatementProfilesSettingsProvider,
                obTransportCertificateProfilesSettingsProvider,
                obSigningCertificateProfilesSettingsProvider,
                new ConsoleInstrumentationClient());
            IRequestBuilder requestBuilder = _mockData.CreateMockRequestBuilder(softwareStatementProfilesRepository);

            // softwareStatementProfileResp.Messages.Should().BeEmpty();
            // softwareStatementProfileResp.Data.Should().NotBeNull();
            // softwareStatementProfileResp.Data.Id.Should().NotBeNullOrWhiteSpace();

            _mockPaymentsServer.SetUpOpenIdMock();
            _mockPaymentsServer.SetupRegistrationMock();

            // Create bank
            var bankRequest = new Bank
            {
                IssuerUrl = MockRoutes.Url,
                FinancialId = _mockData.GetFapiHeader(),
                Name = "MyBank"
            };
            IFluentResponse<BankResponse> bankResp = requestBuilder.BankConfiguration
                .Banks
                .CreateLocalAsync(bankRequest)
                .Result;
            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            var registrationRequest = new BankRegistration
            {
                SoftwareStatementProfileId = softwareStatementProfileId,
                ClientRegistrationApi = DynamicClientRegistrationApiVersion.Version3p3,
                BankId = bankId,
                RegistrationScope = RegistrationScopeEnum.PaymentInitiation,
                AllowMultipleRegistrations = false
            };
            IFluentResponse<BankRegistrationResponse> bankRegistrationResp = requestBuilder.BankConfiguration
                .BankRegistrations
                .CreateAsync(registrationRequest)
                .Result;

            bankRegistrationResp.Should().NotBeNull();
            bankRegistrationResp.Messages.Should().BeEmpty();
            bankRegistrationResp.Data.Should().NotBeNull();
            Guid bankRegistrationId = bankRegistrationResp.Data!.Id;

            // Create bank API information
            var apiSetRequest = new PaymentInitiationApiRequest
            {
                BankId = bankId,
                ApiVersion = PaymentInitiationApiVersion.Version3p1p6,
                BaseUrl = MockRoutes.Url
            };
            IFluentResponse<PaymentInitiationApiResponse> bankApiInformationResponse = requestBuilder
                .BankConfiguration
                .PaymentInitiationApis
                .CreateLocalAsync(apiSetRequest)
                .Result;

            bankApiInformationResponse.Should().NotBeNull();
            bankApiInformationResponse.Messages.Should().BeEmpty();
            bankApiInformationResponse.Data.Should().NotBeNull();
            Guid bankApiInformationId = bankApiInformationResponse.Data!.Id;

            // _mockPaymentsServer.SetupTokenEndpointMock();
            // _mockPaymentsServer.SetupPaymentEndpointMock();

            // DomesticPaymentConsent domesticConsentPaymentRequest = new DomesticPaymentConsent
            // {
            //     WriteDomesticConsent = DomesticPaymentFunctionalSubtest.DomesticPaymentConsent(
            //         DomesticPaymentFunctionalSubtestEnum.PersonToMerchantSubtest,
            //         "placeholder: OBC consent ID",
            //         "placeholder: random GUID"),
            //     BankRegistrationId = Guid.Empty,
            //     BankApiInformationId = Guid.Empty
            // };
            // domesticConsentPaymentRequest.BankRegistrationId = bankRegistrationId;
            // domesticConsentPaymentRequest.BankApiInformationId = bankApiInformationId;
            // IFluentResponse<DomesticPaymentConsentResponse> consentResp;
            // try
            // {
            //     consentResp = requestBuilder.PaymentInitiation
            //         .DomesticPaymentConsents
            //         .PostAsync(domesticConsentPaymentRequest)
            //         .Result;
            //     consentResp.Should().NotBeNull();
            //     consentResp.Messages.Should().BeEmpty();
            //     consentResp.Data.Should().NotBeNull();
            // }
            // catch
            // {
            //     IEnumerable<ILogEntry> x = _mockPaymentsServer.GetLogEntries();
            //     throw;
            // }

            // Check redirect matches above.
            // Check scope = "openid payments"
            // Check response type = "code id_token"
            // Check client ID matches above

            // Call mock bank with auth URL and check.... then return at least auth code and state....

            // Call Fluent method to pass auth code and state (set up mock bank token endpoint)
            // IFluentResponse<AuthorisationRedirectObjectResponse> authCallbackDataResp = requestBuilder
            //     .PaymentInitiation
            //     .AuthorisationRedirectObjects
            //     .PostAsync(
            //         new AuthorisationRedirectObject(
            //             "fragment",
            //             new AuthResult(
            //                 "TODO: place idToken from mock bank here",
            //                 "TODO: place code from mock bank here",
            //                 "TODO: extract state from consentResp.Data.AuthUrl and place here",
            //                 "TODO: extract nonce from consentResp.Data.AuthUrl and place here")))
            //     .Result;

            //authCallbackDataResp.Should().NotBeNull();
            //authCallbackDataResp.Messages.Should().BeEmpty();
            //authCallbackDataResp.Data.Should().NotBeNull();

            // Call Fluent method to make payment (set up mock bank payment endpoint)
            // IFluentResponse<DomesticPaymentResponse> paymentResp = requestBuilder.PaymentInitiation
            //     .DomesticPayments
            //     .PostAsync(
            //         new DomesticPayment
            //         {
            //             DomesticPaymentConsentId = consentResp.Data!.Id
            //         })
            //     .Result;

            //paymentResp.Should().NotBeNull();
            //paymentResp.Messages.Should().BeEmpty();
            //paymentResp.Data.Should().NotBeNull();

            // All done!
        }
    }
}
