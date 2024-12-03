// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using EncryptionKeyDescriptionCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management.EncryptionKeyDescription;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;

/// <summary>
///     EncryptionKeyDescription interactions with cache (handles caching details).
/// </summary>
/// <param name="memoryCache"></param>
/// <param name="secretProvider"></param>
/// <param name="instrumentationClient"></param>
/// <param name="entityMethods"></param>
public class EncryptionKeyDescriptionMethods(
    IMemoryCache memoryCache,
    ISecretProvider secretProvider,
    IInstrumentationClient instrumentationClient,
    IDbReadOnlyEntityMethods<EncryptionKeyDescriptionEntity> entityMethods)
{
    private readonly IInstrumentationClient _instrumentationClient = instrumentationClient;

    public async Task<EncryptionKeyDescriptionCached> Get(
        Guid entityId) =>
        (await memoryCache.GetOrCreateAsync<EncryptionKeyDescriptionCached>(
            GetCacheKey(entityId),
            async cacheEntry =>
            {
                EncryptionKeyDescriptionEntity encryptionKeyDescriptionEntity =
                    await entityMethods
                        .DbSetNoTracking
                        .SingleOrDefaultAsync(x => x.Id == entityId) ??
                    throw new KeyNotFoundException($"No record found for EncryptionKeyDescription with ID {entityId}.");

                SecretResult keyResult = await secretProvider.GetSecretAsync(encryptionKeyDescriptionEntity.Key);
                if (!keyResult.SecretObtained)
                {
                    string fullMessage =
                        $"EncryptionKeyDescription record with ID {encryptionKeyDescriptionEntity.Id} " +
                        $"specifies Key with Source {encryptionKeyDescriptionEntity.Key.Source} " +
                        $"and Name {encryptionKeyDescriptionEntity.Key.Name} which could not be obtained. {keyResult.ErrorMessage}";
                    throw new KeyNotFoundException(fullMessage);
                }
                byte[] key = Convert.FromBase64String(keyResult.Secret!);
                if (key.Length is not 32)
                {
                    throw new ArgumentException(
                        $"EncryptionKeyDescription record with ID {encryptionKeyDescriptionEntity.Id} " +
                        $"specifies Key which has {key.Length} bytes (require 32).");
                }
                var encryptionKeyDescription = new EncryptionKeyDescriptionCached { Key = key };

                return encryptionKeyDescription;
            }))!;

    public void Set(Guid entityId, EncryptionKeyDescriptionCached encryptionKeyDescription)
    {
        memoryCache.Set(
            GetCacheKey(entityId),
            encryptionKeyDescription);
    }

    private static string GetCacheKey(Guid entityId) => string.Join(
        ":",
        "encryption_key_description",
        entityId);
}
