// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    ///     DB-persisted entity with public interface (request and response types and public query type to control permitted
    ///     public queries).
    /// </summary>
    /// <typeparam name="TSelf">Entity (persisted) type, must conform to public query interface</typeparam>
    /// <typeparam name="TPublicRequest">Public request type</typeparam>
    /// <typeparam name="TPublicResponse">Public response type, must conform to public query interface</typeparam>
    /// <typeparam name="TPublicQuery">
    ///     Public query type which should be set to public query interface. IMPORTANT: set this
    ///     correctly to control public queries.
    /// </typeparam>
    internal interface ISupportsFluentGetLocal<TSelf, TPublicQuery, TPublicGetLocalResponse> :
        IEntity
        where TSelf : class, ISupportsFluentGetLocal<TSelf, TPublicQuery, TPublicGetLocalResponse>
    {
        TPublicGetLocalResponse PublicGetLocalResponse { get; }

        static async Task<(TPublicGetLocalResponse response, IList<IFluentResponseInfoOrWarningMessage>)> GetLocalAsync(
            Guid id,
            ISharedContext context)
        {
            GetLocalEntity<TSelf, TPublicQuery, TPublicGetLocalResponse> i =
                new GetLocalEntity<TSelf, TPublicQuery, TPublicGetLocalResponse>(
                    context.DbService.GetDbEntityMethodsClass<TSelf>());

            (TPublicGetLocalResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages) result =
                await i.GetLocalAsync(id);
            return result;
        }

        static async
            Task<(IQueryable<TPublicGetLocalResponse> response, IList<IFluentResponseInfoOrWarningMessage>
                nonErrorMessages
                )>
            GetLocalAsync(
                Expression<Func<TPublicQuery, bool>> predicate,
                ISharedContext context)
        {
            GetLocalEntity<TSelf, TPublicQuery, TPublicGetLocalResponse> i =
                new GetLocalEntity<TSelf, TPublicQuery, TPublicGetLocalResponse>(
                    context.DbService.GetDbEntityMethodsClass<TSelf>());

            (IQueryable<TPublicGetLocalResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)
                result =
                    await i.GetLocalAsync(predicate);
            return result;
        }
    }
}
