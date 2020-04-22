using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

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
            using var context = new SqliteDbContext(_dbContextOptions);
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
            return new List<string> { $"{MockRoutes.Url}/fragment-redirect", $"{MockRoutes.Url}/query-redirect" }.ToArray();
        }

        public string[] GetGrantTypes()
        {
            return new List<string> { "client_credentials", "authorization_code", "refresh_token" }.ToArray();
        }

        public string GetBase64TokenString()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{GetClientId()}:{GetClientSecret()}"));
        }

        public string GetOpenIdConfigJson()
        {
            var openIdConfig = new OpenIdConfiguration
            {
                Issuer = MockRoutes.Url,
                ResponseTypesSupported = new List<string> { "code id_token" }.ToArray(),
                ScopesSupported = new List<string> { "openid", "payments", "accounts", "fundsconfirmations", "profile" }.ToArray(),
                ResponseModesSupported = new List<string> { "fragment", "query", "form_post" }.ToArray(),
                TokenEndpoint = $"{MockRoutes.Url}{MockRoutes.Token}",
                AuthorizationEndpoint = $"{MockRoutes.Url}{MockRoutes.Authorize}",
                RegistrationEndpoint = $"{MockRoutes.Url}{MockRoutes.Register}"
            };

            return JsonConvert.SerializeObject(openIdConfig);
        }

        public string GetOpenBankingClientRegistrationResponseJson()
        {
            var model = new OpenBankingClientRegistrationResponse
            {
                ClientId = GetClientId(),
                ClientSecret = GetClientSecret(),
                ClientIdIssuedAt = DateTimeOffset.Now,
                ClientSecretExpiresAt = DateTimeOffset.Now.AddDays(30),
                TokenEndpointAuthMethod = "tls_client_auth",
                ResponseTypes = new List<string> { "code id_token" }.ToArray(),
                SoftwareId = GetClientId(),
                ApplicationType = "web",
                IdTokenSignedResponseAlg = "PS256",
                RequestObjectSigningAlg = "PS256",
                TokenEndpointAuthSigningAlg = string.Empty,
                GrantTypes = GetGrantTypes(),
                RedirectUris = GetRedirectUris(),
                Scope = new List<string> { "openid", "payments", "accounts", "fundsconfirmations" }.ToArray(),
                TlsClientAuthSubjectDn = $"CN={GetClientId()},OU=OrgId,O=OpenBanking,C=GB"
            };

            return JsonConvert.SerializeObject(model);
        }

        public string GetOpenIdTokenEndpointResponseJson()
        {
            var model = new TokenEndpointResponse
            {
                AccessToken = GetAccessToken(),
                RefreshToken = "RefreshToken",
                ExpiresIn = 300,
                TokenType = "payments"
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
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(MockRoutes.Url)
            };
            var _dB = new SqliteDbContext(_dbContextOptions);
            var requestBuilder = new RequestBuilder(
                new EntityMapper(),
                new DbMultiEntityMethods(_dB),
                new DefaultConfigurationProvider(),
                new ConsoleInstrumentationClient(),
                new MemoryKeySecretProvider(),
                new ApiClient(httpClient),
                new PemParsingCertificateReader(),
                new DbEntityRepository<BankClientProfile>(_dB),
                new DbEntityRepository<SoftwareStatementProfile>(_dB),
                new DbEntityRepository<DomesticConsent>(_dB),
                new DbEntityRepository<ApiProfile>(_dB));

            return requestBuilder;
        }

        public string GetOBWriteDomesticConsent2()
        {
            var instructedAmount = new OBInternational2InstructedAmount("50", "GBP");
            var creditorAccount = new OBCashAccountCreditor3("IBAN", "BE56456394728288", "ACME DIY", "secondary-identif");
            var domestic2 = new OBDomestic2("instr-identification", "e2e-identification", null, instructedAmount, null, creditorAccount);
            var data = new OBWriteDataDomesticConsent2(domestic2);

            var deliveryAddress = new OBRisk1DeliveryAddress("Oxford Street", new List<string>(), new List<string>(), "42", "London", "UK", "SW1 1AA");
            var risk = new OBRisk1(OBExternalPaymentContext1Code.EcommerceGoods, null, null, deliveryAddress);

            var model = new OBWriteDomesticConsent2(data, risk);

            return JsonConvert.SerializeObject(model);
        }

        public string GetOBWriteDomesticConsentResponse2()
        {
            var consentId = Guid.NewGuid().ToString();

            var instructedAmount = new OBInternational2InstructedAmount("50", "GBP");
            var creditorAccount = new OBCashAccountCreditor3("IBAN", "BE56456394728288", "ACME DIY", "secondary-identif");
            var domestic2 = new OBDomestic2("instr-identification", "e2e-identification", null, instructedAmount, null, creditorAccount);
            var data = new OBWriteDataDomesticConsentResponse2(consentId, DateTime.Now, OBExternalConsentStatus1Code.AwaitingAuthorisation, DateTime.Now, null, null, null, null, domestic2);

            var deliveryAddress = new OBRisk1DeliveryAddress("Oxford Street", new List<string>(), new List<string>(), "42", "London", "UK", "SW1 1AA");
            var risk = new OBRisk1(OBExternalPaymentContext1Code.EcommerceGoods, null, null, deliveryAddress);

            var links = new Links($"{MockRoutes.Url}/{MockRoutes.DomesticPayments}/{consentId}");

            var meta = new Meta(1);

            var model = new OBWriteDomesticConsentResponse2(data, risk, links, meta);

            return JsonConvert.SerializeObject(model);
        }
    }
}
