// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
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
        BaseDbContext postgreSqlDbContext,
        ISecretProvider secretProvider,
        IMemoryCache memoryCache,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        DbSet<EncryptionKeyDescriptionEntity> encryptionKeyDescriptions =
            postgreSqlDbContext
                .EncryptionKeyDescription;

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
