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
        AccountAndTransactionModelsPublic.OBReadConsentResponse1 OBReadConsentResponse { get; }

        Guid BankRegistrationId { get; }

        Guid BankApiSetId { get; }
    }

    /// <summary>
    ///     Response object used when creating or reading an AccountAccessConsent object. Includes a UK Open Banking response
    ///     object
    ///     plus local database fields.
    /// </summary>
    public class AccountAccessConsentResponse : BaseResponse, IAccountAccessConsentPublicQuery
    {
        public AccountAccessConsentResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            AccountAndTransactionModelsPublic.OBReadConsentResponse1 obReadConsentResponse,
            Guid bankRegistrationId,
            Guid bankApiSetId) : base(id, name, created, createdBy)
        {
            OBReadConsentResponse = obReadConsentResponse;
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
        }

        /// <summary>
        ///     Response object OBReadConsentResponse1 from UK Open Banking Read-Write Account and Transaction API spec
        ///     <a
        ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.8r5/dist/openapi/account-info-openapi.yaml" />
        ///     v3.1.9r5 <a />. Open Banking Connector will automatically
        ///     translate <i>to</i> this from an older format for banks supporting an earlier spec version.
        /// </summary>
        public AccountAndTransactionModelsPublic.OBReadConsentResponse1 OBReadConsentResponse { get; }

        /// <summary>
        ///     ID of associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     ID of associated BankApiSet object
        /// </summary>
        public Guid BankApiSetId { get; }
    }
}
