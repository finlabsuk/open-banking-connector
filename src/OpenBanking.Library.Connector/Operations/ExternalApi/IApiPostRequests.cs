// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal interface IApiPostRequests<in TApiRequest, TApiResponse>
        where TApiRequest : class, ISupportsValidation
        where TApiResponse : class, ISupportsValidation
    {
        Task<(TApiResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
            PostAsync(
                Uri uri,
                TApiRequest request,
                JsonSerializerSettings? jsonSerializerSettings,
                IApiClient apiClient,
                IApiVariantMapper mapper);
    }
}
