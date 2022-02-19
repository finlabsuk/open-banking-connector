// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for Delete.
    /// </summary>
    public interface IDeleteContext
    {
        /// <summary>
        ///     DELETE object by ID (includes DELETE-ing object at bank API).
        ///     Object will be deleted at bank and also from local database if it is a Bank Registration or Consent.
        ///     Note: deletions from local database are implemented via soft delete (i.e. a flag is set).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy">Optional user name or comment for local DB update when performing soft delete.</param>
        /// <param name="useRegistrationAccessToken"></param>
        /// <returns></returns>
        Task<IFluentResponse> DeleteAsync(
            Guid id,
            string? modifiedBy = null,
            bool useRegistrationAccessToken = false);
    }
}
