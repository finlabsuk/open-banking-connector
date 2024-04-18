// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal interface IGetRequestProcessor
{
    protected (List<HttpHeader> headers, string acceptType) HttpGetRequestData(
        string requestDescription,
        IEnumerable<HttpHeader>? extraHeaders);

    public async Task<(TResponse response, string? xFapiInteractionId)> GetAsync<TResponse>(
        Uri uri,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient apiClient,
        IEnumerable<HttpHeader>? extraHeaders)
        where TResponse : class
    {
        // Process request
        (List<HttpHeader> headers, string acceptType) = HttpGetRequestData($"GET {uri})", extraHeaders);

        // POST request
        (TResponse response, string? xFapiInteractionId) = await new HttpRequestBuilder()
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

        return (response, xFapiInteractionId);
    }
}
