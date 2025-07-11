// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup;

public class SettingsCleanup
{
    public async Task Cleanup(
        BaseDbContext dbContext,
        KeysSettings keysSettings,
        EncryptionSettings encryptionSettings,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        DbSet<SettingsEntity> settingsDbSet = dbContext.Settings;

        IQueryable<EncryptionKeyDescriptionEntity> encryptionKeyDescriptionDbSet =
            new DbEntityMethods<EncryptionKeyDescriptionEntity>(dbContext).DbSetNoTracking;

        DateTimeOffset utcNow = timeProvider.GetUtcNow();

        encryptionSettings.SetDisableEncryption(keysSettings.DisableEncryption);

        // Get singleton settings record (or create if it doesn't exist).
        SettingsEntity? settings =
            await settingsDbSet.SingleOrDefaultAsync(cancellationToken);
        if (settings is null)
        {
            settings = new SettingsEntity(SettingsEntity.SingletonId, null, utcNow, utcNow);
            await settingsDbSet.AddAsync(settings, cancellationToken);
        }

        // Encryption-related checks (only perform when encryption not disabled)
        if (!keysSettings.DisableEncryption)
        {
            // Different checks depending on whether encryption key descriptions are available
            if (encryptionKeyDescriptionDbSet.Any())
            {
                // Check when encryption key descriptions have been specified that one is used.
                // Note: if no encryption key descriptions are specified, attempts to write encrypted objects will still fail
                // if encryption not disabled.
                if (settings.CurrentEncryptionKeyDescriptionId is null)
                {
                    // Automatically populate value if possible
                    EncryptionKeyDescriptionEntity singleValue =
                        encryptionKeyDescriptionDbSet.Single()
                        ?? throw new InvalidOperationException(
                            "Settings error: " + "No CurrentEncryptionKeyDescription ID specified in database " +
                            "settings table (encryption is not disabled and more than one EncryptionKeyDescription exists).");
                    settings.UpdateCurrentEncryptionKeyDescription(singleValue.Id, utcNow);
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
                            "specified in database settings table not found.");
                }

                // Previous checks have ensured CurrentEncryptionKeyDescription exists or have thrown
                encryptionSettings.SetCurrentKeyId(settings.CurrentEncryptionKeyDescriptionId!.Value);
            }
            else
            {
                if (settings.CurrentEncryptionKeyDescriptionId is not null)
                {
                    throw new InvalidOperationException(
                        "Settings error: " +
                        $"EncryptionKeyDescription with ID {settings.CurrentEncryptionKeyDescriptionId} " +
                        "specified in database settings table not found.");
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
