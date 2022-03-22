// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response
{
    public interface IAccountAccessConsentPublicQuery : IBaseQuery
    {
        Guid BankRegistrationId { get; }

        Guid BankApiSetId { get; }

        public string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to ReadLocal requests
    /// </summary>
    public class AccountAccessConsentReadLocalResponse : BaseResponse, IAccountAccessConsentPublicQuery
    {
        public AccountAccessConsentReadLocalResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId) : base(id, name, created, createdBy)
        {
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
            ExternalApiId = externalApiId;
        }

        /// <summary>
        ///     ID of associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     ID of associated BankApiSet object
        /// </summary>
        public Guid BankApiSetId { get; }

        /// <summary>
        ///     External (bank) API ID for this object
        /// </summary>
        public string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to Read and Create requests
    /// </summary>
    public class AccountAccessConsentReadResponse : AccountAccessConsentReadLocalResponse
    {
        public AccountAccessConsentReadResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId,
            AccountAndTransactionModelsPublic.OBReadConsentResponse1 externalApiResponse) : base(
            id,
            name,
            created,
            createdBy,
            bankRegistrationId,
            bankApiSetId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }


        /// <summary>
        ///     Response object OBReadConsentResponse1 from UK Open Banking Read-Write Account and Transaction API spec
        ///     <a
        ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.8r5/dist/openapi/account-info-openapi.yaml" />
        ///     v3.1.9r5 <a />. Open Banking Connector will automatically
        ///     translate <i>to</i> this from an older format for banks supporting an earlier spec version.
        /// </summary>
        public AccountAndTransactionModelsPublic.OBReadConsentResponse1 ExternalApiResponse { get; }
    }
}
