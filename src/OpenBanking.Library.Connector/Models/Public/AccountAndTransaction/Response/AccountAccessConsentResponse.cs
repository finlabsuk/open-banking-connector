// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response
{
    public interface IAccountAccessConsentPublicQuery : IBaseQuery
    {
        Guid BankRegistrationId { get; }

        Guid AccountAndTransactionApiId { get; }

        public string ExternalApiId { get; }
    }

    public abstract class AccountAccessConsentBaseResponse : ConsentResponseBase, IAccountAccessConsentPublicQuery
    {
        internal AccountAccessConsentBaseResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            IList<string>? warnings,
            Guid bankRegistrationId,
            string externalApiId,
            string? externalApiUserId,
            Guid accountAndTransactionApiId) : base(
            id,
            created,
            createdBy,
            reference,
            warnings,
            bankRegistrationId,
            externalApiId,
            externalApiUserId)
        {
            AccountAndTransactionApiId = accountAndTransactionApiId;
        }

        /// <summary>
        ///     ID of associated AccountAndTransactionApiEntity object
        /// </summary>
        public Guid AccountAndTransactionApiId { get; }
    }

    /// <summary>
    ///     Response to AccountAccessConsent Create requests.
    /// </summary>
    public class AccountAccessConsentCreateResponse : AccountAccessConsentBaseResponse
    {
        internal AccountAccessConsentCreateResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            IList<string>? warnings,
            Guid bankRegistrationId,
            string externalApiId,
            string? externalApiUserId,
            Guid accountAndTransactionApiId,
            OBReadConsentResponse1? externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            warnings,
            bankRegistrationId,
            externalApiId,
            externalApiUserId,
            accountAndTransactionApiId)
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
        public OBReadConsentResponse1? ExternalApiResponse { get; }
    }

    /// <summary>
    ///     Response to AccountAccessConsent Read requests.
    /// </summary>
    public class AccountAccessConsentReadResponse : AccountAccessConsentBaseResponse
    {
        internal AccountAccessConsentReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            IList<string>? warnings,
            Guid bankRegistrationId,
            string externalApiId,
            string? externalApiUserId,
            Guid accountAndTransactionApiId,
            OBReadConsentResponse1 externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            warnings,
            bankRegistrationId,
            externalApiId,
            externalApiUserId,
            accountAndTransactionApiId)
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
        public OBReadConsentResponse1 ExternalApiResponse { get; }
    }
}
