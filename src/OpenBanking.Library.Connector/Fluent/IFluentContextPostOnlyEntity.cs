// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    /// <summary>
    ///     Fluent interface for POST-only data objects. These are similar to entities except they are not persisted to DB
    ///     either locally or at bank API and
    ///     only a POST method is available.
    ///     The main current use case is passing authorisation redirect objects which relate to a consent of unknown type and
    ///     ID.
    ///     OBC will try to get a token from bank API and update the appropriate consent if the authorisation has been
    ///     successful.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    /// <typeparam name="TPublicQuery"></typeparam>
    public interface IFluentContextPostOnlyEntity<TPublicRequest, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     POST entity to Open Banking Connector.
        /// </summary>
        /// <param name="publicRequest">Entity request object.</param>
        /// <param name="modifiedBy">Optional user name or comment for DB record(s).</param>
        /// <returns></returns>
        Task<FluentResponse<TPublicResponse>> PostAsync(TPublicRequest publicRequest, string? modifiedBy = null);
    }
}
