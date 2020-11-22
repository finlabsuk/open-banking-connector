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


            // BANK REGISTRATION

            // Create bank
            FluentResponse<BankResponse> bankResp = await builder.ClientRegistration
                .Banks
                .PostAsync(
                    new Bank
                    {
                        IssuerUrl = TestConfig.GetValue("clientProfileIssuerUrl"),
                        FinancialId = TestConfig.GetValue("xFapiFinancialId"),
                        Name = "MyBank",
                        RegistrationScopeApiSet = RegistrationScopeApiSet.All
                    });

            // Checks
            bankResp.Should().NotBeNull();
            bankResp.Messages.Should().BeEmpty();
            bankResp.Data.Should().NotBeNull();
            Guid bankId = bankResp.Data!.Id;

            // Create bank registration
            FluentResponse<BankRegistrationResponse> bankRegistrationResp = await builder.ClientRegistration
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

            // Checks
            bankRegistrationResp.Should().NotBeNull();
            bankRegistrationResp.Messages.Should().BeEmpty();
            bankRegistrationResp.Data.Should().NotBeNull();

            // Create bank profile
            FluentResponse<BankApiInformationResponse> bankProfileResp = await builder.ClientRegistration
                .BankApiInformationObjects
                .PostAsync(
                    new BankApiInformation
                    {
                        BankId = bankId,
                        ReplaceStagingBankProfile = true,
                        PaymentInitiationApi = new PaymentInitiationApi
                        {
                            ApiVersion = TestConfig.GetEnumValue<ApiVersion>("paymentApiVersion")!.Value,
                            BaseUrl = TestConfig.GetValue("paymentApiUrl")
                        }
                    });

            // Checks
            bankProfileResp.Should().NotBeNull();
            bankProfileResp.Messages.Should().BeEmpty();
            bankProfileResp.Data.Should().NotBeNull();


            // CONSENT CREATION 

            // Create consent
            FluentResponse<DomesticPaymentConsentResponse> domesticPaymentConsentResp = await builder.PaymentInitiation
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
                            SchemeName = "UK.OBIE.SortCodeAccountNumber",
                            Identification = "08080021325698",
                            Name = "ACME DIY",
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
                        BankId = bankId,
                        UseStagingNotDefaultBankProfile = true,
                        UseStagingNotDefaultBankRegistration = true
                    });

            // Checks
            domesticPaymentConsentResp.Should().NotBeNull();
            domesticPaymentConsentResp.Messages.Should().BeEmpty();
            domesticPaymentConsentResp.Data.Should().NotBeNull();
        }
    }
}
