// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests;

// Makes available in-memory SQLite database
public abstract class DbTest : IDisposable
{
    protected readonly SqliteDbContext _dB;
    private readonly SqliteConnection _dBConnection;

    protected DbTest()
    {
        _dBConnection = new SqliteConnection("DataSource=:memory:");
        _dBConnection.Open(); // Creates DB
        DbContextOptions<SqliteDbContext> dbContextOptions = new DbContextOptionsBuilder<SqliteDbContext>()
            .UseSqlite(_dBConnection)
            .Options;
        _dB = new SqliteDbContext(dbContextOptions);
        _dB.Database.EnsureCreated(); // Initialise DB with schema
    }

    public void Dispose()
    {
        _dBConnection.Close(); // Deletes DB
    }
}
