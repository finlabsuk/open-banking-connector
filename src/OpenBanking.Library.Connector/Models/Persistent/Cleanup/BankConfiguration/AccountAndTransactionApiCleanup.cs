// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.BankConfiguration;

public class AccountAndTransactionApiCleanup
{
    public Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        ILogger logger)
    {
        IQueryable<AccountAndTransactionApiEntity> entityList =
            postgreSqlDbContext.AccountAndTransactionApi;


        return Task.CompletedTask;
    }
}
