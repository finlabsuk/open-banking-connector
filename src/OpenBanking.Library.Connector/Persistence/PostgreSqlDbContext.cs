// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

/// <summary>
///     PostgreSQL DB context.
/// </summary>
/// <param name="options"></param>
public class PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options)
    : BaseDbContext(options, DbProvider.PostgreSql, true) { }
