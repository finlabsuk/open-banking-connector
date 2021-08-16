// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    internal interface ISupportsFluentDeleteLocal<TSelf> :
        IEntity
        where TSelf : class, ISupportsFluentDeleteLocal<TSelf> { }
}
