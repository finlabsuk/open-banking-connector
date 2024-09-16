// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public interface IApiClient
{
    Task<(T response, string? xFapiInteractionId)> SendExpectingJsonResponseAsync<T>(
        HttpRequestMessage request,
        string? requestContentForLog,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings)
        where T : class;

    Task SendExpectingNoResponseAsync(
        HttpRequestMessage request,
        string? requestContentForLog,
        TppReportingRequestInfo? tppReportingRequestInfo);

    Task<string> SendExpectingStringResponseAsync(
        HttpRequestMessage request,
        string? requestContentForLog,
        TppReportingRequestInfo? tppReportingRequestInfo);
}
