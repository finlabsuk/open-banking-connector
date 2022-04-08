// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
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
        public DomesticVrpConsentAuthContext() { }

        public DomesticVrpConsentAuthContext(
            Guid id,
            string? name,
            string? createdBy,
            ITimeProvider timeProvider,
            string? accessToken,
            int accessTokenExpiresIn,
            string? refreshToken,
            Guid domesticVrpConsentId) : base(
            id,
            name,
            createdBy,
            timeProvider,
            accessToken,
            accessTokenExpiresIn,
            refreshToken)
        {
            DomesticVrpConsentId = domesticVrpConsentId;
        }

        // Parent consent (optional to avoid warning due to non-support of global query filter)
        [ForeignKey("DomesticVrpConsentId")]
        public DomesticVrpConsent DomesticVrpConsentNavigation { get; set; } = null!;

        public Guid DomesticVrpConsentId { get; set; }
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
