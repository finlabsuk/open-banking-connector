// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class DomesticPaymentResponse : OpenBankingResponse
    {
        public DomesticPaymentResponse(OBWriteDataDomesticResponse data)
        {
            Data = data;
        }

        public DomesticPaymentResponse(OpenBankingResponseMessage message) : this(new[] { message })
        {
        }

        public DomesticPaymentResponse(IList<OpenBankingResponseMessage> messages) : base(messages)
        {
        }


        public OBWriteDataDomesticResponse Data { get; set; }
    }
}
