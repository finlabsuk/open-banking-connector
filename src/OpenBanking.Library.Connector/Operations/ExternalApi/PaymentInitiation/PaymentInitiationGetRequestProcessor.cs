// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation
{
    internal class PaymentInitiationGetRequestProcessor : IGetRequestProcessor
    {
        private readonly string _orgId;
        private readonly TokenEndpointResponse _tokenEndpointResponse;


        public PaymentInitiationGetRequestProcessor(string orgId, TokenEndpointResponse tokenEndpointResponse)
        {
            _orgId = orgId;
            _tokenEndpointResponse = tokenEndpointResponse;
        }

        (List<HttpHeader> headers, string acceptType) IGetRequestProcessor.HttpGetRequestData(string requestDescription)
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
