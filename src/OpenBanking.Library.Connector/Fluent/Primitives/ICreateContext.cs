// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for Create.
    /// </summary>
    /// <typeparam name="TPublicRequest"></typeparam>
    /// <typeparam name="TPublicResponse"></typeparam>
    public interface ICreateContext<in TPublicRequest, TPublicResponse>
        where TPublicResponse : class
    {
        /// <summary>
        ///     CREATE object (includes POSTing object to bank API).
        ///     Object will be created at bank and also in local database if it is a Bank Registration or Consent.
        /// </summary>
        /// <param name="publicRequest">Request object</param>
        /// <param name="createdBy">Optional user name or comment for DB record(s).</param>
        /// <param name="apiRequestWriteFile"></param>
        /// <param name="apiResponseWriteFile"></param>
        /// <param name="apiResponseOverrideFile"></param>
        /// <returns></returns>
        Task<IFluentResponse<TPublicResponse>> CreateAsync(
            TPublicRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }
}
