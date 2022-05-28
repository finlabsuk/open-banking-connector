// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
    /// <summary>
    ///     Fluent interface methods for DeleteLocal.
    /// </summary>
    public interface IDeleteLocalContext
    {
        /// <summary>
        ///     DELETE local object by ID (does not include DELETE-ing object from bank API).
        ///     Object will be deleted from local database only.
        ///     Note: deletions from local database are implemented via soft delete (i.e. a flag is set).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy">Optional user name or comment for DB update when performing soft delete.</param>
        /// <returns></returns>
        Task<ObjectDeleteResponse> DeleteLocalAsync(
            Guid id,
            string? modifiedBy = null);
    }

    internal interface IDeleteLocalContextInternal : IDeleteLocalContext, IBaseContextInternal
    {
        IObjectDelete DeleteLocalObject { get; }

        async Task<ObjectDeleteResponse> IDeleteLocalContext.DeleteLocalAsync(
            Guid id,
            string? modifiedBy)
        {
            IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages =
                await DeleteLocalObject.DeleteAsync(
                    id,
                    modifiedBy,
                    false);
            return new ObjectDeleteResponse();
        }
    }
}
