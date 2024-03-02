// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

internal static class HttpRequestMessageExtensions
{
    public static Task<T> SendExpectingJsonResponseAsync<T>(
        this HttpRequestMessage request,
        IApiClient client,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings = null)
        where T : class
    {
        request.ArgNotNull(nameof(request));
        client.ArgNotNull(nameof(client));

        return client.SendExpectingJsonResponseAsync<T>(
            request,
            tppReportingRequestInfo,
            jsonSerializerSettings);
    }

    public static Task<string> SendExpectingStringResponseAsync(
        this HttpRequestMessage request,
        TppReportingRequestInfo? tppReportingRequestInfo,
        IApiClient client)

    {
        request.ArgNotNull(nameof(request));
        client.ArgNotNull(nameof(client));

        return client.SendExpectingStringResponseAsync(request, tppReportingRequestInfo);
    }

    public static Task SendExpectingNoResponseAsync(
        this HttpRequestMessage request,
        TppReportingRequestInfo? tppReportingRequestInfo,
        IApiClient client)
    {
        request.ArgNotNull(nameof(request));
        return client.SendExpectingNoResponseAsync(request, tppReportingRequestInfo);
    }
}
