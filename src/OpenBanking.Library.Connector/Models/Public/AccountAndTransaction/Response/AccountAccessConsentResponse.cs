// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using AccountAndTransactionModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response
{
    public interface IAccountAccessConsentPublicQuery : IBaseQuery
    {
        Guid BankRegistrationId { get; }

        Guid AccountAndTransactionApiId { get; }

        public string ExternalApiId { get; }
    }

    /// <summary>
    ///     Response to AccountAccessConsent Read and Create requests.
    /// </summary>
    public class AccountAccessConsentResponse : LocalObjectBaseResponse, IAccountAccessConsentPublicQuery
    {
        internal AccountAccessConsentResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankRegistrationId,
            Guid accountAndTransactionApiId,
            string externalApiId,
            AccountAndTransactionModelsPublic.OBReadConsentResponse1? externalApiResponse,
            IList<string>? warnings) : base(id, created, createdBy, reference)
        {
            BankRegistrationId = bankRegistrationId;
            AccountAndTransactionApiId = accountAndTransactionApiId;
            ExternalApiId = externalApiId;
            ExternalApiResponse = externalApiResponse;
            Warnings = warnings;
        }

        /// <summary>
        ///     Response object OBReadConsentResponse1 from UK Open Banking Read-Write Account and Transaction API spec
        ///     <a
        ///         href="https://github.com/OpenBankingUK/read-write-api-specs/blob/v3.1.8r5/dist/openapi/account-info-openapi.yaml" />
        ///     v3.1.9r5 <a />. Open Banking Connector will automatically
        ///     translate <i>to</i> this from an older format for banks supporting an earlier spec version.
        /// </summary>
        public AccountAndTransactionModelsPublic.OBReadConsentResponse1? ExternalApiResponse { get; }

        /// <summary>
        ///     Optional list of warning messages from Open Banking Connector.
        /// </summary>
        public IList<string>? Warnings { get; }

        /// <summary>
        ///     ID of associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     ID of associated AccountAndTransactionApiEntity object
        /// </summary>
        public Guid AccountAndTransactionApiId { get; }

        /// <summary>
        ///     External (bank) API ID for this object
        /// </summary>
        public string ExternalApiId { get; }
    }
}
