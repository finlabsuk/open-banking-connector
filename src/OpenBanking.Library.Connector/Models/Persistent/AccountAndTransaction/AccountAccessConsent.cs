// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using AccountAccessConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request.AccountAccessConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class AccountAccessConsent :
        EntityBase
    {
        public AccountAccessConsent() { }

        public AccountAccessConsent(
            Guid id,
            string? name,
            string externalApiId,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string? createdBy,
            ITimeProvider timeProvider) : base(id, name, createdBy, timeProvider)
        {
            ExternalApiId = externalApiId;
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
        }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        [ForeignKey("BankRegistrationId")]
        public BankRegistration BankRegistrationNavigation { get; set; } = null!;

        [ForeignKey("BankApiSetId")]
        public BankApiSet BankApiSetNavigation { get; set; } = null!;

        public IList<AccountAccessConsentAuthContext> AccountAccessConsentAuthContextsNavigation { get; set; } = null!;

        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; set; }

        /// <summary>
        ///     Associated BankApiSet object
        /// </summary>
        public Guid BankApiSetId { get; set; }

    }

    internal partial class AccountAccessConsent : ISupportsFluentLocalEntityGet<AccountAccessConsentReadLocalResponse>
    {
        public AccountAccessConsentReadLocalResponse PublicGetLocalResponse =>
            new AccountAccessConsentReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                BankRegistrationId,
                BankApiSetId,
                ExternalApiId);
    }
}
