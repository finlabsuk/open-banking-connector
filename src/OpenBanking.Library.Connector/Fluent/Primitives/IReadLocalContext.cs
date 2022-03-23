// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

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

    internal interface
        IReadLocalContextInternal<TPublicQuery, TPublicResponse> : IReadLocalContext<TPublicQuery, TPublicResponse>,
            IBaseContextInternal
        where TPublicResponse : class
    {
        IObjectReadLocal<TPublicQuery, TPublicResponse> ReadLocalObject { get; }


        async Task<IFluentResponse<TPublicResponse>> IReadLocalContext<TPublicQuery, TPublicResponse>.ReadLocalAsync(
            Guid id)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await ReadLocalObject.ReadAsync(id);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<TPublicResponse>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<TPublicResponse>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }

        async Task<IFluentResponse<IQueryable<TPublicResponse>>> IReadLocalContext<TPublicQuery, TPublicResponse>.
            ReadLocalAsync(Expression<Func<TPublicQuery, bool>> predicate)
        {
            // Create non-error list
            var nonErrorMessages =
                new List<IFluentResponseInfoOrWarningMessage>();

            try
            {
                (IQueryable<TPublicResponse> response,
                        IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages) =
                    await ReadLocalObject.ReadAsync(predicate);
                nonErrorMessages.AddRange(postEntityNonErrorMessages);

                // Return success response (thrown exceptions produce error response)
                return new FluentSuccessResponse<IQueryable<TPublicResponse>>(
                    response,
                    nonErrorMessages);
            }
            catch (AggregateException ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<IQueryable<TPublicResponse>>(
                    messages: ex.CreateOtherErrorMessages()
                        .ToList()); // ToList() is workaround for IList to IReadOnlyList conversion; see https://github.com/dotnet/runtime/issues/31001
            }
            catch (Exception ex)
            {
                Context.Instrumentation.Exception(ex);

                return new FluentOtherErrorResponse<IQueryable<TPublicResponse>>(
                    new List<FluentResponseOtherErrorMessage> { ex.CreateOtherErrorMessage() });
            }
        }
    }
}
