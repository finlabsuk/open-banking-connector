// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction;
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

        foreach (AccountAndTransactionApiEntity accountAndTransactionApi in entityList)
        {
            // Update HSBC spec version
            if (accountAndTransactionApi.BaseUrl is
                "https://api.ob.firstdirect.com/obie/open-banking/v3.1/aisp" or
                "https://api.ob.business.hsbc.co.uk/obie/open-banking/v3.1/aisp" or
                "https://api.ob.hsbc.co.uk/obie/open-banking/v3.1/aisp")
            {
                if (accountAndTransactionApi.ApiVersion is not AccountAndTransactionApiVersion.Version3p1p10)
                {
                    string message =
                        $"In its database record, AccountAndTransactionApi with ID {accountAndTransactionApi.Id} specifies " +
                        $"use of API version {accountAndTransactionApi.ApiVersion}. ";
                    message +=
                        $"The API version of this record has been updated to {AccountAndTransactionApiVersion.Version3p1p10} " +
                        "as part of database cleanup.";
                    logger.LogInformation(message);
                    accountAndTransactionApi.ApiVersion = AccountAndTransactionApiVersion.Version3p1p10;
                }
            }
        }

        return Task.CompletedTask;
    }
}
