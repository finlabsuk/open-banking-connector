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
    ///     Fluent interface for local entities. These are data objects persisted to DB locally but not at bank API.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TPublicQuery"></typeparam>
    public interface IGetContext<TPublicQuery, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     GET entity by ID from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> GetAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);

        /// <summary>
        ///     GET entity by query from Open Banking Connector.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IFluentResponse<IQueryable<TPublicResponse>>> GetLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate);
    }
}
