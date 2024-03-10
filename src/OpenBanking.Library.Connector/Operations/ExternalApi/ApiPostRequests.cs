// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal class
    ApiPostRequests<TApiRequest, TApiResponse, TVariantApiRequest, TVariantApiResponse> :
    IApiPostRequests<TApiRequest, TApiResponse>
    where TApiRequest : class
    where TApiResponse : class, ISupportsValidation
    where TVariantApiRequest : class
    where TVariantApiResponse : class
{
    private readonly IPostRequestProcessor<TVariantApiRequest> _postRequestProcessor;

    public ApiPostRequests(
        IPostRequestProcessor<TVariantApiRequest> postRequestProcessor)
    {
        _postRequestProcessor = postRequestProcessor;
    }

    /// <summary>
    ///     POST to bank API using request and response types for different API spec versions (variant types) as required.
    ///     Request type is mapped to variant type if necessary before request is sent.
    ///     Response type is mapped from variant type if necessary after response received.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="extraHeaders"></param>
    /// <param name="request"></param>
    /// <param name="tppReportingRequestInfo"></param>
    /// <param name="requestJsonSerializerSettings"></param>
    /// <param name="responseJsonSerializerSettings"></param>
    /// <param name="apiClient"></param>
    /// <param name="mapper"></param>
    /// <returns></returns>
    public async Task<(TApiResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        PostAsync(
            Uri uri,
            IEnumerable<HttpHeader>? extraHeaders,
            TApiRequest request,
            TppReportingRequestInfo? tppReportingRequestInfo,
            JsonSerializerSettings? requestJsonSerializerSettings,
            JsonSerializerSettings? responseJsonSerializerSettings,
            IApiClient apiClient,
            IApiVariantMapper mapper)
    {
        var nonErrorMessages = new List<IFluentResponseInfoOrWarningMessage>();

        // Map request type if necessary
        if (!(request is TVariantApiRequest variantRequest))
        {
            mapper.Map(request, out variantRequest);
        }

        // Process request
        var variantResponse = await _postRequestProcessor.PostAsync<TVariantApiResponse>(
            uri,
            extraHeaders,
            variantRequest,
            tppReportingRequestInfo,
            requestJsonSerializerSettings,
            responseJsonSerializerSettings,
            apiClient);

        // Map response type if necessary
        if (!(variantResponse is TApiResponse response))
        {
            mapper.Map(variantResponse, out response);
        }

        // Validate response
        ValidationResult responseValidationResult = await response.ValidateAsync();
        IEnumerable<IFluentResponseInfoOrWarningMessage> responseNonErrorMessages =
            responseValidationResult.ProcessValidationResultsAndRaiseErrors("prefix");
        nonErrorMessages.AddRange(responseNonErrorMessages);
        return (response, nonErrorMessages);
    }
}
