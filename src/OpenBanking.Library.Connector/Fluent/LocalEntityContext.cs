// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal class
        LocalEntityContextInternal<TEntity, TPublicRequest, TPublicQuery, TPublicCreateLocalResponse,
            TPublicReadLocalResponse> :
            ObjectContextBase<TEntity>,
            ILocalEntityContextInternal<TPublicRequest, TPublicQuery, TPublicCreateLocalResponse,
                TPublicReadLocalResponse>
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>,
        ISupportsFluentLocalEntityGet<TPublicReadLocalResponse>,
        ISupportsFluentLocalEntityPost<TPublicRequest, TPublicCreateLocalResponse, TEntity>, new()
        where TPublicCreateLocalResponse : class
        where TPublicReadLocalResponse : class
        where TPublicRequest : Base, ISupportsValidation
    {
        public LocalEntityContextInternal(
            ISharedContext sharedContext,
            IObjectPost<TPublicRequest, TPublicCreateLocalResponse> postObject) : base(sharedContext)
        {
            PostObject = postObject;
            ReadLocalObject =
                new LocalEntityGet<TEntity, TPublicQuery, TPublicReadLocalResponse>(
                    sharedContext.DbService.GetDbEntityMethodsClass<TEntity>(),
                    sharedContext.DbService.GetDbSaveChangesMethodClass(),
                    sharedContext.TimeProvider,
                    sharedContext.SoftwareStatementProfileCachedRepo,
                    sharedContext.Instrumentation);
        }

        public IObjectPost<TPublicRequest, TPublicCreateLocalResponse> PostObject { get; }

        public IObjectReadLocal<TPublicQuery, TPublicReadLocalResponse> ReadLocalObject { get; }
    }
}
