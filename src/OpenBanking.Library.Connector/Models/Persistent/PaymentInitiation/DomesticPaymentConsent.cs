// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticPaymentConsent :
        BaseEntity,
        IDomesticPaymentConsentPublicQuery
    {
        public DomesticPaymentConsent(
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

        public IList<DomesticPaymentConsentAuthContext> DomesticPaymentConsentAuthContextsNavigation { get; } =
            new List<DomesticPaymentConsentAuthContext>();

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }

        public Guid BankRegistrationId { get; }

        public Guid BankApiSetId { get; }
    }

    internal partial class DomesticPaymentConsent :
        ISupportsFluentLocalEntityGet<DomesticPaymentConsentReadLocalResponse>
    {
        public DomesticPaymentConsentReadLocalResponse PublicGetLocalResponse =>
            new DomesticPaymentConsentReadLocalResponse(
                Id,
                Name,
                Created,
                CreatedBy,
                BankRegistrationId,
                BankApiSetId,
                ExternalApiId);
    }
}
