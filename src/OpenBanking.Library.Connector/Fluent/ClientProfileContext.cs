// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class ClientProfileContext
    {
        internal ClientProfileContext(OpenBankingContext context)
        {
            Context = context.ArgNotNull(nameof(context));
            Data = new BankClientProfile
            {
                BankClient = new BankClient()
            };
        }

        internal OpenBankingContext Context { get; }
        internal BankClientProfile Data { get; set; }

        internal string SoftwareStatementProfileId { get; set; }
    }
}
