// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Services;

namespace FinnovationLabs.OpenBanking.Library.Connector.Operations;

internal abstract class
    LocalEntityCreate<TEntity, TPublicRequest, TPublicResponse> : BaseCreate<TPublicRequest,
        TPublicResponse, LocalCreateParams>
    where TEntity : class, IEntity
    where TPublicRequest : Base
{
    protected readonly IDbReadWriteEntityMethods<TEntity> _entityMethods;

    public LocalEntityCreate(
        IDbReadWriteEntityMethods<TEntity> entityMethods,
        IDbSaveChangesMethod dbSaveChangesMethod,
        ITimeProvider timeProvider,
        IProcessedSoftwareStatementProfileStore softwareStatementProfileRepo,
        IInstrumentationClient instrumentationClient) : base(
        dbSaveChangesMethod,
        timeProvider,
        softwareStatementProfileRepo,
        instrumentationClient)
    {
        _entityMethods = entityMethods;
    }

    protected override async
        Task<(TPublicResponse response, IList<IFluentResponseInfoOrWarningMessage> nonErrorMessages)> ApiPost(
            TPublicRequest request,
            LocalCreateParams createParams)
    {
        // Create non-error list
        var nonErrorMessages =
            new List<IFluentResponseInfoOrWarningMessage>();

        // Add entity
        TPublicResponse response = await AddEntity(
            request,
            _timeProvider);

        // Persist updates (this happens last so as not to happen if there are any previous errors)
        await _dbSaveChangesMethod.SaveChangesAsync();

        return (response, nonErrorMessages);
    }

    protected abstract Task<TPublicResponse> AddEntity(
        TPublicRequest request,
        ITimeProvider timeProvider);
}
