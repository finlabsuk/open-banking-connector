// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;

public static class ObWacCertificate
{
    public static async Task<ProcessedTransportCertificateProfile> GetValue(
        Guid obWacId,
        HttpClientSettings httpClientSettings,
        IInstrumentationClient instrumentationClient,
        ISecretProvider secretProvider,
        IDbReadWriteEntityMethods<ObWacCertificateEntity> dbReadWriteEntityMethods,
        IMemoryCache memoryCache)
    {
        string obWacCacheId = ProcessedTransportCertificateProfile.GetCacheKey(obWacId);
        var processedTransportCertificateProfile =
            (await memoryCache.GetOrCreateAsync<ProcessedTransportCertificateProfile>(
                obWacCacheId,
                async cacheEntry =>
                {
                    ObWacCertificateEntity obWac =
                        await dbReadWriteEntityMethods
                            .DbSetNoTracking
                            .SingleOrDefaultAsync(x => x.Id == obWacId) ??
                        throw new KeyNotFoundException($"No record found for ObWacCertificate with ID {obWacId}.");
                    return ProcessedTransportCertificateProfile.GetProcessedObWac(
                        secretProvider,
                        httpClientSettings,
                        instrumentationClient,
                        obWac);
                }))!;
        return processedTransportCertificateProfile;
    }
}
