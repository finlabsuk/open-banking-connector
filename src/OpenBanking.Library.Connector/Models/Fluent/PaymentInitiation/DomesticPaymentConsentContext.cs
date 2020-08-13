// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Fluent.PaymentInitiation
{
    public class DomesticPaymentConsentContext
    {
        internal DomesticPaymentConsentContext(ISharedContext context)
        {
            Context = context.ArgNotNull(nameof(context));
            Data = new DomesticPaymentConsent();
        }

        internal ISharedContext Context { get; }

        internal DomesticPaymentConsent Data { get; set; }
    }
}
