// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.MongoDb;

public class ToVersion1
{
    public async Task FromVersion0(
        IMongoDatabase mongoDatabase,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
 
        var settingsCollection = mongoDatabase.GetCollection<SettingsEntity>("settings");
        var settingsFilter = Builders<SettingsEntity>.Filter.Eq(s => s.Id, SettingsEntity.SingletonId);

        var projection = Builders<SettingsEntity>.Projection.Include(s => s.SchemaVersion);
        
        var settingsDoc = await settingsCollection.Find(settingsFilter)
            .Project(projection)
            .FirstOrDefaultAsync(cancellationToken);

        // Exit if new database (no settings singleton) or migration previously performed (settings contains schemaVersion)
        if (settingsDoc == null || settingsDoc.Contains("schemaVersion"))
        {
            return;
        }

        var domesticVrpConsentCollection = mongoDatabase.GetCollection<DomesticVrpConsent>("domesticVrpConsent");
        
        var missingFieldFilter = Builders<DomesticVrpConsent>.Filter.Exists(d => d.MigratedToV4, false);

        var updateDefinition = Builders<DomesticVrpConsent>.Update
            .Set(d => d.MigratedToV4, false)
            .Set(d => d.MigratedToV4Modified, timeProvider.GetUtcNow().UtcDateTime);

        var updateResult = await domesticVrpConsentCollection.UpdateManyAsync(
            missingFieldFilter, 
            updateDefinition, 
            new UpdateOptions { IsUpsert = false },
            cancellationToken);

        instrumentationClient.Info($"Migrated {updateResult.ModifiedCount} VRP consent documents.");

        var settingsUpdate = Builders<SettingsEntity>.Update.Set(s => s.SchemaVersion, 1);
        
        await settingsCollection.UpdateOneAsync(
            settingsFilter, 
            settingsUpdate, 
            cancellationToken: cancellationToken);
        
    }
}
