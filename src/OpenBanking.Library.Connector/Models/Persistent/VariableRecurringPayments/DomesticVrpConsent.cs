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
            string? accessTokenRefreshToken,
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy,
            Guid bankRegistrationId,
            Guid variableRecurringPaymentsApiId,
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
            VariableRecurringPaymentsApiId = variableRecurringPaymentsApiId;
            ExternalApiId = externalApiId;
        }

        [ForeignKey("BankRegistrationId")]
        public BankRegistration BankRegistrationNavigation { get; set; } = null!;

        [ForeignKey("VariableRecurringPaymentsApiId")]
        public VariableRecurringPaymentsApiEntity VariableRecurringPaymentsApiNavigation { get; set; } = null!;

        public IList<DomesticVrpConsentAuthContext> DomesticVrpConsentAuthContextsNavigation { get; } =
            new List<DomesticVrpConsentAuthContext>();

        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     Associated VariableRecurringPaymentsApi object
        /// </summary>
        public Guid VariableRecurringPaymentsApiId { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }
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
