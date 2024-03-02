// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal interface IGetRequestProcessor
{
    protected (List<HttpHeader> headers, string acceptType) HttpGetRequestData(string requestDescription);

    public async Task<TResponse> GetAsync<TResponse>(
        Uri uri,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient apiClient)
        where TResponse : class
    {
        // Process request
        (List<HttpHeader> headers, string acceptType) = HttpGetRequestData($"GET {uri})");

        // POST request
        var response = await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(uri)
            .SetHeaders(headers)
            //.SetContentType(contentType)
            //.SetContent(content)
            .Create()
            .SendExpectingJsonResponseAsync<TResponse>(
                apiClient,
                tppReportingRequestInfo,
                jsonSerializerSettings);

        return response;
    }
}
