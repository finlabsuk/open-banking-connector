﻿// Licensed to Finnovation Labs Limited under one or more agreements.
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
    public interface IDeleteLocalContext
    {
        /// <summary>
        ///     DELETE entity from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy">Optional user name or comment for DB update when performing soft delete.</param>
        /// <returns></returns>
        Task<IFluentResponse> DeleteLocalAsync(
            Guid id,
            string? modifiedBy = null);
    }

    internal class
        DeleteLocalContext<TEntity> :
            IDeleteLocalContext
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>
    {
        private readonly ISharedContext _context;

        internal DeleteLocalContext(ISharedContext context)
        {
            _context = context;
        }

        public async Task<IFluentResponse> DeleteLocalAsync(
            Guid id,
            string? modifiedBy)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages =
                    await ISupportsFluentDeleteLocal<TEntity>.DeleteAsync(
                        id,
                        modifiedBy,
                        _context);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse(nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                _context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }
}
