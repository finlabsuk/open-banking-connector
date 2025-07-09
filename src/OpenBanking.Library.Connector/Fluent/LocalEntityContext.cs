// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent;

internal class
    LocalEntityContext<TEntity, TPublicRequest, TPublicQuery, TPublicCreateLocalResponse,
        TPublicReadLocalResponse> :
    ILocalEntityContext<TPublicRequest, TPublicQuery, TPublicCreateLocalResponse,
        TPublicReadLocalResponse>
    where TEntity : class, IEntity,
    ISupportsFluentLocalEntityGet<TPublicReadLocalResponse>
    where TPublicCreateLocalResponse : class
    where TPublicReadLocalResponse : class
    where TPublicRequest : EntityBase, ISupportsValidation
{
    public LocalEntityContext(
        ISharedContext sharedContext,
        IObjectCreate<TPublicRequest, TPublicCreateLocalResponse, LocalCreateParams> postObject)
    {
        CreateLocalObject = postObject;
        ReadLocalObject =
            new LocalEntityRead<TEntity, TPublicQuery, TPublicReadLocalResponse>(
                sharedContext.DbService.GetDbEntityMethods<TEntity>(),
                sharedContext.DbService.GetDbMethods(),
                sharedContext.TimeProvider,
                sharedContext.Instrumentation);
        DeleteLocalObject = new LocalEntityDelete<TEntity, LocalDeleteParams>(
            sharedContext.DbService.GetDbEntityMethods<TEntity>(),
            sharedContext.DbService.GetDbMethods(),
            sharedContext.TimeProvider,
            sharedContext.Instrumentation);
    }

    public IObjectDelete<LocalDeleteParams> DeleteLocalObject { get; }

    public IObjectCreate<TPublicRequest, TPublicCreateLocalResponse, LocalCreateParams> CreateLocalObject { get; }

    public IObjectReadWithSearch<TPublicQuery, TPublicReadLocalResponse, LocalReadParams> ReadLocalObject { get; }
}
