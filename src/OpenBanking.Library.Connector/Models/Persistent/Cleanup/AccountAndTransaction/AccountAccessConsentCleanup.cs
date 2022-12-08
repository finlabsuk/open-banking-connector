// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.AccountAndTransaction;

public class AccountAccessConsentCleanup
{
    public Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        ILogger logger)
    {
        IQueryable<AccountAccessConsent> entityList =
            postgreSqlDbContext.Set<AccountAccessConsent>();

        var negativeInfinity = new DateTimeOffset(
            new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
            new TimeSpan(0, 0, 0, 0, 0));

        foreach (AccountAccessConsent accountAccessConsent in entityList)
        {
            // Fix -infinity values for ExternalApiUserIdModified that are created by migrations
            if (accountAccessConsent.ExternalApiUserIdModified.EqualsExact(negativeInfinity))
            {
                DateTimeOffset newModified = accountAccessConsent.Created;
                accountAccessConsent.UpdateExternalApiUserId(
                    accountAccessConsent.ExternalApiUserId,
                    newModified,
                    accountAccessConsent.ExternalApiUserIdModifiedBy);

                string message =
                    $"In its database record, AccountAccessConsent with ID {accountAccessConsent.Id} specifies " +
                    $"{nameof(accountAccessConsent.ExternalApiUserIdModified)} as -infinity. This was likely set during a database migration " +
                    "and has been fixed as part of database cleanup.";
                logger.LogInformation(message);
            }
        }

        return Task.CompletedTask;
    }
}
