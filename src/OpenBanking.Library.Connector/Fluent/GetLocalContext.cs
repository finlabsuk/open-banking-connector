// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent interface for local entities. These are data objects persisted to DB locally but not at bank API.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TPublicQuery"></typeparam>
    public interface IGetLocalContext<TPublicQuery, TPublicLocalResponse>
        where TPublicLocalResponse : class
    {
        /// <summary>
        ///     GET entity by ID from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicLocalResponse>> GetLocalAsync(Guid id);

        /// <summary>
        ///     GET entity by query from Open Banking Connector.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<IFluentResponse<IQueryable<TPublicLocalResponse>>> GetLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate);
    }

    internal class
        GetLocalContext<TEntity, TPublicQuery, TPublicLocalResponse>
        : IGetLocalContext<TPublicQuery, TPublicLocalResponse>
        where TEntity : class, ISupportsFluentGetLocal<TEntity, TPublicQuery, TPublicLocalResponse>, new()
        where TPublicQuery : class
        where TPublicLocalResponse : class, TPublicQuery
    {
        private readonly ISharedContext _context;

        internal GetLocalContext(ISharedContext context)
        {
            _context = context;
        }

        public async Task<IFluentResponse<IQueryable<TPublicLocalResponse>>> GetLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                var (response, postEntityNonErrorMessages) =
                    await ISupportsFluentGetLocal<TEntity, TPublicQuery, TPublicLocalResponse>.GetLocalAsync(
                        predicate,
                        _context);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<IQueryable<TPublicLocalResponse>>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<IQueryable<TPublicLocalResponse>>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<IQueryable<TPublicLocalResponse>>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }

        public async Task<IFluentResponse<TPublicLocalResponse>> GetLocalAsync(Guid id)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                var (response, postEntityNonErrorMessages) =
                    await ISupportsFluentGetLocal<TEntity, TPublicQuery, TPublicLocalResponse>.GetLocalAsync(
                        id,
                        _context);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<TPublicLocalResponse>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicLocalResponse>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicLocalResponse>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }
}
