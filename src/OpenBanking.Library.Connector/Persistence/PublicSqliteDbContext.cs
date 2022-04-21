// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence
{
    public class PublicSqliteDbContext
    {
        private protected SqliteDbContext _dbContext;

        public PublicSqliteDbContext(SqliteConnection connection)
        {
            DbContextOptions<SqliteDbContext> dbContextOptions = new DbContextOptionsBuilder<SqliteDbContext>()
                .UseSqlite(connection)
                .Options;
            _dbContext = new SqliteDbContext(dbContextOptions);
        }

        public void EnsureDatabaseCreated() => _dbContext.Database.EnsureCreated();
    }
}
