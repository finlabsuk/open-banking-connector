// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.HostedServices;

public class SoftwareStatementCleanup
{
    public Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IProcessedSoftwareStatementProfileStore processedSoftwareStatementProfileStore,
        ISecretProvider secretProvider,
        HttpClientSettings httpClientSettings,
        IMemoryCache memoryCache,
        IInstrumentationClient instrumentationClient,
        TppReportingMetrics tppReportingMetrics)
    {
        List<SoftwareStatementEntity> softwareStatementList =
            postgreSqlDbContext
                .SoftwareStatement
                .ToList();

        DbSet<ObWacCertificateEntity> obWacList =
            postgreSqlDbContext
                .ObWacCertificate;

        DbSet<ObSealCertificateEntity> obSealList =
            postgreSqlDbContext
                .ObSealCertificate;

        var obWacProfiles = new Dictionary<Guid, ObWacCertificate?>();
        foreach (ObWacCertificateEntity obWac in obWacList)
        {
            ObWacCertificate? processedTransportCertificateProfile = null;
            try
            {
                processedTransportCertificateProfile = new ObWacCertificate(
                    obWac,
                    secretProvider,
                    httpClientSettings,
                    instrumentationClient,
                    tppReportingMetrics);
            }
            catch (KeyNotFoundException ex)
            {
                instrumentationClient.Warning(ex.Message);
            }
            obWacProfiles[obWac.Id] = processedTransportCertificateProfile;
            if (processedTransportCertificateProfile is not null)
            {
                memoryCache.Set(
                    ObWacCertificate.GetCacheKey(obWac.Id),
                    processedTransportCertificateProfile);
            }
        }

        var obSealProfiles = new Dictionary<Guid, ObSealCertificate?>();
        foreach (ObSealCertificateEntity obSeal in obSealList)
        {
            ObSealCertificate? processedSigningCertificateProfile = null;
            try
            {
                processedSigningCertificateProfile = new ObSealCertificate(
                    obSeal,
                    secretProvider,
                    instrumentationClient);
            }
            catch (KeyNotFoundException ex)
            {
                instrumentationClient.Warning(ex.Message);
            }
            obSealProfiles[obSeal.Id] = processedSigningCertificateProfile;
            if (processedSigningCertificateProfile is not null)
            {
                memoryCache.Set(
                    ObSealCertificate.GetCacheKey(obSeal.Id),
                    processedSigningCertificateProfile);
            }
        }

        foreach (SoftwareStatementEntity softwareStatement in softwareStatementList)
        {
            ObWacCertificate? obWacProfile =
                obWacProfiles[softwareStatement.DefaultObWacCertificateId];
            ObSealCertificate? obSealProfile =
                obSealProfiles[softwareStatement.DefaultObSealCertificateId];
            if (obWacProfile is null ||
                obSealProfile is null)
            {
                string message =
                    $"Software statement with ID {softwareStatement.Id} " +
                    $"cannot be used due to lack of suitable OBWAC transport " +
                    $"or OBSeal signing certificate. See previous warnings for specific issues.";
                instrumentationClient.Warning(message);
                continue;
            }

            // Add software statement to cache
            var softwareStatementIdString = softwareStatement.Id.ToString(); // sets ID of profile added to store
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                processedSoftwareStatementProfileStore.GetProfile(
                    obWacProfile,
                    obSealProfile,
                    new SoftwareStatementProfile
                    {
                        Active = true,
                        OrganisationId = softwareStatement.OrganisationId,
                        SoftwareId = softwareStatement.SoftwareId,
                        SandboxEnvironment = softwareStatement.SandboxEnvironment,
                        TransportCertificateProfileId =
                            softwareStatement.DefaultObWacCertificateId.ToString(), // ignored
                        SigningCertificateProfileId =
                            softwareStatement.DefaultObSealCertificateId.ToString(), // ignored
                        DefaultQueryRedirectUrl = softwareStatement.DefaultQueryRedirectUrl,
                        DefaultFragmentRedirectUrl = softwareStatement.DefaultFragmentRedirectUrl
                    },
                    softwareStatementIdString, // sets ID of profile added to store
                    instrumentationClient);

            processedSoftwareStatementProfileStore.AddProfile(
                processedSoftwareStatementProfile,
                softwareStatementIdString);
        }
        return Task.CompletedTask;
    }
}
