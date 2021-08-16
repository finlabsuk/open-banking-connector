// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    public class ApiDeleteRequestProcessor : IDeleteRequestProcessor
    {
        private readonly TokenEndpointResponse _tokenEndpointResponse;

        public ApiDeleteRequestProcessor(TokenEndpointResponse tokenEndpointResponse)
        {
            _tokenEndpointResponse = tokenEndpointResponse;
        }

        List<HttpHeader> IDeleteRequestProcessor.HttpDeleteRequestData(string requestDescription)
        {
            // Assemble headers and body
            List<HttpHeader> headers = new List<HttpHeader>
            {
                new HttpHeader("Authorization", "Bearer " + _tokenEndpointResponse.AccessToken),
            };

            return headers;
        }
    }
}
