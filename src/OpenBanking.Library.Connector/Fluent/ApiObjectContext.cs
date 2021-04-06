// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface IApiObjectContext<in TPublicRequest, TPublicPostResponse, TPublicGetResponse, TPublicQuery, TPublicGetLocalResponse> :
        ILocalEntityContext<TPublicRequest, TPublicPostResponse, TPublicQuery, TPublicGetLocalResponse>,
        IGetContext<TPublicGetResponse>
        where TPublicPostResponse : class
        where TPublicGetLocalResponse : class
        where TPublicGetResponse : class { }

    internal class ApiObjectContext<TEntity, TPublicRequest, TPublicPostResponse, TPublicGetResponse, TPublicQuery, TPublicGetLocalResponse> :
        LocalEntityContext<TEntity, TPublicRequest, TPublicPostResponse, TPublicQuery, TPublicGetLocalResponse>,
        IApiObjectContext<TPublicRequest, TPublicPostResponse, TPublicGetResponse, TPublicQuery, TPublicGetLocalResponse>
        where TEntity : class, TPublicQuery,
        ISupportsFluentGetLocal<TEntity, TPublicQuery, TPublicGetLocalResponse>,
        ISupportsFluentPost<TPublicRequest, TPublicPostResponse>,
        ISupportsFluentDeleteLocal<TEntity>,
        ISupportsFluentGet<TPublicGetResponse>, new()
        where TPublicRequest : class, ISupportsValidation
        where TPublicPostResponse : class
        where TPublicGetResponse : class
        where TPublicQuery : class
        where TPublicGetLocalResponse : class, TPublicQuery
    {
        private readonly GetContext<TEntity, TPublicGetResponse> _getContext;

        public ApiObjectContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _getContext = new GetContext<TEntity, TPublicGetResponse>(sharedContext);
        }

        public Task<IFluentResponse<TPublicGetResponse>> GetAsync(Guid id) =>
            _getContext.GetAsync(id);
    }
}
