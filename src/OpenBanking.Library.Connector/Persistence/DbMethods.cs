// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

/// <summary>
///     Non-entity-specific database methods.
/// </summary>
public class DbMethods : IDbMethods
{
    private readonly BaseDbContext _db;

    public DbMethods(BaseDbContext db)
    {
        _db = db;
    }

    public DbProvider DbProvider => _db.DbProvider;

    public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
}
