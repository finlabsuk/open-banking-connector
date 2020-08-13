// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class DomesticPaymentFluentResponse : FluentResponse
    {
        public DomesticPaymentFluentResponse(OBWriteDomesticResponse4Data data)
        {
            Data = data;
        }

        public DomesticPaymentFluentResponse(FluentResponseMessage message) : this(new[] { message }) { }

        public DomesticPaymentFluentResponse(IList<FluentResponseMessage> messages) : base(messages) { }


        public OBWriteDomesticResponse4Data Data { get; set; }
    }
}
