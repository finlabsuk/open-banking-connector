// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.AccountAndTransaction;

internal class
    AccountAndTransactionPostRequestProcessor<TVariantApiRequest> : IPostRequestProcessor<TVariantApiRequest>
    where TVariantApiRequest : class
{
    private readonly string _accessToken;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly string _orgId;

    public AccountAndTransactionPostRequestProcessor(
        string orgId,
        string accessToken,
        IInstrumentationClient instrumentationClient)
    {
        _instrumentationClient = instrumentationClient;
        _orgId = orgId;
        _accessToken = accessToken;
    }

    public async Task<(TResponse response, string? xFapiInteractionId)> PostAsync<TResponse>(
        Uri uri,
        IEnumerable<HttpHeader>? extraHeaders,
        TVariantApiRequest request,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? requestJsonSerializerSettings,
        JsonSerializerSettings? responseJsonSerializerSettings,
        IApiClient apiClient)
        where TResponse : class
    {
        // Assemble headers and body
        var headers = new List<HttpHeader>
        {
            new("x-fapi-financial-id", _orgId),
            new("Authorization", "Bearer " + _accessToken),
            new("x-idempotency-key", Guid.NewGuid().ToString())
        };
        if (extraHeaders is not null)
        {
            foreach (HttpHeader header in extraHeaders)
            {
                headers.Add(header);
            }
        }

        // Send request
        (TResponse response, string? xFapiInteractionId) = await new HttpRequestBuilder()
            .SetMethod(HttpMethod.Post)
            .SetUri(uri)
            .SetHeaders(headers)
            .SetJsonContent(request, requestJsonSerializerSettings)
            .Create()
            .SendExpectingJsonResponseAsync<TResponse>(
                apiClient,
                tppReportingRequestInfo,
                responseJsonSerializerSettings);

        return (response, xFapiInteractionId);
    }
}
