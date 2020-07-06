// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model
{
    public class PaymentConsentResponse
    {
        public string AuthUrl { get; set; }
        public string ConsentId { get; set; }
        private OBWritePaymentConsentResponseDataApiStatus? Status { get; set; }
    }
}
