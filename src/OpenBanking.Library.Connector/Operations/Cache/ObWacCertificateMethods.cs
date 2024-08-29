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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;

public class ObWacCertificateMethods(
    HttpClientSettings httpClientSettings,
    IMemoryCache memoryCache,
    ISecretProvider secretProvider,
    IInstrumentationClient instrumentationClient,
    TppReportingMetrics tppReportingMetrics,
    IDbReadWriteEntityMethods<ObWacCertificateEntity> entityMethods)
{
    public async Task<ObWacCertificate> GetValue(
        Guid obWacId) =>
        (await memoryCache.GetOrCreateAsync<ObWacCertificate>(
            ObWacCertificate.GetCacheKey(obWacId),
            async cacheEntry =>
            {
                ObWacCertificateEntity obWac =
                    await entityMethods
                        .DbSetNoTracking
                        .SingleOrDefaultAsync(x => x.Id == obWacId) ??
                    throw new KeyNotFoundException($"No record found for ObWacCertificate with ID {obWacId}.");

                SecretResult associatedKeyResult = await secretProvider.GetSecretAsync(obWac.AssociatedKey);
                if (!associatedKeyResult.SecretObtained)
                {
                    string fullMessage =
                        $"ObWacCertificate record with ID {obWac.Id} " +
                        $"specifies AssociatedKey with Source {obWac.AssociatedKey.Source} " +
                        $"and Name {obWac.AssociatedKey.Name} which could not be obtained. {associatedKeyResult.ErrorMessage}";
                    throw new KeyNotFoundException(fullMessage);
                }
                var obWacCertificate = ObWacCertificate.CreateInstance(
                    obWac,
                    associatedKeyResult.Secret!,
                    secretProvider,
                    httpClientSettings,
                    instrumentationClient,
                    tppReportingMetrics);

                return obWacCertificate;
            }))!;
}
