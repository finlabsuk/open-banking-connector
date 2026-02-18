// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Cache;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using EncryptionKeyDescriptionCached =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Cache.Management.EncryptionKeyDescription;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;

internal class EncryptionKeyDescriptionPost :
    IObjectCreate<EncryptionKeyDescription, EncryptionKeyDescriptionResponse, LocalCreateParams>
{
    private readonly IDbMethods _dbSaveChangesMethod;
    private readonly IEncryptionKeyDescription _encryptionKeyDescriptionMethods;
    private readonly IDbEntityMethods<EncryptionKeyDescriptionEntity> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly ISecretProvider _secretProvider;
    private readonly IDbSettingsMethods _settingsMethods;
    private readonly ISettingsService _settingsService;
    private readonly ITimeProvider _timeProvider;

    public EncryptionKeyDescriptionPost(
        IDbEntityMethods<EncryptionKeyDescriptionEntity> entityMethods,
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient,
        ISecretProvider secretProvider,
        IEncryptionKeyDescription encryptionKeyDescriptionMethods,
        IDbSettingsMethods settingsMethods,
        ISettingsService settingsService)
    {
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
        _secretProvider = secretProvider;
        _encryptionKeyDescriptionMethods = encryptionKeyDescriptionMethods;
        _settingsMethods = settingsMethods;
        _settingsService = settingsService;
    }

    public async Task<(EncryptionKeyDescriptionResponse response, IList<IFluentResponseInfoOrWarningMessage>
        nonErrorMessages)> CreateAsync(
        EncryptionKeyDescription request,
        LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create entity
        DateTimeOffset utcNow = _timeProvider.GetUtcNow();
        var entity = new EncryptionKeyDescriptionEntity(
            Guid.NewGuid(),
            request.Reference,
            false,
            utcNow,
            request.CreatedBy,
            utcNow,
            request.CreatedBy,
            request.Key);

        // Try to create cache object
        SecretResult keyResult = await _secretProvider.GetSecretAsync(entity.Key);
        if (!keyResult.SecretObtained)
        {
            string fullMessage =
                $"Request specifies Key with Source {entity.Key.Source} " +
                $"and Name {entity.Key.Name} which could not be obtained. {keyResult.ErrorMessage} " +
                "EncryptionKeyDescription record will therefore not be created.";
            throw new KeyNotFoundException(fullMessage);
        }
        byte[] key = Convert.FromBase64String(keyResult.Secret!);
        if (key.Length is not 32)
        {
            throw new ArgumentException(
                $"Request specifies Key which has {key.Length} bytes (require 32). " +
                $"EncryptionKeyDescription record will therefore not be created.");
        }
        var encryptionKeyDescription = new EncryptionKeyDescriptionCached { Key = key };

        // Add entity
        await _entityMethods.AddAsync(entity);

        // Update current encryption key if required
        if (request.SetAsCurrentEncryptionKey)
        {
            SettingsEntity settings = await _settingsMethods.GetSettingsAsync();
            settings.UpdateCurrentEncryptionKey(entity.Id, utcNow);
        }

        // Persist database changes
        await _dbSaveChangesMethod.SaveChangesAsync();

        // Add cache entry
        _encryptionKeyDescriptionMethods.Set(entity.Id, encryptionKeyDescription);

        // Update current encryption key in settings cache
        if (request.SetAsCurrentEncryptionKey)
        {
            _settingsService.CurrentEncryptionKeyId = entity.Id;
        }

        // Create response
        EncryptionKeyDescriptionResponse response = entity.PublicGetLocalResponse;

        return (response, nonErrorMessages);
    }
}
