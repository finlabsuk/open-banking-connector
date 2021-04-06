// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.ApiModels.Base;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    public interface ILocalEntityContext<in TPublicRequest, TPublicPostResponse, TPublicQuery, TPublicGetLocalResponse> :
        IPostContext<TPublicRequest, TPublicPostResponse>,
        IGetLocalContext<TPublicQuery, TPublicGetLocalResponse>,
        IDeleteLocalContext
        where TPublicPostResponse : class
        where TPublicGetLocalResponse : class { }

    internal class
        LocalEntityContext<TEntity, TPublicRequest, TPublicPostResponse, TPublicQuery, TPublicGetLocalResponse> :
            ILocalEntityContext<TPublicRequest, TPublicPostResponse, TPublicQuery, TPublicGetLocalResponse>
        where TEntity : class, TPublicQuery,
        ISupportsFluentGetLocal<TEntity, TPublicQuery, TPublicGetLocalResponse>,
        ISupportsFluentPost<TPublicRequest, TPublicPostResponse>,
        ISupportsFluentDeleteLocal<TEntity>, new()
        where TPublicRequest : class, ISupportsValidation
        where TPublicPostResponse : class
        where TPublicQuery : class
        where TPublicGetLocalResponse : class, TPublicQuery
    {
        private readonly DeleteLocalContext<TEntity> _deleteLocalContext;
        private readonly GetLocalContext<TEntity, TPublicQuery, TPublicGetLocalResponse> _getLocalContext;
        private readonly PostContext<TEntity, TPublicRequest, TPublicPostResponse> _postContext;

        public LocalEntityContext(ISharedContext sharedContext)
        {
            _postContext = new PostContext<TEntity, TPublicRequest, TPublicPostResponse>(sharedContext);
            _getLocalContext = new GetLocalContext<TEntity, TPublicQuery, TPublicGetLocalResponse>(sharedContext);
            _deleteLocalContext =
                new DeleteLocalContext<TEntity>(sharedContext);
        }

        public Task<IFluentResponse<TPublicGetLocalResponse>> GetLocalAsync(Guid id) =>
            _getLocalContext.GetLocalAsync(id);

        public Task<IFluentResponse<IQueryable<TPublicGetLocalResponse>>> GetLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate) =>
            _getLocalContext.GetLocalAsync(predicate);

        public Task<IFluentResponse<TPublicPostResponse>> PostAsync(
            TPublicRequest publicRequest,
            string? modifiedBy = null) =>
            _postContext.PostAsync(publicRequest, modifiedBy);

        public Task<IFluentResponse> DeleteLocalAsync(Guid id, string? modifiedBy = null) =>
            _deleteLocalContext.DeleteLocalAsync(id, modifiedBy);
    }
}
