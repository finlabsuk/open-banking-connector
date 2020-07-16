// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public class PaymentConsentResponse
    {
        public string AuthUrl { get; set; }
        public string ConsentId { get; set; }
        private OBWriteDomesticResponse4Data.StatusEnum Status { get; set; }
    }
}
