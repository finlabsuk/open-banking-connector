// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for ReadLocal.
/// </summary>
/// <typeparam name="TPublicQuery"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
public interface IReadLocalContext<TPublicQuery, TPublicResponse>
    where TPublicResponse : class
{
    private protected IObjectReadWithSearch<TPublicQuery, TPublicResponse, LocalReadParams> ReadLocalObject { get; }

    /// <summary>
    ///     READ local object(s) by query (does not include GETing object(s) from bank API).
    ///     Object(s) will be read from local database only.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    async Task<IQueryable<TPublicResponse>> ReadLocalAsync(Expression<Func<TPublicQuery, bool>> predicate)
    {
        (IQueryable<TPublicResponse> response,
                IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadLocalObject.ReadAsync(predicate);
        return response;
    }

    async Task<TPublicResponse> ReadLocalAsync(LocalReadParams readParams)
    {
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
            await ReadLocalObject.ReadAsync(readParams);

        return response;
    }
}
