// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost;

public static class MongoDbDriverConfiguration
{
    static MongoDbDriverConfiguration()
    {
        // Apply conventions for MongoDB driver
        ConventionRegistry.Register(
            "CamelCaseElementNames",
            new ConventionPack { new CamelCaseElementNameConvention() },
            _ => true);
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.DateTime));
    }

    public static void EnsureConstructorHasRun() { }
}
