// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests.Payments
{
    public class MockPaymentsData : IMockPaymentData
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<SqliteDbContext> _dbContextOptions;
        private readonly IApiVariantMapper _mapper;

        public MockPaymentsData(IApiVariantMapper mapper)
        {
            _mapper = mapper;
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
            return new List<string> { $"{MockRoutes.Url}/fragment-redirect", $"{MockRoutes.Url}/query-redirect" }
                .ToArray();
        }

        public ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum[] GetGrantTypes()
        {
            return new List<ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum>
            {
                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum.ClientCredentials,
                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum.AuthorizationCode,
                ClientRegistrationModelsPublic.OBRegistrationProperties1grantTypesItemEnum.RefreshToken
            }.ToArray();
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
                ResponseTypesSupported = new List<string> { "code id_token" },
                ScopesSupported =
                    new List<string> { "openid", "payments", "accounts", "fundsconfirmations", "profile" },
                ResponseModesSupported = new List<OAuth2ResponseMode>
                {
                    OAuth2ResponseMode.Fragment
                },
                TokenEndpoint = $"{MockRoutes.Url}{MockRoutes.Token}",
                AuthorizationEndpoint = $"{MockRoutes.Url}{MockRoutes.Authorize}",
                RegistrationEndpoint = $"{MockRoutes.Url}{MockRoutes.Register}",
                TokenEndpointAuthMethodsSupported = new List<OpenIdConfigurationTokenEndpointAuthMethodEnum>
                    { OpenIdConfigurationTokenEndpointAuthMethodEnum.TlsClientAuth }
            };

            return JsonConvert.SerializeObject(openIdConfig);
        }

        public string GetOpenBankingClientRegistrationResponseJson()
        {
            var model =
                new ClientRegistrationModelsPublic.OBClientRegistration1
                {
                    ClientId = GetClientId(),
                    ClientSecret = GetClientSecret(),
                    ClientIdIssuedAt = DateTimeOffset.Now,
                    ClientSecretExpiresAt = DateTimeOffset.Now.AddDays(30),
                    TokenEndpointAuthMethod = ClientRegistrationModelsPublic
                        .OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth,
                    ResponseTypes =
                        new List<ClientRegistrationModelsPublic.OBRegistrationProperties1responseTypesItemEnum>
                        {
                            ClientRegistrationModelsPublic.OBRegistrationProperties1responseTypesItemEnum.CodeidToken
                        }.ToArray(),
                    SoftwareId = GetClientId(),
                    ApplicationType = ClientRegistrationModelsPublic.OBRegistrationProperties1applicationTypeEnum.Web,
                    IdTokenSignedResponseAlg = ClientRegistrationModelsPublic.SupportedAlgorithmsEnum.PS256,
                    RequestObjectSigningAlg = ClientRegistrationModelsPublic.SupportedAlgorithmsEnum.PS256,
                    TokenEndpointAuthSigningAlg = null,
                    GrantTypes = GetGrantTypes(),
                    RedirectUris = GetRedirectUris(),
                    Scope = string.Join(
                        " ",
                        new List<string> { "openid", "payments", "accounts", "fundsconfirmations" }),
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

        public string GetAuthtoriseResponse()
        {
            var model = new OAuth2RedirectData(
                "idToken123",
                "code123",
                "1ab89221-ca25-4055-9f96-7064fe953c52",
                "a71276e3-d7fe-4f0d-9ce5-10a7ac2f3dca");

            return JsonConvert.SerializeObject(model);
        }

        public string GetMockPrivateKey()
        {
            var fakePrivateKey = @"-----BEGIN PRIVATE KEY-----
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
            var fakeCertificate = @"-----BEGIN CERTIFICATE-----
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

        public IRequestBuilder CreateMockRequestBuilder(
            IProcessedSoftwareStatementProfileStore softwareStatementProfilesRepository)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(MockRoutes.Url)
            };
            var dB = new SqliteDbContext(_dbContextOptions);
            IApiVariantMapper apiVariantMapper = _mapper;
            var requestBuilder = new RequestBuilder(
                new TimeProvider(),
                apiVariantMapper,
                new ConsoleInstrumentationClient(),
                new ApiClient(new ConsoleInstrumentationClient(), httpClient),
                softwareStatementProfilesRepository,
                new DbService(dB),
                new BankProfileDefinitions(
                    new DefaultSettingsProvider<BankProfilesSettings>(new BankProfilesSettings())));

            return requestBuilder;
        }

        public string GetOBWriteDomesticConsent(BankProfile bankProfile)
        {
            // PaymentInitiationModelsPublic.OBWriteDomesticConsent4 requestModel =
            //     bankProfile.DomesticPaymentConsent(
            //         DomesticPaymentTypeEnum.PersonToMerchant,
            //         "placeholder: OBC consent ID",
            //         "placeholder: random GUID");
            //
            // return JsonConvert.SerializeObject(
            //     requestModel,
            //     new JsonSerializerSettings
            //     {
            //         NullValueHandling = NullValueHandling.Ignore
            //     });
            return "";
        }

        public string GetOBWriteDomesticConsentResponse2(BankProfile bankProfile)
        {
            // PaymentInitiationModelsPublic.OBWriteDomesticConsent4 requestModel =
            //     bankProfile.DomesticPaymentConsent(
            //         DomesticPaymentTypeEnum.PersonToMerchant,
            //         "placeholder: OBC consent ID",
            //         "placeholder: random GUID");
            //
            // string consentId = Guid.NewGuid().ToString();
            // _mapper.Map(
            //     requestModel.Data.Initiation,
            //     out PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5DataInitiation dataInitiation);
            // PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5Data data =
            //     new PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5Data(
            //         consentId,
            //         DateTime.Now,
            //         PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5DataStatusEnum
            //             .AwaitingAuthorisation,
            //         DateTime.Now,
            //         cutOffDateTime: DateTime.Now.Add(new TimeSpan(1, 0, 0, 0)),
            //         expectedExecutionDateTime: DateTime.Now,
            //         expectedSettlementDateTime: DateTime.Now,
            //         charges: null,
            //         initiation: dataInitiation);
            //
            // PaymentInitiationModelsPublic.OBRisk1 risk = requestModel.Risk;
            //
            // PaymentInitiationModelsPublic.Links links =
            //     new PaymentInitiationModelsPublic.Links($"{MockRoutes.Url}/{MockRoutes.DomesticPayments}/{consentId}");
            //
            // PaymentInitiationModelsPublic.Meta meta = new PaymentInitiationModelsPublic.Meta(1);
            //
            // PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5 model =
            //     new PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5(
            //         data,
            //         risk,
            //         links,
            //         meta);
            //
            // return JsonConvert.SerializeObject(
            //     model,
            //     new JsonSerializerSettings
            //     {
            //         NullValueHandling = NullValueHandling.Ignore
            //     });
            return "";
        }

        public string GetOBWriteDomesticResponse2()
        {
            var consentId = Guid.NewGuid().ToString();

            var instructedAmount =
                new PaymentInitiationModelsPublic.OBWriteDomesticResponse5DataInitiationInstructedAmount(
                    "50",
                    "GBP");
            var creditorAccount =
                new PaymentInitiationModelsPublic.OBWriteDomesticResponse5DataInitiationCreditorAccount(
                    "IBAN",
                    "BE56456394728288",
                    "ACME DIY",
                    "secondary-identif");
            var domestic2 =
                new PaymentInitiationModelsPublic.OBWriteDomesticResponse5DataInitiation(
                    "instr-identification",
                    "e2e-identification",
                    localInstrument: null,
                    instructedAmount: instructedAmount,
                    debtorAccount: null,
                    creditorAccount: creditorAccount);
            var dataDomesticReponse2 =
                new PaymentInitiationModelsPublic.OBWriteDomesticResponse5Data(
                    "PaymentId",
                    consentId,
                    DateTime.Now,
                    PaymentInitiationModelsPublic.OBWriteDomesticResponse5DataStatusEnum.Pending,
                    DateTimeOffset.Now,
                    expectedExecutionDateTime: DateTimeOffset.Now,
                    expectedSettlementDateTime: DateTimeOffset.Now,
                    refund: null,
                    charges: null,
                    initiation: domestic2);
            var links =
                new PaymentInitiationModelsPublic.Links($"{MockRoutes.Url}/{MockRoutes.DomesticPayments}");
            var meta = new PaymentInitiationModelsPublic.Meta(1);

            var model =
                new PaymentInitiationModelsPublic.OBWriteDomesticResponse5(
                    dataDomesticReponse2,
                    links,
                    meta);

            return JsonConvert.SerializeObject(model);
        }
    }
}
