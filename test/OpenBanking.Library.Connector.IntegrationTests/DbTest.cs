// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests;

// Makes available in-memory EF Core database
public abstract class DbTest : IDisposable
{
    protected readonly BaseDbContext _dB;

    protected DbTest()
    {
        DbContextOptions<PostgreSqlDbContext> dbContextOptions =
            new DbContextOptionsBuilder<PostgreSqlDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        _dB = new PostgreSqlDbContext(dbContextOptions);
        _dB.Database.EnsureCreated(); // Initialise DB with schema
    }

    public void Dispose()
    {
        _dB.Dispose();
    }
}
