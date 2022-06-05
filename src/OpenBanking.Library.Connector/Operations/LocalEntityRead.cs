// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Microsoft.EntityFrameworkCore;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal class
    LocalEntityRead<TEntity, TPublicQuery, TPublicResponse> : BaseRead<TEntity, TPublicResponse, LocalReadParams>,
        IObjectReadWithSearch<TPublicQuery, TPublicResponse, LocalReadParams>
    where TEntity : class, ISupportsFluentLocalEntityGet<TPublicResponse>, IEntity
{
    public LocalEntityRead(
        IDbReadWriteEntityMethods<TEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient) : base(
        entityMethods,
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient) { }

    public async
        Task<(IQueryable<TPublicResponse> response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages
            )>
        ReadAsync(Expression<Func<TPublicQuery, bool>> predicate)
    {
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Convert expression tree type
        ParameterExpression entityInput = Expression.Parameter(typeof(TEntity), "entity");
        InvocationExpression main = Expression.Invoke(predicate, entityInput);
        Expression<Func<TEntity, bool>> predicateWithUpdatedType =
            Expression.Lambda<Func<TEntity, bool>>(main, entityInput);

        // Run query
        IQueryable<TEntity> resultEntity = await _entityMethods.GetNoTrackingAsync(predicateWithUpdatedType);

        // Process results
        IQueryable<TPublicResponse> resultResponse = resultEntity.Select(b => b.PublicGetLocalResponse);

        // Return success response (thrown exceptions produce error response)
        return (resultResponse, nonErrorMessages);
    }

    protected override async
        Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiGet(
            LocalReadParams readParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Create persisted entity
        TEntity persistedObject =
            await _entityMethods
                .DbSetNoTracking
                .SingleOrDefaultAsync(x => x.Id == readParams.Id) ??
            throw new KeyNotFoundException($"No record found for entity with ID {readParams.Id}.");

        // Create response
        TPublicResponse response = persistedObject.PublicGetLocalResponse;

        return (response, nonErrorMessages);
    }
}
