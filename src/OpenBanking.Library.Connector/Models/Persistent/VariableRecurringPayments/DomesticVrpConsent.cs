// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticVrpConsent :
        BaseConsent,
        IDomesticVrpConsentPublicQuery
    {
        public DomesticVrpConsent(
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
            Guid variableRecurringPaymentsApiId) : base(
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
            VariableRecurringPaymentsApiId = variableRecurringPaymentsApiId;
        }

        [ForeignKey("VariableRecurringPaymentsApiId")]
        public VariableRecurringPaymentsApiEntity VariableRecurringPaymentsApiNavigation { get; set; } = null!;

        public IList<DomesticVrpConsentAuthContext> DomesticVrpConsentAuthContextsNavigation { get; } =
            new List<DomesticVrpConsentAuthContext>();

        /// <summary>
        ///     Associated VariableRecurringPaymentsApi object
        /// </summary>
        public Guid VariableRecurringPaymentsApiId { get; }
    }

    internal partial class DomesticVrpConsent :
        ISupportsFluentLocalEntityGet<DomesticVrpConsentResponse>
    {
        public DomesticVrpConsentResponse PublicGetLocalResponse =>
            new(
                Id,
                Created,
                CreatedBy,
                Reference,
                BankRegistrationId,
                VariableRecurringPaymentsApiId,
                ExternalApiId,
                null);
    }
}
