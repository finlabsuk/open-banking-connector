// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

/// <summary>
///     SQLite DB context.
///     Specifies JSON indenting to aid visualisation (SQLite context mainly used for debug).
/// </summary>
/// <param name="options"></param>
public class SqliteDbContext(DbContextOptions<SqliteDbContext> options)
    : BaseDbContext(options, DbProvider.Sqlite, true, Formatting.Indented) { }
