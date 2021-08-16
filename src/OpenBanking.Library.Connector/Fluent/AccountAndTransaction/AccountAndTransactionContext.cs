// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.AccountAndTransaction
{
    public interface IAccountAndTransactionContext { }

    internal class AccountAndTransactionContext : IAccountAndTransactionContext
    {
        private readonly ISharedContext _sharedContext;

        public AccountAndTransactionContext(ISharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }
    }
}
