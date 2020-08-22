// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation
{
    public class DomesticPaymentConsent : IEntity
    {
        internal DomesticPaymentConsent() { }

        internal DomesticPaymentConsent(
            string state,
            string bankProfileId,
            OBWriteDomesticConsent4 obWriteDomesticConsent,
            OBWriteDomesticConsentResponse4 obWriteDomesticResponse,
            string id)
        {
            State = state;
            BankProfileId = bankProfileId;
            ObWriteDomesticConsent = obWriteDomesticConsent;
            ObWriteDomesticResponse = obWriteDomesticResponse;
            Id = id;
        }

        public string State { get; set; } = null!;

        public string BankProfileId { get; set; } = null!;

        public OBWriteDomesticConsent4 ObWriteDomesticConsent { get; set; } = null!;

        public OBWriteDomesticConsentResponse4 ObWriteDomesticResponse { get; set; } = null!;

        public TokenEndpointResponse? TokenEndpointResponse { get; set; }

        public string OBId => ObWriteDomesticResponse.Data.ConsentId;

        public string Id { get; set; } = null!;
    }
}
