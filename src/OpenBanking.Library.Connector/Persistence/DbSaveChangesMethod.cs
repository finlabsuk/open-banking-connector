// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

/// <summary>
///     Non entity- (type-) specific DB methods
/// </summary>
public class DbSaveChangesMethod : IDbSaveChangesMethod
{
    private readonly BaseDbContext _db;

    public DbSaveChangesMethod(BaseDbContext db)
    {
        _db = db;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _db.SaveChangesAsync();
    }
}
