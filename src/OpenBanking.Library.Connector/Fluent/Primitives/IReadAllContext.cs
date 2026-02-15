// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

public interface IReadAllContext<TPublicResponse>
    where TPublicResponse : class
{
    private protected IObjectReadAll<TPublicResponse, LocalReadAllParams> ReadAllObject { get; }

    async Task<TPublicResponse> ReadLocalAsync(LocalReadAllParams readParams)
    {
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadAllObject.ReadAllAsync(readParams);

        return response;
    }
}
