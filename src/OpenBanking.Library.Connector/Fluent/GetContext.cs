// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
    public interface IGetContext<TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     GET entity by ID from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> GetAsync(Guid id);
    }

    internal class
        GetContext<TEntity, TPublicResponse>
        : IGetContext<TPublicResponse>
        where TEntity : class, ISupportsFluentGet<TPublicResponse>, new()
        where TPublicResponse : class
    {
        private readonly ISharedContext _context;

        internal GetContext(ISharedContext context)
        {
            _context = context;
        }

        public async Task<IFluentResponse<TPublicResponse>> GetAsync(Guid id)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                var (response, postEntityNonErrorMessages) =
                    await new TEntity().GetAsyncWrapper(
                        id,
                        _context);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<TPublicResponse>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }
}
