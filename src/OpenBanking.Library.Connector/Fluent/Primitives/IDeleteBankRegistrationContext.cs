// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives
{
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
        /// <param name="includeExternalApiOperation"></param>
        /// <param name="useRegistrationAccessToken"></param>
        /// <param name="bankProfile"></param>
        /// <returns></returns>
        Task<ObjectDeleteResponse> DeleteAsync(
            Guid id,
            string? modifiedBy = null,
            bool? includeExternalApiOperation = null,
            bool? useRegistrationAccessToken = null,
            BankProfileEnum? bankProfile = null);
    }

    internal interface IDeleteBankRegistrationContextInternal : IDeleteBankRegistrationContext, IBaseContextInternal
    {
        IObjectDelete<BankRegistrationDeleteParams> DeleteObject { get; }

        async Task<ObjectDeleteResponse> IDeleteBankRegistrationContext.DeleteAsync(
            Guid id,
            string? modifiedBy,
            bool? includeExternalApiOperation,
            bool? useRegistrationAccessToken,
            BankProfileEnum? bankProfile)
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
}
