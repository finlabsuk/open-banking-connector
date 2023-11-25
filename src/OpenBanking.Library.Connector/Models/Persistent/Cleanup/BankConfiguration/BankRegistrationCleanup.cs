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

            // Check for empty RedirectUris
            if (!bankRegistration.RedirectUris.Any())
            {
                throw new Exception(
                    $"Found non-empty RedirectUris for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check for empty DefaultFragmentRedirectUri
            if (string.IsNullOrEmpty(bankRegistration.DefaultFragmentRedirectUri))
            {
                throw new Exception(
                    $"Null or empty DefaultFragmentRedirectUri found for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check DefaultFragmentRedirectUri
            if (!bankRegistration.RedirectUris.Contains(bankRegistration.DefaultFragmentRedirectUri))
            {
                throw new Exception(
                    $"DefaultFragmentRedirectUri not included " +
                    $"in RedirectUris for BankRegistration with ID {bankRegistration.Id}.");
            }

            // Check for empty DefaultQueryRedirectUri
            if (string.IsNullOrEmpty(bankRegistration.DefaultQueryRedirectUri))
            {
                if (processedSoftwareStatementProfile is not null)
                {
                    string defaultQueryRedirectUrl = processedSoftwareStatementProfile.DefaultQueryRedirectUrl;
                    if (string.IsNullOrEmpty(defaultQueryRedirectUrl))
                    {
                        throw new Exception(
                            $"Can't find DefaultQueryRedirectUrl for software statement profile {softwareStatementProfileId} ");
                    }
                    if (!bankRegistration.RedirectUris.Contains(defaultQueryRedirectUrl))
                    {
                        throw new Exception(
                            $"DefaultQueryRedirectUrl for software statement profile {softwareStatementProfileId} " +
                            $"not included in RedirectUris for BankRegistration with ID {bankRegistration.Id}.");
                    }
                    bankRegistration.DefaultQueryRedirectUri = defaultQueryRedirectUrl;
                }
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
