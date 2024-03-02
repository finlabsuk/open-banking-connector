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

internal class
    ApiRequests<TApiRequest, TApiResponse, TVariantApiRequest, TVariantApiResponse> :
    IApiRequests<TApiRequest, TApiResponse>
    where TApiRequest : class
    where TApiResponse : class, ISupportsValidation
    where TVariantApiRequest : class
    where TVariantApiResponse : class
{
    private readonly ApiGetRequests<TApiResponse, TVariantApiResponse> _apiGetRequests;

    private readonly ApiPostRequests<TApiRequest, TApiResponse, TVariantApiRequest, TVariantApiResponse>
        _apiPostRequests;

    public ApiRequests(
        IGetRequestProcessor getRequestProcessor,
        IPostRequestProcessor<TVariantApiRequest> postRequestProcessor)
    {
        _apiGetRequests = new ApiGetRequests<TApiResponse, TVariantApiResponse>(getRequestProcessor);
        _apiPostRequests =
            new ApiPostRequests<TApiRequest, TApiResponse, TVariantApiRequest, TVariantApiResponse>(
                postRequestProcessor);
    }

    public Task<(TApiResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> GetAsync(
        Uri uri,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings,
        IApiClient apiClient,
        IApiVariantMapper mapper) => _apiGetRequests.GetAsync(
        uri,
        tppReportingRequestInfo,
        jsonSerializerSettings,
        apiClient,
        mapper);

    public Task<(TApiResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> PostAsync(
        Uri uri,
        TApiRequest request,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? requestJsonSerializerSettings,
        JsonSerializerSettings? responseJsonSerializerSettings,
        IApiClient apiClient,
        IApiVariantMapper mapper) =>
        _apiPostRequests.PostAsync(
            uri,
            request,
            tppReportingRequestInfo,
            requestJsonSerializerSettings,
            responseJsonSerializerSettings,
            apiClient,
            mapper);
}
