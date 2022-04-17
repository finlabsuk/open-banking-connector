// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        BaseConsent,
        IDomesticPaymentConsentPublicQuery
    {
        public DomesticPaymentConsent(
            Guid id,
            string? name,
            string? reference,
            bool isDeleted,
            DateTimeOffset isDeletedModified,
            string? isDeletedModifiedBy,
            DateTimeOffset created,
            string? createdBy,
            string? accessToken_AccessToken,
            int accessToken_ExpiresIn,
            string? accessToken_RefreshToken,
            DateTimeOffset accessTokenModified,
            string? accessTokenModifiedBy,
            Guid bankRegistrationId,
            Guid paymentInitiationApiId,
            string externalApiId) : base(
            id,
            name,
            reference,
            isDeleted,
            isDeletedModified,
            isDeletedModifiedBy,
            created,
            createdBy,
            accessToken_AccessToken,
            accessToken_ExpiresIn,
            accessToken_RefreshToken,
            accessTokenModified,
            accessTokenModifiedBy)
        {
            BankRegistrationId = bankRegistrationId;
            PaymentInitiationApiId = paymentInitiationApiId;
            ExternalApiId = externalApiId;
        }

        [ForeignKey("BankRegistrationId")]
        public BankRegistration BankRegistrationNavigation { get; set; } = null!;

        [ForeignKey("PaymentInitiationApiId")]
        public PaymentInitiationApiEntity PaymentInitiationApiNavigation { get; set; } = null!;

        public IList<DomesticPaymentConsentAuthContext> DomesticPaymentConsentAuthContextsNavigation { get; } =
            new List<DomesticPaymentConsentAuthContext>();

        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     Associated PaymentInitiationApi object
        /// </summary>
        public Guid PaymentInitiationApiId { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }
    }

    internal partial class DomesticPaymentConsent :
        ISupportsFluentLocalEntityGet<DomesticPaymentConsentReadLocalResponse>
    {
        public DomesticPaymentConsentReadLocalResponse PublicGetLocalResponse =>
            new(
                Id,
                Name,
                Created,
                CreatedBy,
                BankRegistrationId,
                PaymentInitiationApiId,
                ExternalApiId);
    }
}
