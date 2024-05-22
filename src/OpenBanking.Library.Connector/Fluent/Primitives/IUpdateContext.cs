// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

public interface IUpdateContext<in TPublicRequest, TPublicResponse>
    where TPublicResponse : class
{
    private protected IObjectUpdate2<TPublicRequest, TPublicResponse> UpdateObject { get; }

    async Task<TPublicResponse> UpdateAsync(
        TPublicRequest request,
        LocalReadParams readParams)
    {
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await UpdateObject.UpdateAsync(request, readParams);

        return response;
    }
}
