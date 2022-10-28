// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

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
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy,
            string? accessTokenRefreshToken,
            Guid bankRegistrationId,
            string externalApiId,
            string? authContextState,
            string? authContextNonce,
            DateTimeOffset authContextModified,
            string? authContextModifiedBy,
            Guid accountAndTransactionApiId) : base(
            id,
            reference,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy,
            accessTokenAccessToken,
            accessTokenExpiresIn,
            accessTokenModified,
            accessTokenModifiedBy,
            accessTokenRefreshToken,
            bankRegistrationId,
            externalApiId,
            authContextState,
            authContextNonce,
            authContextModified,
            authContextModifiedBy)
        {
            AccountAndTransactionApiId = accountAndTransactionApiId;
        }

        [ForeignKey("AccountAndTransactionApiId")]
        public AccountAndTransactionApiEntity AccountAndTransactionApiNavigation { get; set; } = null!;

        public IList<AccountAccessConsentAuthContext> AccountAccessConsentAuthContextsNavigation { get; } =
            new List<AccountAccessConsentAuthContext>();

        /// <summary>
        ///     Associated AccountAndTransactionApi object
        /// </summary>
        public Guid AccountAndTransactionApiId { get; }
    }

    internal partial class AccountAccessConsent : ISupportsFluentLocalEntityGet<AccountAccessConsentResponse>
    {
        public AccountAccessConsentResponse PublicGetLocalResponse =>
            new(
                Id,
                Created,
                CreatedBy,
                Reference,
                BankRegistrationId,
                AccountAndTransactionApiId,
                ExternalApiId,
                null,
                null);
    }
}
