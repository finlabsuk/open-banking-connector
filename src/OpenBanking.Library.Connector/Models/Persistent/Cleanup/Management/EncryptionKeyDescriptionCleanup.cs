// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;
using EncryptionKeyDescriptionCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management.EncryptionKeyDescription;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup.Management;

public class EncryptionKeyDescriptionCleanup
{
    public async Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        ISecretProvider secretProvider,
        KeysSettings keysSettings,
        EncryptionKeyDescriptionMethods encryptionKeyDescriptionMethods,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        DbSet<EncryptionKeyDescriptionEntity> encryptionKeyDescriptions =
            postgreSqlDbContext
                .EncryptionKeyDescription;

        var createdBy = "Database cleanup";
        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        // Check current key has description record in database if encryption enabled
        if (!keysSettings.DisableEncryption)
        {
            string currentEncryptionKeyId = keysSettings.CurrentEncryptionKeyId;

            // If no current key specified, warn and continue
            if (currentEncryptionKeyId == "")
            {
                instrumentationClient.Warning(
                    "Configuration or key secrets warning: " +
                    "No encryption key specified by CurrentEncryptionKeyId.");
            }
            // If no key descriptions in database, create description from legacy settings
            else if (!encryptionKeyDescriptions.Any())
            {
                // Get current key ID and check key present
                if (!keysSettings.Encryption.TryGetValue(currentEncryptionKeyId, out EncryptionKey? encryptionKey))
                {
                    throw new ArgumentException(
                        "Configuration or key secrets error: " +
                        $"Encryption key with ID {currentEncryptionKeyId} as specified by CurrentEncryptionKeyId not found.");
                }

                // Create entity
                var entity = new EncryptionKeyDescriptionEntity(
                    Guid.NewGuid(),
                    null,
                    false,
                    utcNow,
                    createdBy,
                    utcNow,
                    createdBy,
                    new SecretDescription
                    {
                        Name = $"OpenBankingConnector:Keys:Encryption:{currentEncryptionKeyId}:Value"
                    },
                    currentEncryptionKeyId);
                encryptionKeyDescriptions.Add(entity);
            }
            // Check current key has description in database
            else
            {
                EncryptionKeyDescriptionEntity? currentKeyEntity =
                    Guid.TryParse(currentEncryptionKeyId, out Guid guidKey)
                        ? encryptionKeyDescriptions.SingleOrDefault(x => x.Id == guidKey)
                        : encryptionKeyDescriptions.SingleOrDefault(x => x.LegacyName == currentEncryptionKeyId);
                if (currentKeyEntity is null)
                {
                    throw new ArgumentException(
                        "Configuration or key secrets error: " +
                        $"EncryptionKeyDescription with ID {currentEncryptionKeyId} as specified by CurrentEncryptionKeyId not found.");
                }
            }
        }

        // Check and cache encryption keys
        foreach (EncryptionKeyDescriptionEntity encryptionKeyDescriptionEntity in encryptionKeyDescriptions)
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
            encryptionKeyDescriptionMethods.Set(encryptionKeyDescriptionEntity.Id, encryptionKeyDescription);
        }

        await postgreSqlDbContext.SaveChangesAsync(cancellationToken);
    }
}
