// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class DomesticPaymentFluentResponse : FluentResponse
    {
        public DomesticPaymentFluentResponse(OBWriteDataDomesticResponse data)
        {
            Data = data;
        }

        public DomesticPaymentFluentResponse(FluentResponseMessage message) : this(new[] { message }) { }

        public DomesticPaymentFluentResponse(IList<FluentResponseMessage> messages) : base(messages) { }


        public OBWriteDataDomesticResponse Data { get; set; }
    }
}
