// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi;

internal class
    ApiGetRequests<TApiResponse, TVariantApiResponse> :
        IApiGetRequests<TApiResponse>
    where TApiResponse : class, ISupportsValidation
    where TVariantApiResponse : class
{
    private readonly IGetRequestProcessor _getRequestProcessor;

    public ApiGetRequests(IGetRequestProcessor getRequestProcessor)
    {
        _getRequestProcessor = getRequestProcessor;
    }

    public async Task<(TApiResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        GetAsync(
            Uri uri,
            JsonSerializerSettings? jsonSerializerSettings,
            IApiClient apiClient,
            IApiVariantMapper mapper)
    {
        var nonErrorMessages = new List<IFluentResponseInfoOrWarningMessage>();

        // Process request
        var variantResponse = await _getRequestProcessor.GetAsync<TVariantApiResponse>(
            uri,
            jsonSerializerSettings,
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
