// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal abstract class BaseDelete<TEntity, TDeleteParams> :
    IObjectDelete<TDeleteParams>
    where TEntity : class, IEntity
    where TDeleteParams : LocalDeleteParams
{
    private readonly IDbMethods _dbSaveChangesMethod;
    protected readonly IDbEntityMethods<TEntity> _entityMethods;
    protected readonly IInstrumentationClient _instrumentationClient;
    private readonly ITimeProvider _timeProvider;

    public BaseDelete(
        IDbEntityMethods<TEntity> entityMethods,
        IDbMethods dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient)
    {
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
    }

    public async
        Task<IList<IFluentResponseInfoOrWarningMessage>> DeleteAsync(TDeleteParams deleteParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // DELETE at bank API
        (TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await ApiDelete(deleteParams);
        nonErrorMessages.AddRange(newNonErrorMessages);

        // Local soft delete
        persistedObject.UpdateIsDeleted(
            true,
            _timeProvider.GetUtcNow(),
            deleteParams.ModifiedBy);

        await _dbSaveChangesMethod.SaveChangesAsync();

        // Return success response (thrown exceptions produce error response)
        return nonErrorMessages;
    }

    protected abstract
        Task<(TEntity persistedObject, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ApiDelete(TDeleteParams deleteParams);
}
