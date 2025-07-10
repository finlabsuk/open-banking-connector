// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

// SQLite DB context
public class SqliteDbContext(DbContextOptions<SqliteDbContext> options) : RelationalDbContext(options)
{
    // Use indenting to aid visualisation (SQLite context mainly used for debug).
    protected override Formatting JsonFormatting => Formatting.Indented;
    protected override DbProvider DbProvider => DbProvider.Sqlite;
}
