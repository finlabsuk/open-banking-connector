// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

/// <summary>
///     Read operations on local entities (objects stored in local database only).
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TPublicResponse"></typeparam>
/// <typeparam name="TReadParams"></typeparam>
internal abstract class BaseRead<TEntity, TPublicResponse, TReadParams> :
    IObjectRead<TPublicResponse, TReadParams>
    where TEntity : class, IEntity
    where TReadParams : LocalReadParams
{
    private readonly IDbSaveChangesMethod _dbSaveChangesMethod;
    protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;
    protected readonly IInstrumentationClient _instrumentationClient;
    protected readonly ITimeProvider _timeProvider;


    public BaseRead(
        IDbReadWriteEntityMethods<TEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IInstrumentationClient instrumentationClient)
    {
        _entityMethods = entityMethods;
        _dbSaveChangesMethod = dbSaveChangesMethod;
        _timeProvider = timeProvider;
        _instrumentationClient = instrumentationClient;
    }

    public async Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ReadAsync(TReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // GET from bank API
        (TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> newNonErrorMessages) =
            await ApiGet(readParams);
        nonErrorMessages.AddRange(newNonErrorMessages);

        return (response, nonErrorMessages);
    }


    /// <summary>
    ///     Empty function as by definition POST local does not include POST to bank API.
    /// </summary>
    /// <returns></returns>
    protected abstract
        Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)>
        ApiGet(TReadParams readParams);
}
