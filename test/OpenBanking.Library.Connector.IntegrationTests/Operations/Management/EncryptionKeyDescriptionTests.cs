// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Operations.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Xunit;
using TimeProvider = FinnovationLabs.OpenBanking.Library.Connector.Services.TimeProvider;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Operations.Management;

public class EncryptionKeyDescriptionTests : DbTest
{
    private readonly IDbMethods _dbSaveChangesMethod;
    private readonly IDbEntityMethods<EncryptionKeyDescriptionEntity> _entityMethods;
    private readonly IInstrumentationClient _instrumentationClient;
    private readonly IDbSettingsMethods _settingsMethods;
    private readonly ITimeProvider _timeProvider;

    public EncryptionKeyDescriptionTests()
    {
        _entityMethods = new DbEntityMethods<EncryptionKeyDescriptionEntity>(_dB);
        _dbSaveChangesMethod = new DbMethods(_dB);
        _settingsMethods = new DbMethods(_dB);
        _timeProvider = new TimeProvider();
        _instrumentationClient = new ConsoleInstrumentationClient(new StringWriter());
    }

    private EncryptionKeyDescriptionDelete GetEncryptionKeyDescriptionDelete(ISettingsService settingsService) =>
        new(
            _entityMethods,
            _dbSaveChangesMethod,
            _timeProvider,
            _instrumentationClient,
            _settingsMethods,
            settingsService);

    private async Task<EncryptionKeyDescriptionEntity> AddKeyAsync(string reference)
    {
        DateTimeOffset now = _timeProvider.GetUtcNow();
        var entity = new EncryptionKeyDescriptionEntity(
            Guid.NewGuid(),
            reference,
            false,
            now,
            null,
            now,
            "test",
            new SecretDescription
            {
                Name = "test-key",
                Source = SecretSource.Configuration
            });
        await _entityMethods.AddAsync(entity);
        await _dbSaveChangesMethod.SaveChangesAsync();
        return entity;
    }

    private async Task SeedSettingsAsync(Guid? currentEncryptionKeyDescriptionId)
    {
        DateTimeOffset now = _timeProvider.GetUtcNow();
        var settings = new SettingsEntity(
            SettingsEntity.SingletonId,
            currentEncryptionKeyDescriptionId,
            false,
            now,
            now,
            SettingsEntity.CurrentSchemaVersion);
        _dB.Settings.Add(settings);
        await _dbSaveChangesMethod.SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_NonCurrentKey_NoSettingsChange()
    {
        EncryptionKeyDescriptionEntity current = await AddKeyAsync("current");
        EncryptionKeyDescriptionEntity other = await AddKeyAsync("other");
        await SeedSettingsAsync(current.Id);
        var settingsService = new SettingsService { CurrentEncryptionKeyId = current.Id };
        EncryptionKeyDescriptionDelete sut = GetEncryptionKeyDescriptionDelete(settingsService);

        IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages = await sut.DeleteAsync(
            new LocalDeleteParams
            {
                Id = other.Id,
                ModifiedBy = "test"
            });

        Assert.Empty(nonErrorMessages);
        Assert.True(other.IsDeleted);
        Assert.Equal(current.Id, settingsService.CurrentEncryptionKeyId);
        SettingsEntity settings = await _settingsMethods.GetSettingsAsync();
        Assert.Equal(current.Id, settings.CurrentEncryptionKeyDescriptionId);
    }

    [Fact]
    public async Task DeleteAsync_CurrentKey_ClearsSettings()
    {
        EncryptionKeyDescriptionEntity current = await AddKeyAsync("current");
        await SeedSettingsAsync(current.Id);
        var settingsService = new SettingsService { CurrentEncryptionKeyId = current.Id };
        EncryptionKeyDescriptionDelete sut = GetEncryptionKeyDescriptionDelete(settingsService);

        IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages = await sut.DeleteAsync(
            new LocalDeleteParams
            {
                Id = current.Id,
                ModifiedBy = "test"
            });

        Assert.Single(nonErrorMessages);
        Assert.True(current.IsDeleted);
        Assert.Null(settingsService.CurrentEncryptionKeyId);
        SettingsEntity settings = await _settingsMethods.GetSettingsAsync();
        Assert.Null(settings.CurrentEncryptionKeyDescriptionId);
    }

    [Fact]
    public async Task DeleteAsync_UnknownId_Throws()
    {
        await SeedSettingsAsync(null);
        var settingsService = new SettingsService();
        EncryptionKeyDescriptionDelete sut = GetEncryptionKeyDescriptionDelete(settingsService);

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => sut.DeleteAsync(
                new LocalDeleteParams
                {
                    Id = Guid.NewGuid(),
                    ModifiedBy = "test"
                }));
    }
}
