// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation
{
    internal class PaymentInitiationGetRequestProcessor : IGetRequestProcessor
    {
        private readonly string _accessToken;
        private readonly string _orgId;


        public PaymentInitiationGetRequestProcessor(string orgId, string accessToken)
        {
            _orgId = orgId;
            _accessToken = accessToken;
        }

        (List<HttpHeader> headers, string acceptType) IGetRequestProcessor.HttpGetRequestData(string requestDescription)
        {
            // Assemble headers and body
            var headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", _orgId),
                new HttpHeader("Authorization", "Bearer " + _accessToken),
            };

            return (headers, "application/json");
        }
    }
}
