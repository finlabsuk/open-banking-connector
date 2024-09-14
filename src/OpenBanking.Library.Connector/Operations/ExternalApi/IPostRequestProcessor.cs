// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
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
    public Task<(TResponse response, string? xFapiInteractionId)> PostAsync<TResponse>(
        Uri uri,
        IEnumerable<HttpHeader>? extraHeaders,
        TRequest request,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? requestJsonSerializerSettings,
        JsonSerializerSettings? responseJsonSerializerSettings,
        IApiClient apiClient)
        where TResponse : class;
}
