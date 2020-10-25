// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.ObApi.Base.Json;
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
            // Set up a Software Statement and associated configuration/preferences (a "software statement profile") within our Servicing service. Configuration
            // includes the transport and signing keys associated with the software statement.
            // This is performed once in the lifetime of Servicing or in the case of using multiple identities to connect to banks, once per identity.
            string softwareStatementProfileId = "0";
            SoftwareStatementProfile softwareStatementProfile = new SoftwareStatementProfile
            {
                SoftwareStatement = TestConfig.GetValue("softwarestatement"),
                SigningKeyId = TestConfig.GetValue("signingkeyid"),
                SigningKey = TestConfig.GetValue("signingcertificatekey"),
                SigningCertificate = TestConfig.GetValue("signingcertificate"),
                TransportKey = TestConfig.GetValue("transportcertificatekey"),
                TransportCertificate = TestConfig.GetValue("transportcertificate"),
                DefaultFragmentRedirectUrl = TestConfig.GetValue("defaultfragmentredirecturl"),
                Id = softwareStatementProfileId
            };
            Models.KeySecrets.Cached.SoftwareStatementProfile softwareStatementProfileCached =
                new Models.KeySecrets.Cached.SoftwareStatementProfile(
                    profileKeySecrets: softwareStatementProfile,
                    apiClient: new ApiClient(new HttpClient()));
            ConcurrentDictionary<string, Models.KeySecrets.Cached.SoftwareStatementProfile>
                softwareStatementProfileDictionary =
                    new ConcurrentDictionary<string, Models.KeySecrets.Cached.SoftwareStatementProfile>
                        { [softwareStatementProfileCached.Id] = softwareStatementProfileCached };
            IRequestBuilder builder = CreateOpenBankingRequestBuilder(softwareStatementProfileDictionary);

            // softwareStatementProfileResp.Should().NotBeNull();
            // softwareStatementProfileResp.Messages.Should().BeEmpty();
            // softwareStatementProfileResp.Data.Should().NotBeNull();
            // softwareStatementProfileResp.Data.Id.Should().NotBeNullOrWhiteSpace();


            // Where you see TestConfig.GetValue( .. )
            // these are injecting test data values. Here they're from test data, but can be anything else: database queries, Azure Key Vault configuration, etc.

            // Create bank
            string bankName = "MyBank";
            Bank bank = new Bank
            {
                IssuerUrl = TestConfig.GetValue("clientProfileIssuerUrl"),
                FinancialId = TestConfig.GetValue("xFapiFinancialId"),
                Name = bankName,
                RegistrationScopeApiSet = RegistrationScopeApiSet.All
            };
            FluentResponse<BankResponse> bankResp = await builder.ClientRegistration
                .Banks
                .PostAsync(bank);

            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

            // Set up a bank registration (client) and associated configuration/preferences (a "bank client profile"). The bank client profile will only
            // be created if bank registration is successful.
            // Bank registration uses a software statement to establish our identity in the eyes of the bank. This and other data are transmitted to the bank.
            // This is performed once per Bank.
            FluentResponse<BankRegistrationResponse> bankClientProfileResp = await builder.ClientRegistration
                .BankRegistrations
                .PostAsync(
                    new BankRegistration
                    {
                        SoftwareStatementProfileId = softwareStatementProfileId,
                        BankRegistrationClaimsOverrides =
                            TestConfig.GetOpenBankingClientRegistrationClaimsOverrides(),
                        BankRegistrationResponseJsonOptions = new BankRegistrationResponseJsonOptions
                        {
                            DelimitedStringConverterOptions = DelimitedStringConverterOptions.JsonStringArrayNotString
                        },
                        BankId = bankId,
                        ReplaceStagingBankRegistration = true
                    });

            // These are test assertions to ensure the bank says "request successfully processed". "Messages" enumerates all warnings & errors, and so is empty for this scenario.
            bankClientProfileResp.Should().NotBeNull();
            bankClientProfileResp.Messages.Should().BeEmpty();
            bankClientProfileResp.Data.Should().NotBeNull();

            // Set up an Open Banking Read/Write API and associated configuration/preferences (an "API profile"). This should be done separately for Payment Initiation and Account Transaction
            // APIs and specifies the functional endpoints, spec version and
            // any preferences related to that API including the bank client to be used.
            // This is performed once per Bank per Read/Write API type (e.g. Payment Initiation, Account Transaction, etc).
            FluentResponse<BankProfileResponse> paymentInitiationApiProfileResp = await builder.ClientRegistration
                .BankProfiles
                .PostAsync(
                    new BankProfile
                    {
                        BankId = bankId,
                        ReplaceStagingBankProfile = true,
                        PaymentInitiationApi = new PaymentInitiationApi
                        {
                            ApiVersion = TestConfig.GetEnumValue<ApiVersion>("paymentApiVersion").Value,
                            BaseUrl = TestConfig.GetValue("paymentApiUrl")
                        }
                    });

            paymentInitiationApiProfileResp.Should().NotBeNull();
            paymentInitiationApiProfileResp.Messages.Should().BeEmpty();
            paymentInitiationApiProfileResp.Data.Should().NotBeNull();

            // Create a consent object and transmit to the bank to support user authorisation of an intended domestic payment.
            // This is performed once per payment.
            FluentResponse<DomesticPaymentConsentResponse> consentResp = await builder.PaymentInitiation
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
                    });

            consentResp.Should().NotBeNull();
            consentResp.Messages.Should().BeEmpty();
            consentResp.Data.Should().NotBeNull();

            // After successful consent creation, a URL is generated which may be called to allow the user to authorise the payment.
            // After authorisation, the bank will asynchronously call back (redirect) to us with an authorisation code. 
            // We provide the endpoint to receive this auth code, and then can complete the payment process (a flow that is significantly simpler than the above).
        }
    }
}
