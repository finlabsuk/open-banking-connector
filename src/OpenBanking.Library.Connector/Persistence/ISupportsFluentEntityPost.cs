// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    internal interface
        ISupportsFluentEntityPost<in TPublicRequest, out TPublicResponse, in TApiRequest, in TApiResponse> :
            ISupportsFluentLocalEntityPost<TPublicRequest, TPublicResponse>
        where TApiRequest : class, ISupportsValidation
        where TApiResponse : class, ISupportsValidation
    {
        void UpdateBeforeApiPost(TApiRequest apiRequest);

        void UpdateAfterApiPost(TApiResponse apiResponse, string? modifiedBy, ITimeProvider timeProvider);
    }
}
