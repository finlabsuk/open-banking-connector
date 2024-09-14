// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.BankConfiguration;

public class BankRegistrationDeleteRequestProcessor : IDeleteRequestProcessor
{
    private readonly string _accessToken;

    public BankRegistrationDeleteRequestProcessor(string accessToken)
    {
        _accessToken = accessToken;
    }

    public async Task DeleteAsync(
        Uri uri,
        IEnumerable<HttpHeader>? extraHeaders,
        TppReportingRequestInfo? tppReportingRequestInfo,
        IApiClient apiClient)
    {
        // Assemble headers and body
        var headers = new List<HttpHeader> { new("Authorization", "Bearer " + _accessToken) };

        if (extraHeaders is not null)
        {
            foreach (HttpHeader header in extraHeaders)
            {
                headers.Add(header);
            }
        }

        // Send request
        await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Delete)
            .SetUri(uri)
            .SetHeaders(headers)
            .Create()
            .SendExpectingNoResponseAsync(tppReportingRequestInfo, apiClient);
    }
}
