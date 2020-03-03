// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.Request;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class BankClientProfileContext
    {
        internal BankClientProfileContext(OpenBankingContext context)
        {
            Context = context.ArgNotNull(nameof(context));
        }

        internal OpenBankingContext Context { get; }
        internal BankClientProfile Data { get; set; }
    }
}
