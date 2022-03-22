// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for ReadLocal.
    /// </summary>
    /// <typeparam name="TPublicQuery"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IReadLocalContext<TPublicQuery, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     READ local object by ID (does not include GETing object from bank API).
        ///     Object will be read from local database only.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> ReadLocalAsync(Guid id);

        /// <summary>
        ///     READ local object(s) by query (does not include GETing object(s) from bank API).
        ///     Object(s) will be read from local database only.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IFluentResponse<IQueryable<TPublicResponse>>> ReadLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate);
    }
}
