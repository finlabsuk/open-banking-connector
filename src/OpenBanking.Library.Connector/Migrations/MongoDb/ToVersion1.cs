// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using MongoDB.Driver;

namespace FinnovationLabs.OpenBanking.Library.Connector.Migrations.MongoDb;

public class ToVersion1
{
    public Task FromVersion0(
        IMongoDatabase mongoDatabase,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        // Steps:
        // 1. Check database exists (exit if not)
        // 2. Check if settings singleton (row with _id "232c4049-a77a-4dbe-b740-ce6e9f4f54cf") has field schemaVersion; if so exit
        // 3. Run through each document in domesticVrpConsent collection and, where missing, add fields migratedToV4 (Boolean, false)
        // and migratedToV4Modified (ISODate, set to now or equal to field created as preferred).
        // 4. Add schemaVersion (long in C#) to settings document with value set to 1 to indicate migration completed

        return Task.CompletedTask;
    }
}
