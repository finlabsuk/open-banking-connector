// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        BaseEntity,
        IDomesticVrpConsentPublicQuery
    {
        public DomesticVrpConsent(
            string? name,
            string? reference,
            Guid id,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string externalApiId,
            Guid bankRegistrationId,
            Guid bankApiSetId) : base(
            name,
            reference,
            id,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy)
        {
            ExternalApiId = externalApiId;
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
        }


        [ForeignKey("BankRegistrationId")]
        public BankRegistration BankRegistrationNavigation { get; set; } = null!;

        [ForeignKey("BankApiSetId")]
        public BankApiSet BankApiSetNavigation { get; set; } = null!;

        public IList<DomesticVrpConsentAuthContext> DomesticVrpConsentAuthContextsNavigation { get; } =
            new List<DomesticVrpConsentAuthContext>();

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }

        public Guid BankRegistrationId { get; }

        public Guid BankApiSetId { get; }
    }

    internal partial class DomesticVrpConsent :
        ISupportsFluentLocalEntityGet<DomesticVrpConsentReadLocalResponse>
    {
        public DomesticVrpConsentReadLocalResponse PublicGetLocalResponse =>
            new DomesticVrpConsentReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                BankRegistrationId,
                BankApiSetId,
                ExternalApiId);
    }
}
