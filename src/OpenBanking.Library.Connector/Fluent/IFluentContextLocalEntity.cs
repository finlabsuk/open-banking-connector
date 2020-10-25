// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent interface for local entities. These are data objects persisted to DB locally but not at bank API.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TPublicQuery"></typeparam>
    public interface IFluentContextLocalEntity<TPublicRequest, TPublicResponse, TPublicQuery>
        where TPublicResponse : class
    {
        /// <summary>
        ///     POST entity to Open Banking Connector.
        /// </summary>
        /// <param name="publicRequest">Entity request object.</param>
        /// <param name="createdBy">Optional user name or comment for DB record.</param>
        /// <returns></returns>
        Task<FluentResponse<TPublicResponse>> PostAsync(TPublicRequest publicRequest, string? createdBy = null);

        /// <summary>
        ///     GET entity by ID from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FluentResponse<TPublicResponse>> GetAsync(Guid id);

        /// <summary>
        ///     GET entity by query from Open Banking Connector.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<FluentResponse<IQueryable<TPublicResponse>>> GetAsync(Expression<Func<TPublicQuery, bool>> predicate);

        /// <summary>
        ///     DELETE entity from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy">Optional user name or comment for DB update when performing soft delete.</param>
        /// <param name="hardNotSoftDelete">Remove record from DB instead of setting IsDeleted flag.</param>
        /// <returns></returns>
        Task<FluentResponse<EmptyClass>> DeleteAsync(
            Guid id,
            string? modifiedBy = null,
            bool hardNotSoftDelete = false);
    }
}
