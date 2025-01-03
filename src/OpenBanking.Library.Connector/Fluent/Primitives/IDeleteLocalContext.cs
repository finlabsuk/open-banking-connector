// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for DeleteLocal.
/// </summary>
public interface IDeleteLocalContext
{
    private protected IObjectDelete<LocalDeleteParams> DeleteLocalObject { get; }

    /// <summary>
    ///     DELETE local object by ID (does not include DELETE-ing object from bank API).
    ///     Object will be deleted from local database only.
    ///     Note: deletions from local database are implemented via soft delete (i.e. a flag is set).
    /// </summary>
    /// <param name="deleteParams"></param>
    /// <returns></returns>
    async Task<BaseResponse> DeleteLocalAsync(LocalDeleteParams deleteParams)
    {
        IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages =
            await DeleteLocalObject.DeleteAsync(deleteParams);
        return new BaseResponse();
    }
}
