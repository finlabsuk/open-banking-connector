// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction
{
    internal class
        AccountAndTransactionPostRequestProcessor<TVariantApiRequest> : IPostRequestProcessor<TVariantApiRequest>
        where TVariantApiRequest : class
    {
        private readonly IInstrumentationClient _instrumentationClient;
        private readonly string _orgId;
        private readonly TokenEndpointResponse _tokenEndpointResponse;

        public AccountAndTransactionPostRequestProcessor(
            string orgId,
            TokenEndpointResponse tokenEndpointResponse,
            IInstrumentationClient instrumentationClient)
        {
            _instrumentationClient = instrumentationClient;
            _orgId = orgId;
            _tokenEndpointResponse = tokenEndpointResponse;
        }

        (List<HttpHeader> headers, string body, string contentType) IPostRequestProcessor<TVariantApiRequest>.
            HttpPostRequestData(
                TVariantApiRequest variantRequest,
                JsonSerializerSettings? requestJsonSerializerSettings,
                string requestDescription)
        {
            // Assemble headers and body
            var headers = new List<HttpHeader>
            {
                new HttpHeader("x-fapi-financial-id", _orgId),
                new HttpHeader("Authorization", "Bearer " + _tokenEndpointResponse.AccessToken),
                new HttpHeader("x-idempotency-key", Guid.NewGuid().ToString()),
            };
            JsonSerializerSettings jsonSerializerSettings =
                requestJsonSerializerSettings ?? new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            string content = JsonConvert.SerializeObject(
                variantRequest,
                jsonSerializerSettings);
            return (headers, content, "application/json");
        }
    }
}
