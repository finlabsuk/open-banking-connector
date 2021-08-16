// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    public interface IPostContext<in TPublicRequest, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     POST object to Open Banking Connector (creates local and remote objects, includes bank API POST).
        /// </summary>
        /// <param name="publicRequest">Request object</param>
        /// <param name="createdBy">Optional user name or comment for DB record(s).</param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> PostAsync(
            TPublicRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }
}
