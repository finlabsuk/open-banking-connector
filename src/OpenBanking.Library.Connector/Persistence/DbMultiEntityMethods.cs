// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    /// <summary>
    ///     Non entity- (type-) specific DB methods
    /// </summary>
    public class DbMultiEntityMethods : IDbMultiEntityMethods
    {
        private readonly BaseDbContext _db;

        public DbMultiEntityMethods(BaseDbContext db)
        {
            _db = db;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
