// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Delete.
/// </summary>
public interface IDeleteConsentContext
{
    /// <summary>
    ///     DELETE object by ID (includes DELETE-ing object at bank API).
    ///     Object will be deleted at bank and also from local database if it is a Bank Registration or Consent.
    ///     Note: deletions from local database are implemented via soft delete (i.e. a flag is set).
    /// </summary>
    /// <param name="id"> </param>
    /// <param name="modifiedBy">Optional user name or comment for local DB update when performing soft delete.</param>
    /// <param name="includeExternalApiOperation"></param>
    /// <returns></returns>
    Task<BaseResponse> DeleteAsync(
        Guid id,
        string? modifiedBy = null,
        bool includeExternalApiOperation = true);
}

internal interface IDeleteConsentContextInternal : IDeleteConsentContext
{
    IObjectDelete<ConsentDeleteParams> DeleteObject { get; }

    async Task<BaseResponse> IDeleteConsentContext.DeleteAsync(
        Guid id,
        string? modifiedBy,
        bool includeExternalApiOperation)
    {
        var consentDeleteParams = new ConsentDeleteParams(id, modifiedBy, includeExternalApiOperation);
        IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages =
            await DeleteObject.DeleteAsync(consentDeleteParams);

        return new BaseResponse();
    }
}
