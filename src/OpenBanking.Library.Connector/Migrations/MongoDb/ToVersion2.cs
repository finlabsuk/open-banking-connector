// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.AccountAndTransaction;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.MongoDb;

public class ToVersion2
{
    public async Task FromVersion1(
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

        // Exit if new database (no settings singleton) or migration previously performed (schemaVersion not 1)
        if (settingsDoc == null || !settingsDoc.Contains("schemaVersion") || settingsDoc["schemaVersion"].ToInt64() != 1)
        {
            return;
        }

        // Backfill redirectUri (new required field) on existing auth context documents
        var authContextCollection = mongoDatabase.GetCollection<AuthContext>("authContext");

        var missingRedirectUriFilter = Builders<AuthContext>.Filter.Exists(a => a.RedirectUri, false);

        var authContextUpdateDefinition = Builders<AuthContext>.Update
            .Set(a => a.RedirectUri, "");

        var authContextUpdateResult = await authContextCollection.UpdateManyAsync(
            missingRedirectUriFilter,
            authContextUpdateDefinition,
            new UpdateOptions { IsUpsert = false },
            cancellationToken);

        instrumentationClient.Info($"Migrated {authContextUpdateResult.ModifiedCount} auth context documents.");

        // Rename software statement redirect URI fields
        // (defaultQueryRedirectUrl -> defaultQueryRedirectUri, defaultFragmentRedirectUrl -> defaultFragmentRedirectUri)
        var softwareStatementCollection = mongoDatabase.GetCollection<BsonDocument>("softwareStatement");

        var oldRedirectUrlFieldsFilter = Builders<BsonDocument>.Filter.Or(
            Builders<BsonDocument>.Filter.Exists("defaultQueryRedirectUrl"),
            Builders<BsonDocument>.Filter.Exists("defaultFragmentRedirectUrl"));

        var softwareStatementUpdateDefinition = Builders<BsonDocument>.Update
            .Rename("defaultQueryRedirectUrl", "defaultQueryRedirectUri")
            .Rename("defaultFragmentRedirectUrl", "defaultFragmentRedirectUri");

        var softwareStatementUpdateResult = await softwareStatementCollection.UpdateManyAsync(
            oldRedirectUrlFieldsFilter,
            softwareStatementUpdateDefinition,
            new UpdateOptions { IsUpsert = false },
            cancellationToken);

        instrumentationClient.Info(
            $"Migrated {softwareStatementUpdateResult.ModifiedCount} software statement documents.");

        // Backfill authContextAcr/authContextAuthTime (new optional fields, stored as explicit nulls) on existing
        // consent documents
        long consentModifiedCount = 0;
        consentModifiedCount += await BackfillAuthContextAcrAndAuthTime<AccountAccessConsent>(
            mongoDatabase,
            "accountAccessConsent",
            cancellationToken);
        consentModifiedCount += await BackfillAuthContextAcrAndAuthTime<DomesticPaymentConsent>(
            mongoDatabase,
            "domesticPaymentConsent",
            cancellationToken);
        consentModifiedCount += await BackfillAuthContextAcrAndAuthTime<DomesticVrpConsent>(
            mongoDatabase,
            "domesticVrpConsent",
            cancellationToken);

        instrumentationClient.Info($"Migrated {consentModifiedCount} consent documents.");

        var settingsUpdate = Builders<SettingsEntity>.Update.Set(s => s.SchemaVersion, 2);

        await settingsCollection.UpdateOneAsync(
            settingsFilter,
            settingsUpdate,
            cancellationToken: cancellationToken);
    }

    private static async Task<long> BackfillAuthContextAcrAndAuthTime<TConsent>(
        IMongoDatabase mongoDatabase,
        string collectionName,
        CancellationToken cancellationToken)
        where TConsent : BaseConsent
    {
        var consentCollection = mongoDatabase.GetCollection<TConsent>(collectionName);

        var missingFieldFilter = Builders<TConsent>.Filter.Or(
            Builders<TConsent>.Filter.Exists(c => c.AuthContextAcr, false),
            Builders<TConsent>.Filter.Exists(c => c.AuthContextAuthTime, false));

        var updateDefinition = Builders<TConsent>.Update
            .Set(c => c.AuthContextAcr, null)
            .Set(c => c.AuthContextAuthTime, null);

        var updateResult = await consentCollection.UpdateManyAsync(
            missingFieldFilter,
            updateDefinition,
            new UpdateOptions { IsUpsert = false },
            cancellationToken);

        return updateResult.ModifiedCount;
    }
}
