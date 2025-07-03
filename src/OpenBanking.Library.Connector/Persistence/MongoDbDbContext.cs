// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

public class MongoDbDbContext(DbContextOptions<MongoDbDbContext> options) : BaseDbContext(options)
{
    protected override DbProvider DbProvider => DbProvider.MongoDb;
}
