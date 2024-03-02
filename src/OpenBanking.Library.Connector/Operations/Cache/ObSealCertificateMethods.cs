// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;

public class ObSealCertificateMethods(
    IMemoryCache memoryCache,
    ISecretProvider secretProvider,
    IInstrumentationClient instrumentationClient,
    IDbReadWriteEntityMethods<ObSealCertificateEntity> entityMethods)
{
    public async Task<ObSealCertificate> GetValue(
        Guid obSealId)
    {
        string obSealCacheId = ObSealCertificate.GetCacheKey(obSealId);
        var processedSigningCertificateProfile =
            (await memoryCache.GetOrCreateAsync<ObSealCertificate>(
                obSealCacheId,
                async cacheEntry =>
                {
                    ObSealCertificateEntity obSeal =
                        await entityMethods
                            .DbSetNoTracking
                            .SingleOrDefaultAsync(x => x.Id == obSealId) ??
                        throw new KeyNotFoundException($"No record found for ObSealCertificate with ID {obSealId}.");
                    return new ObSealCertificate(obSeal, secretProvider, instrumentationClient);
                }))!;
        return processedSigningCertificateProfile;
    }
}
