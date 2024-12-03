// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.Cleanup;

public class EncryptedObjectCleanup
{
    public async Task Cleanup(
        PostgreSqlDbContext postgreSqlDbContext,
        IInstrumentationClient instrumentationClient,
        ITimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        DbSet<EncryptionKeyDescriptionEntity> encryptionKeyDescriptions =
            postgreSqlDbContext
                .EncryptionKeyDescription;

        DbSet<EncryptedObject> encryptedObjects =
            postgreSqlDbContext
                .EncryptedObject;

        // Check encrypted objects
        foreach (EncryptedObject encryptedObject in encryptedObjects)
        {
            if (encryptedObject.KeyId is not null &&
                encryptedObject.EncryptionKeyDescriptionId is null)
            {
                EncryptionKeyDescriptionEntity keyEntity =
                    encryptionKeyDescriptions.SingleOrDefault(x => x.LegacyName == encryptedObject.KeyId) ??
                    throw new ArgumentException(
                        "Database clean-up error: " +
                        $"EncryptionKeyDescription with legacy name {encryptedObject.KeyId} specified by EncryptedObject with ID {encryptedObject.Id} not found.");
                encryptedObject.EncryptionKeyDescriptionId = keyEntity.Id;
            }
        }

        await postgreSqlDbContext.SaveChangesAsync(cancellationToken);
    }
}
