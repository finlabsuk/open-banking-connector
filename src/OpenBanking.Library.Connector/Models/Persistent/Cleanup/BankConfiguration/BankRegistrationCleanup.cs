// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.BankConfiguration;

public class BankRegistrationCleanup
{
    public async Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        ILogger logger)
    {
        List<BankRegistrationEntity> entityList =
            postgreSqlDbContext
                .BankRegistration
                .ToList();

        DbSet<SoftwareStatementEntity> sList =
            postgreSqlDbContext
                .SoftwareStatement;

        DbSet<ObWacCertificateEntity> obwacList =
            postgreSqlDbContext
                .ObWacCertificate;

        DbSet<ObSealCertificateEntity> obsealList =
            postgreSqlDbContext
                .ObSealCertificate;

        DateTimeOffset utcNow = DateTimeOffset.UtcNow;
        var databaseUser = "Automated database clean-up";


        foreach (BankRegistrationEntity bankRegistration in entityList)
        {
            // Check if software statement profile available for un-migrated registrations
            ProcessedSoftwareStatementProfile? processedSoftwareStatementProfile = null;
            string softwareStatementProfileId = bankRegistration.SoftwareStatementProfileId;
            string? softwareStatementProfileOverride = bankRegistration.SoftwareStatementProfileOverride;
            if (bankRegistration.SoftwareStatementId is null)
            {
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
                    throw new Exception(message);
                }
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
            if (processedSoftwareStatementProfile is not null &&
                string.IsNullOrEmpty(bankRegistration.DefaultQueryRedirectUri))
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

            // Prepare for removal of DbTransitionalDefault
            if (bankRegistration.BankGroup is BankGroupEnum.DbTransitionalDefault)
            {
                string message =
                    $"No suitable BankGroup could be found for BankRegistration with ID {bankRegistration.Id} " +
                    $"during database cleanup.";
                throw new Exception(message);
            }

            // Migrate software statement etc if possible
            if (processedSoftwareStatementProfile is not null)
            {
                string sReference = processedSoftwareStatementProfile.Id;
                SoftwareStatementEntity? softwareStatement =
                    sList.SingleOrDefault(x => x.Reference == sReference);
                if (softwareStatement is null)
                {
                    string obWacReference = processedSoftwareStatementProfile.TransportCertificateId;
                    ObWacCertificateEntity? obWac =
                        obwacList.SingleOrDefault(x => x.Reference == obWacReference);
                    if (obWac is null)
                    {
                        obWac = new ObWacCertificateEntity(
                            Guid.NewGuid(),
                            obWacReference,
                            false,
                            utcNow,
                            databaseUser,
                            utcNow,
                            databaseUser,
                            new SecretDescription
                            {
                                Name =
                                    $"OpenBankingConnector:TransportCertificateProfiles:{obWacReference}:AssociatedKey"
                            },
                            processedSoftwareStatementProfile.TransportCertificate);
                        await obwacList.AddAsync(obWac);
                    }
                    string obSealReference = processedSoftwareStatementProfile.SigningCertificateId;
                    ObSealCertificateEntity? obSeal =
                        obsealList.SingleOrDefault(x => x.Reference == obSealReference);
                    if (obSeal is null)
                    {
                        obSeal = new ObSealCertificateEntity(
                            Guid.NewGuid(),
                            obSealReference,
                            false,
                            utcNow,
                            databaseUser,
                            utcNow,
                            databaseUser,
                            processedSoftwareStatementProfile.OBSealKey.KeyId,
                            new SecretDescription
                            {
                                Name =
                                    $"OpenBankingConnector:SigningCertificateProfiles:{obSealReference}:AssociatedKey"
                            },
                            processedSoftwareStatementProfile.SigningCertificate);
                        await obsealList.AddAsync(obSeal);
                    }
                    softwareStatement = new SoftwareStatementEntity(
                        Guid.NewGuid(),
                        sReference,
                        false,
                        utcNow,
                        databaseUser,
                        utcNow,
                        databaseUser,
                        processedSoftwareStatementProfile.OrganisationId,
                        processedSoftwareStatementProfile.SoftwareId,
                        processedSoftwareStatementProfile.SandboxEnvironment,
                        obWac.Id,
                        obSeal.Id,
                        processedSoftwareStatementProfile.DefaultQueryRedirectUrl,
                        processedSoftwareStatementProfile.DefaultFragmentRedirectUrl);
                    await sList.AddAsync(softwareStatement);
                    await postgreSqlDbContext.SaveChangesAsync();
                }
                bankRegistration.SoftwareStatementId = softwareStatement.Id;
            }
        }
    }
}
