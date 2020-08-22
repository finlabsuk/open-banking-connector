// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using OBWriteDomesticConsentResponse = FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBWriteDomesticConsentResponse4;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public class DomesticPaymentConsentResponse
    {
        internal DomesticPaymentConsentResponse(string authUrl, string consentId, OBWriteDomesticConsentResponse obWriteDomesticConsentResponse)
        {
            AuthUrl = authUrl;
            ConsentId = consentId;
            OBWriteDomesticConsentResponse = obWriteDomesticConsentResponse;
        }

        public string AuthUrl { get; }
        public string ConsentId { get; }
        public OBWriteDomesticConsentResponse OBWriteDomesticConsentResponse { get; }
    }
}
