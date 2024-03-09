// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class LocalEntityDelete<TEntity, TDeleteParams> : BaseDelete<TEntity, TDeleteParams>
    where TEntity : class, IEntity
    where TDeleteParams : LocalDeleteParams
{
    /// <summary>
    ///     Delete operations on local entities (objects stored in local database only).
    /// </summary>
    /// <param name="entityMethods"></param>
    /// <param name="dbSaveChangesMethod"></param>
    /// <param name="timeProvider"></param>
    /// <param name="instrumentationClient"></param>
    public LocalEntityDelete(
        IDbReadWriteEntityMethods<TEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        instrumentationClient) { }

    protected override async
        Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiDelete(
            TDeleteParams deleteParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Get persisted entity
        TEntity persistedObject =
            await _entityMethods
                .DbSet
                .SingleOrDefaultAsync(x => x.Id == deleteParams.Id) ??
            throw new KeyNotFoundException($"No record found for entity with ID {deleteParams.Id}.");

        return (persistedObject, nonErrorMessages);
    }
}
