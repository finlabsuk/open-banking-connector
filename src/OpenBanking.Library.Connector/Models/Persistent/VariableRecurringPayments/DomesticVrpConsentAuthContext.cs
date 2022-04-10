// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticVrpConsentAuthContext :
        AuthContext,
        IDomesticVrpConsentAuthContextPublicQuery
    {
        public DomesticVrpConsentAuthContext(
            string? name,
            string? reference,
            Guid id,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string? accessTokenValue,
            int accessTokenExpiresIn,
            string? accessTokenRefreshToken,
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy,
            Guid domesticVrpConsentId) : base(
            name,
            reference,
            id,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy,
            accessTokenValue,
            accessTokenExpiresIn,
            accessTokenRefreshToken,
            accessTokenModified,
            accessTokenModifiedBy)
        {
            DomesticVrpConsentId = domesticVrpConsentId;
        }


        // Parent consent (optional to avoid warning due to non-support of global query filter)
        [ForeignKey("DomesticVrpConsentId")]
        public DomesticVrpConsent DomesticVrpConsentNavigation { get; set; } = null!;

        public Guid DomesticVrpConsentId { get; }
    }

    internal partial class DomesticVrpConsentAuthContext :
        ISupportsFluentLocalEntityGet<DomesticVrpConsentAuthContextReadLocalResponse>
    {
        public DomesticVrpConsentAuthContextReadLocalResponse PublicGetLocalResponse =>
            new DomesticVrpConsentAuthContextReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                DomesticVrpConsentId);
    }
}
