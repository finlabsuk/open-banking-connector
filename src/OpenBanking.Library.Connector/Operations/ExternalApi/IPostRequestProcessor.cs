// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

/// <summary>
///     Class that captures logic related to request data processing and formatting,
///     correct header generation etc
///     to allow HTTP POST to external API to be executed.
///     HTTP POST is implemented by calling <see cref="PostAsync{TResponse}" /> with request data.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
internal interface IPostRequestProcessor<in TRequest>
    where TRequest : class
{
    protected (List<HttpHeader> headers, string body, string contentType) HttpPostRequestData(
        TRequest variantRequest,
        JsonSerializerSettings? requestJsonSerializerSettings,
        string requestDescription);

    public async Task<TResponse> PostAsync<TResponse>(
        Uri uri,
        TRequest request,
        JsonSerializerSettings? requestJsonSerializerSettings,
        JsonSerializerSettings? responseJsonSerializerSettings,
        IApiClient apiClient)
        where TResponse : class
    {
        // Process request
        (List<HttpHeader> headers, string content, string contentType) =
            HttpPostRequestData(request, requestJsonSerializerSettings, $"POST {uri})");

        // POST request
        var response = await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Post)
            .SetUri(uri)
            .SetHeaders(headers)
            .SetContentType(contentType)
            .SetContent(content)
            .Create()
            .SendExpectingJsonResponseAsync<TResponse>(
                apiClient,
                responseJsonSerializerSettings);

        return response;
    }
}
