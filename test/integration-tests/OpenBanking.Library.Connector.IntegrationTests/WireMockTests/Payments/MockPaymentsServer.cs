using System;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests.Payments
{
    public class MockPaymentsServer : IMockPaymentServer
    {
        private readonly WireMockServer _server;
        private readonly IMockPaymentData _mockData;

        public MockPaymentsServer(IMockPaymentData mockData)
        {
            _server = WireMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:8080" }
            });

            Console.WriteLine("FluentMockServer running at {0}", string.Join(",", _server.Ports));

            this._mockData = mockData;
        }

        public void SetUpOpenIdMock()
        {
            _server
                .Given(Request.Create()
                    .WithPath(MockRoutes.OpenId)
                    .UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(_mockData.GetOpenIdConfigJson())
                );
        }

        public void SetupRegistrationMock()
        {
            _server
                .Given(Request.Create()
                    .WithPath(MockRoutes.Register)
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(201)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(_mockData.GetOpenBankingClientRegistrationResponseJson())
                );
        }

        public void SetupTokenEndpointMock()
        {
            _server
                .Given(Request.Create()
                    .WithPath(MockRoutes.Token)
                    .WithHeader("x-fapi-financial-id", "*")
                    .WithBody(b => b.Contains($"grant_type=client_credentials&scope=payments&client_id={_mockData.GetClientId()}"))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(_mockData.GetOpenIdTokenEndpointResponseJson())
                );
        }

        public void SetupPaymentEndpointMock() 
        {
            _server
                .Given(Request.Create()
                    .WithPath(MockRoutes.DomesticPaymentConsents)
                    .WithHeader("x-fapi-financial-id", "*")
                    .WithHeader("Authorization", "*")
                    .WithHeader("x-idempotency-key", "*")
                    .WithHeader("x-jws-signature", "*")
                    .WithBody(b => b.Contains(_mockData.GetOBWriteDomesticConsent2()))
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(_mockData.GetOBWriteDomesticConsentResponse2())
                );
        }

        public void SetUpAuthEndpoint()
        {
            _server
                .Given(Request.Create()
                    .WithPath(MockRoutes.Token)
                    .WithHeader("response_type", "*")
                    .WithHeader("client_id", "*")
                    .WithHeader("redirect_uri", "*")
                    .WithHeader("scope", "*")
                    .WithHeader("request", "*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(_mockData.GetAuthtoriseResponse())
                );
        }

        public void SetUpOBDomesticResponseEndpoint()
        {
            _server
                .Given(Request.Create()
                    .WithPath(MockRoutes.DomesticPayments)
                    .WithHeader("x-fapi-financial-id", "*")
                    .WithHeader("Authorization", "*")
                    .WithHeader("x-idempotency-key", "*")
                    .WithHeader("x-jws-signature", "*")
                    .UsingPost())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(_mockData.GetOBWriteDomesticResponse2())
                );
        }
    }
}
