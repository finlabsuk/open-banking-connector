// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
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
    IDbReadWriteEntityMethods<ObWacCertificateEntity> entityMethods)
{
    public async Task<ObWacCertificate> GetValue(
        Guid obWacId)
    {
        string obWacCacheId = ObWacCertificate.GetCacheKey(obWacId);
        var processedTransportCertificateProfile =
            (await memoryCache.GetOrCreateAsync<ObWacCertificate>(
                obWacCacheId,
                async cacheEntry =>
                {
                    ObWacCertificateEntity obWac =
                        await entityMethods
                            .DbSetNoTracking
                            .SingleOrDefaultAsync(x => x.Id == obWacId) ??
                        throw new KeyNotFoundException($"No record found for ObWacCertificate with ID {obWacId}.");
                    return ObWacCertificate.GetProcessedObWac(
                        secretProvider,
                        httpClientSettings,
                        instrumentationClient,
                        obWac);
                }))!;
        return processedTransportCertificateProfile;
    }
}
