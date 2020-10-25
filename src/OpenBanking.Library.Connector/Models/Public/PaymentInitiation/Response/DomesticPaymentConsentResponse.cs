// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using OBWriteDomesticConsentResponse =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.
    OBWriteDomesticConsentResponse4;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentPublicQuery
    {
        Guid Id { get; }
        OBWriteDomesticConsentResponse OBWriteDomesticConsentResponse { get; }

        Guid BankRegistrationId { get; }

        Guid BankProfileId { get; }
    }

    public class DomesticPaymentConsentResponse : IDomesticPaymentConsentPublicQuery
    {
        public DomesticPaymentConsentResponse(
            string? authUrl,
            Guid id,
            OBWriteDomesticConsentResponse obWriteDomesticConsentResponse,
            Guid bankRegistrationId,
            Guid bankProfileId)
        {
            AuthUrl = authUrl;
            Id = id;
            OBWriteDomesticConsentResponse = obWriteDomesticConsentResponse;
            BankRegistrationId = bankRegistrationId;
            BankProfileId = bankProfileId;
        }

        public string? AuthUrl { get; set; } // may be set after default initialisation code which doesn't set it
        public Guid Id { get; }
        public OBWriteDomesticConsentResponse OBWriteDomesticConsentResponse { get; }
        public Guid BankRegistrationId { get; }
        public Guid BankProfileId { get; }
    }
}
