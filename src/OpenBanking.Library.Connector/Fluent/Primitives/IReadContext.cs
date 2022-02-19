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
    ///     Fluent interface methods for Read.
    /// </summary>
    /// <typeparam name="TPublicQuery"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface IReadContext<TPublicQuery, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     READ object by ID (includes GETing object from bank API).
        ///     Object will be read from bank and also from local database if it is a Bank Registration or Consent.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> ReadAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);

        /// <summary>
        ///     READ objects by query (includes GETing object from bank API).
        ///     Object will be read from bank and also from local database if it is a Bank Registration or Consent.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IFluentResponse<IQueryable<TPublicResponse>>> ReadLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate);
    }
}
