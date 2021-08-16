// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface for local entities. These are data objects persisted to DB locally but not at bank API.
    /// </summary>
    public interface IDeleteContext
    {
        /// <summary>
        ///     DELETE entity from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy">Optional user name or comment for DB update when performing soft delete.</param>
        /// <returns></returns>
        Task<IFluentResponse> DeleteAsync(
            Guid id,
            string? modifiedBy = null,
            bool useRegistrationAccessToken = false);
    }
}
