// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Read.
/// </summary>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadAccountAccessConsentExternalEntityContext<TPublicResponse>
    where TPublicResponse : class
{
    private protected IAccountAccessConsentExternalRead<TPublicResponse, AccountAccessConsentExternalReadParams>
        ReadObject { get; }

    /// <summary>
    ///     READ objects using consent ID (includes GETing objects from bank API).
    ///     Objects will be read from bank database only.
    /// </summary>
    /// <param name="readParams"></param>
    /// <returns></returns>
    async Task<TPublicResponse> ReadAsync(AccountAccessConsentExternalReadParams readParams)
    {
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadObject.ReadAsync(readParams);

        return response;
    }
}
