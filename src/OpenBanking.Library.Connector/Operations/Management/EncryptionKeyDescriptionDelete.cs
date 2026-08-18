// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

/// <summary>
///     Deletes an EncryptionKeyDescription. If it is the current encryption key, also clears
///     Settings.CurrentEncryptionKeyDescriptionId and the ISettingsService cache, so Settings never references a
///     deleted key.
/// </summary>
internal class EncryptionKeyDescriptionDelete : IObjectDelete<LocalDeleteParams>
{
    private readonly IDbMethods _dbSaveChangesMethod;
    private readonly IDbEntityMethods<EncryptionKeyDescriptionEntity> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbSettingsMethods _settingsMethods;
    private readonly ISettingsService _settingsService;
    private readonly ITimeProvider _timeProvider;

    public EncryptionKeyDescriptionDelete(
        IDbEntityMethods<EncryptionKeyDescriptionEntity> entityMethods,
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        IDbSettingsMethods settingsMethods,
        ISettingsService settingsService)
    {
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _settingsMethods = settingsMethods;
        _settingsService = settingsService;
    }

    public async Task<IList<IFluentResponseInfoOrWarningMessage>> DeleteAsync(LocalDeleteParams deleteParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get persisted entity
        EncryptionKeyDescriptionEntity persistedObject =
            await _entityMethods
                .DbSet
                .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
            throw new KeyNotFoundException($"No record found for entity with ID {deleteParams.Id}.");

        DateTimeOffset utcNow = _timeProvider.GetUtcNow();

        // Local soft delete
        persistedObject.UpdateIsDeleted(true, utcNow, deleteParams.ModifiedBy);

        // If deleting the current encryption key, clear the Settings pointer so it does not dangle-reference
        // a soft-deleted key.
        SettingsEntity settings = await _settingsMethods.GetSettingsAsync();
        if (settings.CurrentEncryptionKeyDescriptionId == deleteParams.Id)
        {
            settings.ClearCurrentEncryptionKey(utcNow);
            nonErrorMessages.Add(
                FluentResponseMessage.Warning(
                    "The deleted EncryptionKeyDescription was the current encryption key. " +
                    "Settings.CurrentEncryptionKeyDescriptionId has been cleared. A new current encryption key " +
                    "must be set (e.g. create a new EncryptionKeyDescription with SetAsCurrentEncryptionKey = " +
                    "true) before further objects requiring encryption can be created."));
        }

        await _dbSaveChangesMethod.SaveChangesAsync();

        // Update in-memory cache after the DB save has committed
        if (_settingsService.CurrentEncryptionKeyId == persistedObject.Id)
        {
            _settingsService.CurrentEncryptionKeyId = null;
        }

        // Return success response (thrown exceptions produce error response)
        return nonErrorMessages;
    }
}
