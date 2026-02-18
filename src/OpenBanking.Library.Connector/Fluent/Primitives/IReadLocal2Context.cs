// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for ReadLocal.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadLocal2Context<TPublicResponse>
    where TPublicResponse : class
{
    private protected IObjectRead<TPublicResponse, LocalReadParams> ReadLocalObject { get; }

    async Task<TPublicResponse> ReadLocalAsync(LocalReadParams readParams)
    {
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadLocalObject.ReadAsync(readParams);

        return response;
    }
}
