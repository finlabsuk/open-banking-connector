// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.ExternalApi
{
    internal interface IApiRequests<in TApiRequest, TApiResponse> :
        IApiGetRequests<TApiResponse>,
        IApiPostRequests<TApiRequest, TApiResponse>
        where TApiResponse : class, ISupportsValidation
        where TApiRequest : class, ISupportsValidation { }
}
