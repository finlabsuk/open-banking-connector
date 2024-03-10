// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

/// <summary>
///     Class that captures logic related to request data processing and formatting,
///     correct header generation etc
///     to allow HTTP DELETE to external API to be executed.
///     HTTP DELETE is implemented by calling <see cref="DeleteAsync" /> with request data.
/// </summary>
internal interface IDeleteRequestProcessor
{
    protected List<HttpHeader> HttpDeleteRequestData(string requestDescription, IEnumerable<HttpHeader>? extraHeaders);

    public async Task DeleteAsync(
        Uri uri,
        IEnumerable<HttpHeader>? extraHeaders,
        TppReportingRequestInfo? tppReportingRequestInfo,
        IApiClient apiClient)
    {
        // Process request
        List<HttpHeader> headers =
            HttpDeleteRequestData($"DELETE {uri})", extraHeaders);

        // POST request
        await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Delete)
            .SetUri(uri)
            .SetHeaders(headers)
            .Create()
            .SendExpectingNoResponseAsync(tppReportingRequestInfo, apiClient);
    }
}
