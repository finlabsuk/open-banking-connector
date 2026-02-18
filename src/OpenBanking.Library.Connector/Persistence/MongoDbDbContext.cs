// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Persistence;

/// <summary>
///     MongoDB DB context.
/// </summary>
/// <param name="options"></param>
/// <param name="mongoDatabase"></param>
public class MongoDbDbContext(DbContextOptions<MongoDbDbContext> options, IMongoDatabase mongoDatabase)
    : BaseDbContext(options, DbProvider.MongoDb, false, Formatting.None, mongoDatabase) { }
