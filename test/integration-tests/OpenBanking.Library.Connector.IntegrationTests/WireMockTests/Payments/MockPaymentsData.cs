﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.ClientRegistration.V3p2.Models;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SoftwareStatementProfile =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request.SoftwareStatementProfile;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests.Payments
{
    public class MockPaymentsData : IMockPaymentData
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<SqliteDbContext> _dbContextOptions;

        public MockPaymentsData()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open(); // Creates DB
            _dbContextOptions = new DbContextOptionsBuilder<SqliteDbContext>()
                .UseSqlite(_connection)
                .Options;
            using SqliteDbContext context = new SqliteDbContext(_dbContextOptions);
            context.Database.EnsureCreated();
        }

        public string GetClientId()
        {
            return "ABC123XYZ";
        }

        public string GetClientSecret()
        {
            return "123ABC789";
        }

        public string GetFapiHeader()
        {
            return "A1B1C1";
        }

        public string GetAccessToken()
        {
            return "AccessToken";
        }

        public string GetJwsSignature()
        {
            return "JwsSig";
        }

        public string GetIdempotencyKey()
        {
            return "e45e6a28-561c-4b28-a4a1-d9814ce089d2";
        }

        public string GetPaymentConsentId()
        {
            return "e45e6a28-561c-4b28-a4a1-d9814ce089d2";
        }

        public string[] GetRedirectUris()
        {
            return new List<string> { $"{MockRoutes.Url}/fragment-redirect", $"{MockRoutes.Url}/query-redirect" }
                .ToArray();
        }

        public GrantTypesItemEnum[] GetGrantTypes()
        {
            return new List<GrantTypesItemEnum> { GrantTypesItemEnum.ClientCredentials, GrantTypesItemEnum.AuthorizationCode, GrantTypesItemEnum.RefreshToken }.ToArray();
        }

        public string GetBase64TokenString()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{GetClientId()}:{GetClientSecret()}"));
        }

        public string GetOpenIdConfigJson()
        {
            OpenIdConfiguration openIdConfig = new OpenIdConfiguration
            {
                Issuer = MockRoutes.Url,
                ResponseTypesSupported = new List<string> { "code id_token" }.ToArray(),
                ScopesSupported = new List<string> { "openid", "payments", "accounts", "fundsconfirmations", "profile" }
                    .ToArray(),
                ResponseModesSupported = new List<string> { "fragment", "query", "form_post" }.ToArray(),
                TokenEndpoint = $"{MockRoutes.Url}{MockRoutes.Token}",
                AuthorizationEndpoint = $"{MockRoutes.Url}{MockRoutes.Authorize}",
                RegistrationEndpoint = $"{MockRoutes.Url}{MockRoutes.Register}"
            };

            return JsonConvert.SerializeObject(openIdConfig);
        }

        public string GetOpenBankingClientRegistrationResponseJson()
        {
            OBClientRegistration1 model = new OBClientRegistration1
            {
                ClientId = GetClientId(),
                ClientSecret = GetClientSecret(),
                ClientIdIssuedAt = DateTimeOffset.Now,
                ClientSecretExpiresAt = DateTimeOffset.Now.AddDays(30),
                TokenEndpointAuthMethod = TokenEndpointAuthMethodEnum.TlsClientAuth,
                ResponseTypes = new List<ResponseTypesItemEnum> { ResponseTypesItemEnum.CodeidToken }.ToArray(),
                SoftwareId = GetClientId(),
                ApplicationType = ApplicationTypeEnum.Web,
                IdTokenSignedResponseAlg = SupportedAlgorithmsEnum.PS256,
                RequestObjectSigningAlg = SupportedAlgorithmsEnum.PS256,
                TokenEndpointAuthSigningAlg = null,
                GrantTypes = GetGrantTypes(),
                RedirectUris = GetRedirectUris(),
                Scope = string.Join(" ", new List<string> { "openid", "payments", "accounts", "fundsconfirmations" }),
                TlsClientAuthSubjectDn = $"CN={GetClientId()},OU=OrgId,O=OpenBanking,C=GB"
            };

            return JsonConvert.SerializeObject(model);
        }

        public string GetOpenIdTokenEndpointResponseJson()
        {
            TokenEndpointResponse? model = new TokenEndpointResponse
            {
                AccessToken = GetAccessToken(),
                RefreshToken = "RefreshToken",
                ExpiresIn = 300,
                TokenType = "payments"
            };

            return JsonConvert.SerializeObject(model);
        }

        public string GetAuthtoriseResponse()
        {
            var model = new AuthorisationCallbackPayload()
            {
                Code = "code123",
                State = "1ab89221-ca25-4055-9f96-7064fe953c52",
                Nonce = "a71276e3-d7fe-4f0d-9ce5-10a7ac2f3dca"
            };

            return JsonConvert.SerializeObject(model); 
        }

        public string GetMockPrivateKey()
        {
            string fakePrivateKey = @"-----BEGIN PRIVATE KEY-----
                    MIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCotsqtXUIESl1d
                    0OjahnUT0evKM/GxBXiL/HU1Zf1gUQ5t17gHo2xDvMZZI28655awWNrgKqbdXUct
                    Zi2p+9pWjx5Ud4By4vzfeekNCpfc1N/U6BmNCME82rqwEH8dcUMh2PViUPX0Zado
                    I7toWsHY+PoUmx7pHZS/1yeEDyBhMCq/hSAcUsSyXECNAyaUCt1fVP9l78VJsahg
                    VQkojCu3Jf8YALU3CIw0G604WXoTWinCUfI95Kp87rXSl/qXJr5zG+oNanswuAKO
                    c4Sc3Z8yta+0GSCgnL2bA2fEpCGwJS7B9FqX2TmSx9wP2xJchYSh+0/JBMS82JKV
                    X2PSBa/jAgMBAAECggEAay9WLDXnTxp/nq6ugMaTcvgWuOuvTSuQnj7RqA8Xni1g
                    4V//RrsIeTBhQxhD/kRLc9W/mFMAl/o+0hTsReX/nKZoOnmBXXvf84pcCTEFsgJ8
                    AaNDOFqriaHcoSaZ23atk79mhgOxfodmh77LvUyLgvEK0DSMusVUT1y9eiSyCYus
                    S3wqM3wMTl7ydaRfpDaKhx7ztXU1FvXxX+MZuNWpS7OxHxMp9ihfOwboDcPq8tD+
                    AW8oI5+nbjceUXVm8DziP84Sv0njml35I2DKVGEnsNWbDtdC2qJz2Z0hrgZRFiC6
                    SC9jY+yK/X/RuILj7KqOXfGsJuSZSLmgyO5AZrodwQKBgQDQUbLCKm1FKTXlujtx
                    9S+3Z5tNYefoWwDEEFp6AmEdGb5w8ZAKZgxEsYQGShCV9ZvbYH+ab6EdNTsSnxYg
                    u2mnkrjHFDfgsX64n+aSlGqyEDQznUJQBCLmNaAe2EVvNwkqMdIcAtYOea77w430
                    na6AIaevdlgX51GhNkTyLFPzGwKBgQDPVHZf6iKwiXMVURpCkvazkf68+3EV5Cuw
                    KWM/EjOaTQyszU8wNOWeoEn3B5ecxsds8ArNJtQY46lte/BGrVrxkCJ81Pd2JTMe
                    ZFguO2+IKezc/LsDXJ8ip1GOje23opaqhLY60ssNIT7BgROQNYpW0I1o1c4+t9lG
                    TcU3mWG62QKBgAY+LIk9FEE6Vu8ngZiMrH1mkXTkNjr0XzWA9/UoFRj2KIrh8lsd
                    a8izOS2nEpAr6BvO7IQITF+/tKF+Ov+IHkZzUy5EINiO+Sk3QYWrBPeJHMdfMxmK
                    xjD38nB6Mrp+f7vP+mf0fc25P9EzrxFhmFUmrQvJFCLDP+MA6VgIlAt5AoGAcYVh
                    u2rLy7W22bkqk8E8a15JCNj0NnRUp4RB30rsDlSZ2BQig9ay04QYe7YggasljrRw
                    dZwJDge5wUTXbxuEbDfrakaBqKL9tmpjDg1lKD0sgGOxUknoqGTM9y0nOCFCNeOs
                    k9DPdUT6w6EVQSTPtnFCvGA1KYoo4aG7lpHZPSECgYEAxwkHhDOSE2GXqfB1c+D9
                    9DXxPiaHDX+FbT//J4xUpKMlhVq9CpqUxseC5RZjMra31BKXIRCGnQ4dyDHR9VgR
                    BM+ItEfWUr/XsLNIc9p8ZqR8wpKbwORfpq4TYUybMMVlWfpH/CdvGWrPRe+YVlX9
                    mMO7k6ZO+5gl9DP86o2jSEU=
                    -----END PRIVATE KEY-----
                    ";

            return fakePrivateKey;
        }

        public string GetMockCertificate()
        {
            string fakeCertificate = @"-----BEGIN CERTIFICATE-----
                    MIIDlzCCAn8CFFeWF2pKRIFp+jN4U01mGrtpGHzyMA0GCSqGSIb3DQEBCwUAMIGH
                    MQswCQYDVQQGEwJHQjENMAsGA1UECAwES2VudDESMBAGA1UEBwwJTWFpZHN0b25l
                    MREwDwYDVQQKDAhNb2NrQmFuazELMAkGA1UECwwCSVQxEjAQBgNVBAMMCWxvY2Fs
                    aG9zdDEhMB8GCSqGSIb3DQEJARYSdGVzdEBsb2NhbGhvc3QuY29tMB4XDTIwMDMx
                    NDExMTU0N1oXDTIxMDMxNDExMTU0N1owgYcxCzAJBgNVBAYTAkdCMQ0wCwYDVQQI
                    DARLZW50MRIwEAYDVQQHDAlNYWlkc3RvbmUxETAPBgNVBAoMCE1vY2tCYW5rMQsw
                    CQYDVQQLDAJJVDESMBAGA1UEAwwJbG9jYWxob3N0MSEwHwYJKoZIhvcNAQkBFhJ0
                    ZXN0QGxvY2FsaG9zdC5jb20wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
                    AQCotsqtXUIESl1d0OjahnUT0evKM/GxBXiL/HU1Zf1gUQ5t17gHo2xDvMZZI286
                    55awWNrgKqbdXUctZi2p+9pWjx5Ud4By4vzfeekNCpfc1N/U6BmNCME82rqwEH8d
                    cUMh2PViUPX0ZadoI7toWsHY+PoUmx7pHZS/1yeEDyBhMCq/hSAcUsSyXECNAyaU
                    Ct1fVP9l78VJsahgVQkojCu3Jf8YALU3CIw0G604WXoTWinCUfI95Kp87rXSl/qX
                    Jr5zG+oNanswuAKOc4Sc3Z8yta+0GSCgnL2bA2fEpCGwJS7B9FqX2TmSx9wP2xJc
                    hYSh+0/JBMS82JKVX2PSBa/jAgMBAAEwDQYJKoZIhvcNAQELBQADggEBADKs3mYQ
                    kxHZ4KQ4lyewxMvq3SX02MDleJBkx2HljikZJZJhKRchnJjmTN3xaa7X/LsYVOg9
                    j1OEE2ZYBxK578z/+3YrexfFvbgMQB9hnnx1oXtEFiy6dylmVl5jafaw5MnoqXyL
                    jS8n21Mc0+hnlFuiaH0ARVNKB9/aErUw9ChzZXkCZDXJmhIdRLp++0BotR/5wR1p
                    yzoH75jpLRNcpVHhlaouZH/mAJdyIaeiQZ3Iw6KYkUb7MWFrUZrzt73AC/xOZ6aW
                    PABJk0N8hR8hAGCOLgVWybY4DKZfmEUdWvLpO8TchWp5h/4iUuC0U75Nef+dvnk7
                    pMsJHkbmD1bJMS0=
                    -----END CERTIFICATE-----
                    ";

            return fakeCertificate;
        }

        public IOpenBankingRequestBuilder CreateMockRequestBuilder()
        {
            HttpClient httpClient = new HttpClient
            {
                BaseAddress = new Uri(MockRoutes.Url)
            };
            SqliteDbContext _dB = new SqliteDbContext(_dbContextOptions);
            MemoryKeySecretProvider _secretProvider = new MemoryKeySecretProvider();
            EntityMapper _entityMapper = new EntityMapper();
            RequestBuilder requestBuilder = new RequestBuilder(
                entityMapper: _entityMapper,
                dbContextService: new DbMultiEntityMethods(_dB),
                configurationProvider: new DefaultConfigurationProvider(),
                logger: new ConsoleInstrumentationClient(),
                keySecretReadOnlyProvider: _secretProvider,
                apiClient: new ApiClient(httpClient),
                certificateReader: new PemParsingCertificateReader(),
                clientProfileRepository: new DbEntityRepository<BankRegistration>(_dB),
                domesticConsentRepo: new DbEntityRepository<DomesticPaymentConsent>(_dB),
                apiProfileRepository: new DbEntityRepository<BankProfile>(_dB),
                activeSReadOnlyRepo: new KeySecretReadRepository<ActiveSoftwareStatementProfiles>(_secretProvider),
                activeSrRepo: new KeySecretWriteRepository<ActiveSoftwareStatementProfiles>(_secretProvider),
                sReadOnlyRepo: new KeySecretMultiItemReadRepository<SoftwareStatementProfile>(_secretProvider),
                sRepo: new KeySecretMultiItemWriteRepository<SoftwareStatementProfile>(_secretProvider),
                softwareStatementProfileService: new SoftwareStatementProfileService(
                    softwareStatementProfileRepo:
                    new KeySecretMultiItemReadRepository<SoftwareStatementProfile>(_secretProvider),
                    activeSoftwareStatementProfilesRepo: new KeySecretReadRepository<ActiveSoftwareStatementProfiles>(
                        _secretProvider),
                    mapper: _entityMapper),
                new DbEntityRepository<Bank>(_dB));

            return requestBuilder;
        }

        public string GetOBWriteDomesticConsent2()
        {
            OBWriteDomestic2DataInitiationInstructedAmount instructedAmount =
                new OBWriteDomestic2DataInitiationInstructedAmount(amount: "50", currency: "GBP");
            OBWriteDomestic2DataInitiationCreditorAccount creditorAccount =
                new OBWriteDomestic2DataInitiationCreditorAccount(
                    schemeName: "IBAN",
                    identification: "BE56456394728288",
                    name: "ACME DIY",
                    secondaryIdentification: "secondary-identif");
            OBWriteDomestic2DataInitiationRemittanceInformation remittanceInformation =
                new OBWriteDomestic2DataInitiationRemittanceInformation(unstructured: "Tools", reference: "Tools");
            OBWriteDomestic2DataInitiation domestic2 = new OBWriteDomestic2DataInitiation(
                instructionIdentification: "instr-identification",
                endToEndIdentification: "e2e-identification",
                localInstrument: null,
                instructedAmount: instructedAmount,
                debtorAccount: null,
                creditorAccount: creditorAccount,
                remittanceInformation: remittanceInformation);
            OBWriteDomesticConsent4Data data = new OBWriteDomesticConsent4Data
            {
                Initiation = domestic2
            };

            OBRisk1DeliveryAddress deliveryAddress = new OBRisk1DeliveryAddress
            {
                StreetName = "Oxford Street",
                BuildingNumber = "42",
                PostCode = "SW1 1AA",
                TownName = "London",
                Country = "UK"
            };
            OBRisk1 risk = new OBRisk1(
                paymentContextCode: OBRisk1.PaymentContextCodeEnum.EcommerceGoods,
                merchantCategoryCode: null,
                merchantCustomerIdentification: null,
                deliveryAddress: deliveryAddress);

            OBWriteDomesticConsent4 model = new OBWriteDomesticConsent4(data: data, risk: risk);

            return JsonConvert.SerializeObject(model);
        }

        public string GetOBWriteDomesticConsentResponse2()
        {
            string consentId = Guid.NewGuid().ToString();

            OBWriteDomestic2DataInitiationInstructedAmount instructedAmount =
                new OBWriteDomestic2DataInitiationInstructedAmount(amount: "50", currency: "GBP");
            OBWriteDomestic2DataInitiationCreditorAccount creditorAccount =
                new OBWriteDomestic2DataInitiationCreditorAccount(
                    schemeName: "IBAN",
                    identification: "BE56456394728288",
                    name: "ACME DIY",
                    secondaryIdentification: "secondary-identif");
            OBWriteDomestic2DataInitiation domestic2 = new OBWriteDomestic2DataInitiation(
                instructionIdentification: "instr-identification",
                endToEndIdentification: "e2e-identification",
                localInstrument: null,
                instructedAmount: instructedAmount,
                debtorAccount: null,
                creditorAccount: creditorAccount);
            OBWriteDomesticConsentResponse4Data data = new OBWriteDomesticConsentResponse4Data(
                consentId: consentId,
                creationDateTime: DateTime.Now,
                status: OBWriteDomesticConsentResponse4Data.StatusEnum.AwaitingAuthorisation,
                statusUpdateDateTime: DateTime.Now,
                cutOffDateTime: DateTime.Now.Add(new TimeSpan(days: 1, hours: 0, minutes: 0, seconds: 0)),
                expectedExecutionDateTime: DateTime.Now,
                expectedSettlementDateTime: DateTime.Now,
                charges: null,
                initiation: domestic2);
            OBRisk1DeliveryAddress deliveryAddress = new OBRisk1DeliveryAddress
            {
                StreetName = "Oxford Street",
                BuildingNumber = "42",
                PostCode = "SW1 1AA",
                TownName = "London",
                Country = "UK"
            };
            OBRisk1 risk = new OBRisk1(
                paymentContextCode: OBRisk1.PaymentContextCodeEnum.EcommerceGoods,
                merchantCategoryCode: null,
                merchantCustomerIdentification: null,
                deliveryAddress: deliveryAddress);

            Links links = new Links($"{MockRoutes.Url}/{MockRoutes.DomesticPayments}/{consentId}");

            Meta meta = new Meta(1);

            OBWriteDomesticConsentResponse4 model = new OBWriteDomesticConsentResponse4(
                data: data,
                risk: risk,
                links: links,
                meta: meta);

            return JsonConvert.SerializeObject(model);
        }

        public string GetOBWriteDomesticResponse2()
        {
            var consentId = Guid.NewGuid().ToString();

            var instructedAmount = new OBWriteDomestic2DataInitiationInstructedAmount("50", "GBP");
            var creditorAccount = new OBWriteDomestic2DataInitiationCreditorAccount("IBAN", "BE56456394728288", "ACME DIY", "secondary-identif");
            var domestic2 = new OBWriteDomestic2DataInitiation("instr-identification", "e2e-identification", null, instructedAmount, null, creditorAccount);
            var dataDomesticReponse2 = new OBWriteDomesticResponse4Data("PaymentId", consentId, DateTime.Now, OBWriteDomesticResponse4Data.StatusEnum.Pending, DateTimeOffset.Now, DateTimeOffset.Now, DateTimeOffset.Now, null, null, domestic2);
            var links = new Links($"{MockRoutes.Url}/{MockRoutes.DomesticPayments}");
            var meta = new Meta(1);

            var model = new OBWriteDomesticResponse4(dataDomesticReponse2, links, meta);

            return JsonConvert.SerializeObject(model);
        }
    }
}
