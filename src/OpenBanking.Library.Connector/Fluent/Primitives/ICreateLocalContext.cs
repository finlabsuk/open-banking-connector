// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for CreateLocal.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicPostResponse"></typeparam>
    public interface ICreateLocalContext<in TPublicRequest, TPublicPostResponse>
        where TPublicPostResponse : class
    {
        /// <summary>
        ///     CREATE local object (does not include POSTing object to bank API).
        ///     Object will be created in local database only.
        /// </summary>
        /// <param name="publicRequest">Request object</param>
        /// <param name="createdBy">Optional user name or comment for DB record(s).</param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicPostResponse>> CreateLocalAsync(
            TPublicRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }
}
