// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
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
/// <param name="encryptionSettings"></param>
/// <param name="instrumentationClient"></param>
/// <param name="entityMethods"></param>
public class EncryptionKeyDescriptionMethods(
    IMemoryCache memoryCache,
    ISecretProvider secretProvider,
    EncryptionSettings encryptionSettings,
    IInstrumentationClient instrumentationClient,
    IDbReadOnlyEntityMethods<EncryptionKeyDescriptionEntity> entityMethods) : IEncryptionKeyDescription
{
    private readonly IInstrumentationClient _instrumentationClient = instrumentationClient;

    public void Set(Guid entityId, EncryptionKeyDescriptionCached encryptionKeyDescription) =>
        SetEncryptionKeyDescription(entityId, encryptionKeyDescription, memoryCache);

    public Guid? GetCurrentKeyId()
    {
        if (encryptionSettings.DisableEncryption)
        {
            return null;
        }
        if (encryptionSettings.CurrentKeyId is null)
        {
            throw new ArgumentException(
                "Configuration or key secrets warning: " +
                "No encryption key specified by CurrentEncryptionKeyId. If you haven't done so, " +
                "please create an EncryptionKeyDescription and specify its ID as CurrentEncryptionKeyId.");
        }
        return encryptionSettings.CurrentKeyId;
    }

    public async Task<byte[]> GetEncryptionKey(Guid? keyId)
    {
        if (keyId is null)
        {
            return [];
        }

        EncryptionKeyDescriptionCached encryptionKeyDescription =
            await Get(keyId.Value);

        return encryptionKeyDescription.Key;
    }

    public static void SetEncryptionKeyDescription(
        Guid entityId,
        EncryptionKeyDescriptionCached encryptionKeyDescription,
        IMemoryCache memoryCache) =>
        memoryCache.Set(
            GetCacheKey(entityId),
            encryptionKeyDescription);

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

    private static string GetCacheKey(Guid entityId) => string.Join(
        ":",
        "encryption_key_description",
        entityId);
}
