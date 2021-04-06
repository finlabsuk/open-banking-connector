// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation
{
    internal class PaymentInitiationGetRequestProcessor : GetRequestProcessor

    {
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly string _orgId;
        private readonly PaymentInitiationApi _paymentInitiationApi;
        private readonly SoftwareStatementProfile _softwareStatementProfile;
        private readonly TokenEndpointResponse _tokenEndpointResponse;

        public PaymentInitiationGetRequestProcessor(
            string orgId,
            TokenEndpointResponse tokenEndpointResponse,
            SoftwareStatementProfile softwareStatementProfile,
            PaymentInitiationApi paymentInitiationApi,
            IInstrumentationClient instrumentationClient)
        {
            _orgId = orgId;
            _tokenEndpointResponse = tokenEndpointResponse;
            _softwareStatementProfile = softwareStatementProfile;
            _paymentInitiationApi = paymentInitiationApi;
            _instrumentationClient = instrumentationClient;
        }

        protected override (List<HttpHeader> headers, string acceptType) HttpGetRequestData(string requestDescription)
        {
            // Assemble headers and body
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", _orgId),
                new HttpHeader("Authorization", "Bearer " + _tokenEndpointResponse.AccessToken),
                new HttpHeader("x-idempotency-key", Guid.NewGuid().ToString()),
            };

            return (headers, "application/json");
        }
    }
}
