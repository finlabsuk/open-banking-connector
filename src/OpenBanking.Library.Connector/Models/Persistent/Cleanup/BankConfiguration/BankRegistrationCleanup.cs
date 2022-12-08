// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            postgreSqlDbContext.Set<BankRegistration>();

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

            // Populate redirect URIs in case of record created with earlier software version where
            // these were not persisted in the database
            if (processedSoftwareStatementProfile is not null &&
                string.IsNullOrEmpty(bankRegistration.DefaultRedirectUri))
            {
                string defaultRedirectUri = processedSoftwareStatementProfile.DefaultFragmentRedirectUrl;
                if (string.IsNullOrEmpty(defaultRedirectUri))
                {
                    throw new Exception(
                        $"Can't find defaultRedirectUri for software statement profile {softwareStatementProfileId} ");
                }

                List<string> otherRedirectUris =
                    processedSoftwareStatementProfile.SoftwareStatementPayload.SoftwareRedirectUris;
                otherRedirectUris.Remove(defaultRedirectUri);

                bankRegistration.DefaultRedirectUri = defaultRedirectUri;
                bankRegistration.OtherRedirectUris = otherRedirectUris;

                string message =
                    $"In its database record, BankRegistration with ID {bankRegistration.Id} specifies " +
                    $"use of software statement profile with ID {softwareStatementProfileId}";
                message += softwareStatementProfileOverride is null
                    ? ". "
                    : $" and override {softwareStatementProfileOverride}. ";
                message +=
                    $"This has been used to set {nameof(bankRegistration.DefaultRedirectUri)} and " +
                    $"{nameof(bankRegistration.OtherRedirectUris)} as part of database cleanup.";
                logger.LogInformation(message);
            }
        }
    }
}
