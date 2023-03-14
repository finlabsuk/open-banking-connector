// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;

/// <summary>
///     Fluent interface methods for Delete.
/// </summary>
public interface IDeleteBankRegistrationContext
{
    /// <summary>
    ///     DELETE object by ID (includes DELETE-ing object at bank API).
    ///     Object will be deleted at bank and also from local database if it is a Bank Registration or Consent.
    ///     Note: deletions from local database are implemented via soft delete (i.e. a flag is set).
    /// </summary>
    /// <param name="id"> </param>
    /// <param name="modifiedBy">Optional user name or comment for local DB update when performing soft delete.</param>
    /// <param name="bankProfile">
    ///     BankProfile used to supply default values for unspecified properties. Use null to not specify
    ///     a bank profile.
    /// </param>
    /// <param name="includeExternalApiOperation">
    ///     Include deletion of corresponding object at external API endpoint. When null,
    ///     BankProfile is used to set value.
    /// </param>
    /// <param name="useRegistrationAccessToken">
    ///     Use registration access token when deleting object at external API. When null, BankProfile is used
    ///     to set value if deleting object at external API.
    /// </param>
    /// <returns></returns>
    Task<ObjectDeleteResponse> DeleteAsync(
        Guid id,
        string? modifiedBy = null,
        BankProfileEnum? bankProfile = null,
        bool? includeExternalApiOperation = null,
        bool? useRegistrationAccessToken = null);
}

internal interface IDeleteBankRegistrationContextInternal : IDeleteBankRegistrationContext
{
    IObjectDelete<BankRegistrationDeleteParams> DeleteObject { get; }

    async Task<ObjectDeleteResponse> IDeleteBankRegistrationContext.DeleteAsync(
        Guid id,
        string? modifiedBy,
        BankProfileEnum? bankProfile,
        bool? includeExternalApiOperation,
        bool? useRegistrationAccessToken)
    {
        var bankRegistrationDeleteParams = new BankRegistrationDeleteParams(
            id,
            modifiedBy,
            includeExternalApiOperation,
            useRegistrationAccessToken,
            bankProfile);
        IList<IFluentResponseInfoOrWarningMessage> postEntityNonErrorMessages =
            await DeleteObject.DeleteAsync(bankRegistrationDeleteParams);

        return new ObjectDeleteResponse();
    }
}
