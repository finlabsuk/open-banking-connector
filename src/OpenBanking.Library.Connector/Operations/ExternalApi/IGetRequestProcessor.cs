// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal interface IGetRequestProcessor
    {
        protected (List<HttpHeader> headers, string acceptType) HttpGetRequestData(string requestDescription);

        public async Task<TResponse> GetAsync<TResponse>(
            Uri uri,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient apiClient)
            where TResponse : class
        {
            // Process request
            var (headers, acceptType) = HttpGetRequestData($"POST {uri})");

            // POST request
            TResponse response = await new HttpRequestBuilder()
                .SetMethod(HttpMethod.Get)
                .SetUri(uri)
                .SetHeaders(headers)
                //.SetContentType(contentType)
                //.SetContent(content)
                .Create()
                .RequestJsonAsync<TResponse>(
                    apiClient,
                    jsonSerializerSettings);

            return response;
        }
    }
}
