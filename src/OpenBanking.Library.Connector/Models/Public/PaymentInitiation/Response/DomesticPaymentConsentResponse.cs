// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using OBWriteDomesticConsentResponse =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.
    OBWriteDomesticConsentResponse4;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentPublicQuery
    {
        string Id { get; }
        OBWriteDomesticConsentResponse OBWriteDomesticConsentResponse { get; }
    }

    public class DomesticPaymentConsentResponse : IDomesticPaymentConsentPublicQuery
    {
        public DomesticPaymentConsentResponse(
            string? authUrl,
            string id,
            OBWriteDomesticConsentResponse obWriteDomesticConsentResponse)
        {
            AuthUrl = authUrl;
            Id = id;
            OBWriteDomesticConsentResponse = obWriteDomesticConsentResponse;
        }

        public string? AuthUrl { get; set; } // may be set after default initialisation code which doesn't set it
        public string Id { get; }
        public OBWriteDomesticConsentResponse OBWriteDomesticConsentResponse { get; }
    }
}
