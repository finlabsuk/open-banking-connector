// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
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
            if (processedTransportCertificateProfile is not null)
            {
                memoryCache.Set(
                    ObWacCertificate.GetCacheKey(obWac.Id),
                    processedTransportCertificateProfile);
            }
        }

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
            if (processedSigningCertificateProfile is not null)
            {
                memoryCache.Set(
                    ObSealCertificate.GetCacheKey(obSeal.Id),
                    processedSigningCertificateProfile);
            }
        }

        return Task.CompletedTask;
    }
}
