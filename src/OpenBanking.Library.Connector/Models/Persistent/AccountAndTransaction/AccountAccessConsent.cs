// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using AccountAccessConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class AccountAccessConsent :
        BaseConsent
    {
        public AccountAccessConsent(
            Guid id,
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string? accessTokenAccessToken,
            int accessTokenExpiresIn,
            string? accessTokenRefreshToken,
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy,
            Guid bankRegistrationId,
            Guid accountAndTransactionApiId,
            string externalApiId) : base(
            id,
            reference,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy,
            accessTokenAccessToken,
            accessTokenExpiresIn,
            accessTokenRefreshToken,
            accessTokenModified,
            accessTokenModifiedBy)
        {
            BankRegistrationId = bankRegistrationId;
            AccountAndTransactionApiId = accountAndTransactionApiId;
            ExternalApiId = externalApiId;
        }

        [ForeignKey("BankRegistrationId")]
        public BankRegistration BankRegistrationNavigation { get; set; } = null!;

        [ForeignKey("AccountAndTransactionApiId")]
        public AccountAndTransactionApiEntity AccountAndTransactionApiNavigation { get; set; } = null!;

        public IList<AccountAccessConsentAuthContext> AccountAccessConsentAuthContextsNavigation { get; } =
            new List<AccountAccessConsentAuthContext>();

        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     Associated AccountAndTransactionApi object
        /// </summary>
        public Guid AccountAndTransactionApiId { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }
    }

    internal partial class AccountAccessConsent : ISupportsFluentLocalEntityGet<AccountAccessConsentReadLocalResponse>
    {
        public AccountAccessConsentReadLocalResponse PublicGetLocalResponse =>
            new(
                Id,
                Created,
                CreatedBy,
                BankRegistrationId,
                AccountAndTransactionApiId,
                ExternalApiId);
    }
}
