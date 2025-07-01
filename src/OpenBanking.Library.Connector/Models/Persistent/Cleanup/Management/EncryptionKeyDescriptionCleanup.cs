// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using EncryptionKeyDescriptionCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management.EncryptionKeyDescription;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.Management;

public class EncryptionKeyDescriptionCleanup
{
    public async Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        ISecretProvider secretProvider,
        KeysSettings keysSettings,
        EncryptionSettings encryptionSettings,
        IMemoryCache memoryCache,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        DbSet<EncryptionKeyDescriptionEntity> encryptionKeyDescriptions =
            postgreSqlDbContext
                .EncryptionKeyDescription;

        //var createdBy = "Database cleanup";
        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        encryptionSettings.SetDisableEncryption(keysSettings.DisableEncryption);

        // Check specified current key has description record in database if encryption enabled
        if (!keysSettings.DisableEncryption)
        {
            string currentEncryptionKeyId = keysSettings.CurrentEncryptionKeyId;

            // Check current key not empty string
            if (currentEncryptionKeyId == "")
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    "No encryption key specified by CurrentEncryptionKeyId.");
            }

            // Check current key is valid Guid
            if (!Guid.TryParse(currentEncryptionKeyId, out Guid guidKey))
            {
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    "Encryption key specified by CurrentEncryptionKeyId is not valid ID (GUID). " +
                    "It probably needs updating as legacy key names are no longer supported.");
            }

            // Check current key has description in database
            EncryptionKeyDescriptionEntity currentKeyEntity =
                encryptionKeyDescriptions.SingleOrDefault(x => x.Id == guidKey) ??
                throw new ArgumentException(
                    "Configuration or key secrets error: " +
                    $"EncryptionKeyDescription with ID {guidKey} as specified by CurrentEncryptionKeyId not found.");

            encryptionSettings.SetCurrentKeyId(guidKey);
        }

        // Check and cache encryption keys
        foreach (EncryptionKeyDescriptionEntity encryptionKeyDescriptionEntity in encryptionKeyDescriptions.Local)
        {
            // Attempt to obtain key
            SecretResult keyResult = await secretProvider.GetSecretAsync(encryptionKeyDescriptionEntity.Key);
            if (!keyResult.SecretObtained)
            {
                string fullMessage =
                    $"EncryptionKeyDescription with ID {encryptionKeyDescriptionEntity.Id} specifies Key with Source " +
                    $"{encryptionKeyDescriptionEntity.Key.Source} " +
                    $"and Name {encryptionKeyDescriptionEntity.Key.Name} which could not be obtained. {keyResult.ErrorMessage} " +
                    "Any EncryptedObject records depending " +
                    "on this EncryptionKeyDescription will not be able to be used.";
                instrumentationClient.Warning(fullMessage);
                continue;
            }

            // Attempt to create cache object
            byte[] key = Convert.FromBase64String(keyResult.Secret!);
            if (key.Length is not 32)
            {
                string message =
                    $"EncryptionKeyDescription with ID {encryptionKeyDescriptionEntity.Id} specifies Key with Source " +
                    $"{encryptionKeyDescriptionEntity.Key.Source} " +
                    $"and Name {encryptionKeyDescriptionEntity.Key.Name} which has {key.Length} bytes (require 32). " +
                    "Any EncryptedObject records depending " +
                    "on this EncryptionKeyDescription will not be able to be used.";
                instrumentationClient.Warning(message);
                continue;
            }
            var encryptionKeyDescription = new EncryptionKeyDescriptionCached { Key = key };

            // Add cache entry
            EncryptionKeyDescriptionMethods.SetEncryptionKeyDescription(
                encryptionKeyDescriptionEntity.Id,
                encryptionKeyDescription,
                memoryCache);
        }

        await postgreSqlDbContext.SaveChangesAsync(cancellationToken);
    }
}
