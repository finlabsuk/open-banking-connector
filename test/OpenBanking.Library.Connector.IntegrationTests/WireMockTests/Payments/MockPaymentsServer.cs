// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.WireMockTests.Payments
{
    public class MockPaymentsServer : IMockPaymentServer
    {
        private readonly IMockPaymentData _mockData;
        private readonly WireMockServer _server;

        public MockPaymentsServer(IMockPaymentData mockData)
        {
            _server = WireMockServer.Start(
                new WireMockServerSettings
                {
                    Urls = new[] { "http://+:8080" }
                });

            Console.WriteLine(
                "FluentMockServer running at {0}",
                string.Join(",", _server.Ports));

            _mockData = mockData;
        }

        public IEnumerable<ILogEntry> GetLogEntries()
        {
            IEnumerable<ILogEntry> logEntries = _server.LogEntries;
            return logEntries;
        }

        public void SetUpOpenIdMock()
        {
            _server
                .Given(
                    Request.Create()
                        .WithPath(MockRoutes.OpenId)
                        .UsingGet())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_mockData.GetOpenIdConfigJson()));
        }

        public void SetupRegistrationMock()
        {
            _server
                .Given(
                    Request.Create()
                        .WithPath(MockRoutes.Register)
                        .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(201)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_mockData.GetOpenBankingClientRegistrationResponseJson()));
        }

        public void SetupTokenEndpointMock()
        {
            _server
                .Given(
                    Request.Create()
                        .WithPath(MockRoutes.Token)
                        .WithHeader("x-fapi-financial-id", "*")
                        .WithBody(
                            b => b.Contains(
                                $"grant_type=client_credentials&scope=payments&client_id={_mockData.GetClientId()}"))
                        .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_mockData.GetOpenIdTokenEndpointResponseJson()));
        }

        public void SetupPaymentEndpointMock()
        {
            _server
                .Given(
                    Request.Create()
                        .WithPath(MockRoutes.DomesticPaymentConsents)
                        .WithHeader("x-fapi-financial-id", "*")
                        .WithHeader("Authorization", "*")
                        .WithHeader("x-idempotency-key", "*")
                        .WithHeader("x-jws-signature", "*")
                        .WithBody(b => b.Contains(_mockData.GetOBWriteDomesticConsent()))
                        .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_mockData.GetOBWriteDomesticConsentResponse2()));
        }

        public void SetUpAuthEndpoint()
        {
            _server
                .Given(
                    Request.Create()
                        .WithPath(MockRoutes.Token)
                        .WithHeader("response_type", "*")
                        .WithHeader("client_id", "*")
                        .WithHeader("redirect_uri", "*")
                        .WithHeader("scope", "*")
                        .WithHeader("request", "*")
                        .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_mockData.GetAuthtoriseResponse()));
        }

        public void SetUpOBDomesticResponseEndpoint()
        {
            _server
                .Given(
                    Request.Create()
                        .WithPath(MockRoutes.DomesticPayments)
                        .WithHeader("x-fapi-financial-id", "*")
                        .WithHeader("Authorization", "*")
                        .WithHeader("x-idempotency-key", "*")
                        .WithHeader("x-jws-signature", "*")
                        .UsingPost())
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "application/json")
                        .WithBody(_mockData.GetOBWriteDomesticResponse2()));
        }
    }
}
