// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal interface IApiGetRequests<TApiResponse>
    where TApiResponse : class, ISupportsValidation
{
    Task<(TApiResponse response, string? xFapiInteractionId, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
        )> GetAsync(
        Uri uri,
        IEnumerable<HttpHeader>? extraHeaders,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient apiClient,
        IApiVariantMapper mapper);
}
