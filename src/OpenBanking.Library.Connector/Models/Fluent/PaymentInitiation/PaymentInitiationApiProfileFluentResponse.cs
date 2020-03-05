// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class PaymentInitiationApiProfileFluentResponse : OpenBankingResponse
    {
        public PaymentInitiationApiProfileFluentResponse(PaymentInitiationApiProfileResponse data)
            : this((IList<OpenBankingResponseMessage>) null, data)
        {
        }

        public PaymentInitiationApiProfileFluentResponse(OpenBankingResponseMessage message,
            PaymentInitiationApiProfileResponse data)
            : this(new[] { message }, data)
        {
        }

        public PaymentInitiationApiProfileFluentResponse(IList<OpenBankingResponseMessage> messages,
            PaymentInitiationApiProfileResponse data)
            : base(messages)
        {
            Data = data;
        }

        public PaymentInitiationApiProfileResponse Data { get; }
    }
}
