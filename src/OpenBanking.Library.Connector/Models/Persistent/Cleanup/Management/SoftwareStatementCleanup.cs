// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.Management;

public class SoftwareStatementCleanup
{
    public async Task Cleanup(
        BaseDbContext postgreSqlDbContext,
        ISecretProvider secretProvider,
        HttpClientSettings httpClientSettings,
        IMemoryCache memoryCache,
        IInstrumentationClient instrumentationClient,
        TppReportingMetrics tppReportingMetrics)
    {
        DbSet<ObWacCertificateEntity> obWacList =
            postgreSqlDbContext
                .ObWacCertificate;

        DbSet<ObSealCertificateEntity> obSealList =
            postgreSqlDbContext
                .ObSealCertificate;

        foreach (ObWacCertificateEntity obWac in obWacList)
        {
            SecretResult associatedKeyResult = await secretProvider.GetSecretAsync(obWac.AssociatedKey);
            if (!associatedKeyResult.SecretObtained)
            {
                string fullMessage =
                    $"ObWacCertificate record with ID {obWac.Id} " +
                    $"specifies AssociatedKey with Source {obWac.AssociatedKey.Source} " +
                    $"and Name {obWac.AssociatedKey.Name} which could not be obtained. {associatedKeyResult.ErrorMessage} " +
                    "Any SoftwareStatement records depending " +
                    "on this ObWacCertificate will not be able to be used.";
                instrumentationClient.Warning(fullMessage);
                continue;
            }
            try
            {
                var obWacCertificate = ObWacCertificate.CreateInstance(
                    obWac,
                    associatedKeyResult.Secret!,
                    secretProvider,
                    httpClientSettings,
                    instrumentationClient,
                    tppReportingMetrics);
                memoryCache.Set(
                    ObWacCertificate.GetCacheKey(obWac.Id),
                    obWacCertificate);
            }
            catch (CryptographicException ex)
            {
                string fullMessage =
                    $"ObWacCertificate record with ID {obWac.Id} " +
                    $"specifies AssociatedKey with Source {obWac.AssociatedKey.Source} " +
                    $"and Name {obWac.AssociatedKey.Name} which caused a cryptographic exception. {ex.Message} " +
                    "Any SoftwareStatement records depending " +
                    "on this ObWacCertificate will not be able to be used.";
                instrumentationClient.Warning(fullMessage);
            }
        }

        foreach (ObSealCertificateEntity obSeal in obSealList)
        {
            SecretResult associatedKeyResult = await secretProvider.GetSecretAsync(obSeal.AssociatedKey);
            if (!associatedKeyResult.SecretObtained)
            {
                string fullMessage =
                    $"ObSealCertificate record with ID {obSeal.Id} " +
                    $"specifies AssociatedKey with Source {obSeal.AssociatedKey.Source} " +
                    $"and Name {obSeal.AssociatedKey.Name} which could not be obtained. {associatedKeyResult.ErrorMessage} " +
                    "Any SoftwareStatement records depending " +
                    "on this ObSealCertificate will not be able to be used.";
                instrumentationClient.Warning(fullMessage);
                continue;
            }
            var obSealCertificate = ObSealCertificate.CreateInstance(
                obSeal,
                associatedKeyResult.Secret!,
                instrumentationClient);
            memoryCache.Set(
                ObSealCertificate.GetCacheKey(obSeal.Id),
                obSealCertificate);
        }
    }
}
