// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    internal delegate Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        GetAsyncWrapperDelegate<TPublicResponse>(
            Guid id,
            ISharedContext context);

    /// <summary>
    ///     DB-persisted entity with public interface (request and response types and public query type to control permitted
    ///     public queries).
    /// </summary>
    /// <typeparam name="TSelf">Entity (persisted) type, must conform to public query interface</typeparam>
    /// <typeparam name="TPublicRequest">Public request type</typeparam>
    /// <typeparam name="TPublicGetResponse">Public response type, must conform to public query interface</typeparam>
    /// <typeparam name="TPublicQuery">
    ///     Public query type which should be set to public query interface. IMPORTANT: set this
    ///     correctly to control public queries.
    /// </typeparam>
    internal interface ISupportsFluentGet<TPublicGetResponse> :
        IEntity
    {
        string ExternalApiPath { get; }

        TPublicGetResponse PublicGetResponse { get; }


        /// <summary>
        ///     GET entity from bank API.
        ///     If entity contains cached bank API info, also refresh and save.
        ///     Response may include more bank API info than what is cached locally as there
        ///     may be limits on local caching e.g. relating to PII.
        /// </summary>
        GetAsyncWrapperDelegate<TPublicGetResponse> GetAsyncWrapper { get; }
    }
}
