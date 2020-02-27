// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.Request.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public class ApiProfileContext
    {
        internal ApiProfileContext(OpenBankingContext context)
        {
            Context = context;
        }

        internal OpenBankingContext Context { get; }

        internal ApiProfile Data { get; set; }

        internal string Id { get; set; }

        internal string BankClientProfileId { get; set; }

        internal ApiVersion? ApiVersion { get; set; }

        internal string BaseUrl { get; set; }
    }
}
