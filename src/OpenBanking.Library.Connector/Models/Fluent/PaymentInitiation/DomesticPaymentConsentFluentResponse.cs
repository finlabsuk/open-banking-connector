// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class DomesticPaymentConsentFluentResponse : FluentResponse
    {
        public DomesticPaymentConsentFluentResponse(FluentResponseMessage message) : this(new[] { message })
        {
        }

        public DomesticPaymentConsentFluentResponse(IList<FluentResponseMessage> messages) : base(messages)
        {
        }

        public DomesticPaymentConsentFluentResponse(PaymentConsentResponse data)
        {
            Data = data;
        }

        public PaymentConsentResponse Data { get; }
    }
}
