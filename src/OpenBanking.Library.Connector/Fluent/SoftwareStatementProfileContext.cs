// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Model.Public;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public class SoftwareStatementProfileContext
    {
        public SoftwareStatementProfileContext(OpenBankingContext context)
        {
            Context = context.ArgNotNull(nameof(context));
            Data = new SoftwareStatementProfile();
        }

        internal OpenBankingContext Context { get; }

        internal SoftwareStatementProfile Data { get; set; }
    }
}
