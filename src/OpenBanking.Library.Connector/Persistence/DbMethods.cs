// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

/// <summary>
///     Non-entity-specific database methods.
/// </summary>
public class DbMethods : IDbMethods, IDbSettingsMethods
{
    private readonly BaseDbContext _db;

    public DbMethods(BaseDbContext db)
    {
        _db = db;
    }

    public DbProvider DbProvider => _db.DbProvider;

    public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();

    async ValueTask<SettingsEntity> IDbSettingsMethods.GetSettingsAsync() =>
        await _db.Settings
            .FindAsync(SettingsEntity.SingletonId) ??
        throw new InvalidOperationException("Cannot find settings record in database.");

    async ValueTask<SettingsEntity> IDbSettingsReadOnlyMethods.GetSettingsNoTrackingAsync() =>
        await _db.Settings
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == SettingsEntity.SingletonId) ??
        throw new InvalidOperationException("Cannot find settings record in database.");
}
