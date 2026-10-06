// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup;

public class SettingsCleanup
{
    public async Task Cleanup(
        BaseDbContext dbContext,
        ISettingsService settingsService,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        DbSet<SettingsEntity> settingsDbSet = dbContext.Settings;

        IQueryable<EncryptionKeyDescriptionEntity> encryptionKeyDescriptionDbSet =
            new DbEntityMethods<EncryptionKeyDescriptionEntity>(dbContext).DbSetNoTracking;

        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        // Get singleton settings record (or create if it doesn't exist).
        SettingsEntity? settings =
            await settingsDbSet.SingleOrDefaultAsync(x => x.Id == SettingsEntity.SingletonId, cancellationToken);
        if (settings is null)
        {
            settings = new SettingsEntity(
                SettingsEntity.SingletonId,
                null,
                false,
                utcNow,
                utcNow,
                SettingsEntity
                    .CurrentSchemaVersion);
            await settingsDbSet.AddAsync(settings, cancellationToken);
        }

        // Encryption-related checks (only perform when encryption not disabled)
        if (!settings.DisableEncryption)
        {
            // Warn if no current encryption key specified.
            // Note: attempts to write encrypted objects will fail until one is specified.
            if (settings.CurrentEncryptionKeyDescriptionId is null)
            {
                instrumentationClient.Warning(
                    "Settings warning: " +
                    "No CurrentEncryptionKeyDescription ID specified in database settings table " +
                    "and encryption not disabled. Objects requiring encryption cannot be created until an " +
                    "EncryptionKeyDescription is created with SetAsCurrentEncryptionKey = true.");
            }

            // Check CurrentEncryptionKeyDescription ID is valid
            else
            {
                EncryptionKeyDescriptionEntity _ =
                    encryptionKeyDescriptionDbSet.SingleOrDefault(
                        x => x.Id == settings.CurrentEncryptionKeyDescriptionId) ??
                    throw new InvalidOperationException(
                        "Settings error: " +
                        $"EncryptionKeyDescription with ID {settings.CurrentEncryptionKeyDescriptionId} " +
                        "specified in CurrentEncryptionKeyDescriptionId in database settings table not found.");
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Initialise settings service (cache) according to database settings record
        settingsService.DisableEncryption = settings.DisableEncryption;
        settingsService.CurrentEncryptionKeyId = settings.CurrentEncryptionKeyDescriptionId;
    }
}
