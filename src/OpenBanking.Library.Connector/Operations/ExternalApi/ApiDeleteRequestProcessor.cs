// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Http;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    public class ApiDeleteRequestProcessor : IDeleteRequestProcessor
    {
        private readonly string _accessToken;
        private readonly string? _financialId;

        public ApiDeleteRequestProcessor(string accessToken, string? financialId)
        {
            _accessToken = accessToken;
            _financialId = financialId;
        }

        List<HttpHeader> IDeleteRequestProcessor.HttpDeleteRequestData(string requestDescription)
        {
            // Assemble headers and body
            var headers = new List<HttpHeader>
            {
                new HttpHeader("Authorization", "Bearer " + _accessToken),
            };

            if (!(_financialId is null))
            {
                headers.Add(new HttpHeader("x-fapi-financial-id", _financialId));
            }

            return headers;
        }
    }
}
