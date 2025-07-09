// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Configuration;

internal class AuthContextConfig<TEntity>(
    bool supportsGlobalQueryFilter,
    DbProvider dbProvider,
    bool isRelationalDatabase,
    Formatting jsonFormatting)
    : BaseConfig<TEntity>(supportsGlobalQueryFilter, dbProvider, isRelationalDatabase, jsonFormatting)
    where TEntity : AuthContext
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        // Top-level property info: read-only, JSON conversion, etc
        builder.Property(e => e.Nonce)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.State)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.CodeVerifier)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        builder.Property(e => e.AppSessionId)
            .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        if (_dbProvider is DbProvider.MongoDb)
        {
            builder.HasIndex(x => x.CodeVerifier)
                .HasCreateIndexOptions(
                    new CreateIndexOptions<BsonDocument>
                    {
                        PartialFilterExpression =
                            Builders<BsonDocument>.Filter.And(
                                Builders<BsonDocument>.Filter.Exists("CodeVerifier"),
                                Builders<BsonDocument>.Filter.Type("CodeVerifier", BsonType.String))
                    });
        }
    }
}
