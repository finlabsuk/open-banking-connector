// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup;

public class EncryptedObjectCleanup
{
    public Task Cleanup(
        BaseDbContext postgreSqlDbContext,
        ISettingsService settingsService,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        DbSet<EncryptionKeyDescriptionEntity> encryptionKeyDescriptions =
            postgreSqlDbContext
                .EncryptionKeyDescription;

        DbSet<EncryptedObject> encryptedObjects =
            postgreSqlDbContext
                .EncryptedObject;

        // Warn if database contains unencrypted objects
        if (!settingsService.DisableEncryption &&
            encryptedObjects.Any(r => r.EncryptionKeyDescriptionId == null))
        {
            string fullMessage =
                "Database clean-up warning: " +
                "Encryption is enabled yet at least one EncryptedObject is not encrypted (has null EncryptionKeyDescriptionId).";
            instrumentationClient.Warning(fullMessage);
        }
        return Task.CompletedTask;
    }
}
