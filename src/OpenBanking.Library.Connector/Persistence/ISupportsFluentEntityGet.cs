// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ExternalApiBase;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    internal interface ISupportsFluentEntityGet<out TPublicResponse, in TApiResponse> :
        ISupportsFluentLocalEntityGet<TPublicResponse>
        where TApiResponse : class, ISupportsValidation
    {
        void UpdateAfterApiGet(TApiResponse apiResponse, string? modifiedBy, ITimeProvider timeProvider);
    }
}
