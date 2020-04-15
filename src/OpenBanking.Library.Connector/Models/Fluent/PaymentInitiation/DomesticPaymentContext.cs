// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class DomesticPaymentContext
    {
        internal DomesticPaymentContext(ISharedContext context)
        {
            Context = context.ArgNotNull(nameof(context));
        }

        internal ISharedContext Context { get; }

        internal DomesticPaymentRequest Data { get; set; }

        internal string ConsentId { get; set; }
    }
}
