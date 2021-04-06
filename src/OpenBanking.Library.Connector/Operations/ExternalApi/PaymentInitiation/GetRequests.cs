// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi.PaymentInitiation
{
    internal class GetRequests<TApiResponse>
        where TApiResponse : class, ISupportsValidation
    {
        public async Task<(TApiResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            GetAsync<TVariantApiResponse>(
                Uri uri,
                GetRequestProcessor requestProcessor,
                JsonSerializerSettings? jsonSerializerSettings,
                IApiClient apiClient,
                IApiVariantMapper mapper)
            where TVariantApiResponse : class
        {
            var nonErrorMessages = new List<IFluentResponseInfoOrWarningMessage>();

            // Process request
            var variantResponse = await requestProcessor.GetAsync<TVariantApiResponse>(
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
                responseValidationResult.ProcessValidationResultsAndRaiseErrors(messagePrefix: "prefix");
            nonErrorMessages.AddRange(responseNonErrorMessages);
            return (response, nonErrorMessages);
        }
    }
}
