// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.BankConfiguration;

public class BankRegistrationCleanup
{
    public async Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        ILogger logger)
    {
        IQueryable<BankRegistration> entityList =
            postgreSqlDbContext
                .BankRegistration;

        foreach (BankRegistration bankRegistration in entityList)
        {
            // Check if software statement profile available
            ProcessedSoftwareStatementProfile? processedSoftwareStatementProfile = null;
            string softwareStatementProfileId = bankRegistration.SoftwareStatementProfileId;
            string? softwareStatementProfileOverride = bankRegistration.SoftwareStatementProfileOverride;
            try
            {
                processedSoftwareStatementProfile = await processedSoftwareStatementProfileStore.GetAsync(
                    softwareStatementProfileId,
                    softwareStatementProfileOverride);
            }
            catch
            {
                string message =
                    $"In its database record, BankRegistration with ID {bankRegistration.Id} specifies " +
                    $"use of software statement profile with ID {softwareStatementProfileId}";
                message += softwareStatementProfileOverride is null
                    ? ". "
                    : $" and override {softwareStatementProfileOverride}. ";
                message += "This software statement profile was not found in configuration/secrets.";
                logger.LogWarning(message);
            }

            // Check for empty DefaultFragmentRedirectUri
            if (string.IsNullOrEmpty(bankRegistration.DefaultFragmentRedirectUri))
            {
                throw new Exception(
                    $"Null or empty DefaultFragmentRedirectUri found for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check for empty RedirectUris
            if (!bankRegistration.RedirectUris.Any())
            {
                var newRedirectUris =
                    new List<string>(bankRegistration.OtherRedirectUris)
                    {
                        bankRegistration.DefaultFragmentRedirectUri
                    };
                if (!newRedirectUris.Any())
                {
                    throw new Exception(
                        $"Cannot create non-empty RedirectUris for BankRegistration with ID {bankRegistration.Id}.");
                }
                bankRegistration.RedirectUris = newRedirectUris;
                string message =
                    $"RedirectUris for BankRegistration with ID {bankRegistration.Id} has been populated " +
                    $"as part of database cleanup.";
                logger.LogInformation(message);
            }

            // Prepare for removal of DbTransitionalDefault
            if (bankRegistration.BankGroup is BankGroupEnum.DbTransitionalDefault)
            {
                string message =
                    $"No suitable BankGroup could be found for BankRegistration with ID {bankRegistration.Id} " +
                    $"during database cleanup.";
                throw new Exception(message);
            }
        }
    }
}
