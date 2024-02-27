// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;

public static class ObSealCertificate
{
    public static async Task<ProcessedSigningCertificateProfile> GetValue(
        Guid obSealId,
        IInstrumentationClient instrumentationClient,
        ISecretProvider secretProvider,
        IDbReadWriteEntityMethods<ObSealCertificateEntity> dbReadWriteEntityMethods,
        IMemoryCache memoryCache)
    {
        string obSealCacheId = ProcessedSigningCertificateProfile.GetCacheKey(obSealId);
        var processedSigningCertificateProfile =
            (await memoryCache.GetOrCreateAsync<ProcessedSigningCertificateProfile>(
                obSealCacheId,
                async cacheEntry =>
                {
                    ObSealCertificateEntity obSeal =
                        await dbReadWriteEntityMethods
                            .DbSetNoTracking
                            .SingleOrDefaultAsync(x => x.Id == obSealId) ??
                        throw new KeyNotFoundException($"No record found for ObSealCertificate with ID {obSealId}.");
                    return ProcessedSigningCertificateProfile.GetProcessedObSeal(
                        secretProvider,
                        instrumentationClient,
                        obSeal);
                }))!;
        return processedSigningCertificateProfile;
    }
}
