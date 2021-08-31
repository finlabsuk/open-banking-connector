// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.BankApiModels;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent
{
    internal class
        LocalEntityContext<TEntity, TPublicRequest, TPublicQuery, TPublicResponse> :
            ObjectContextBase<TEntity>,
            ILocalEntityContext<TPublicRequest, TPublicQuery, TPublicResponse>
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>,
        ISupportsFluentLocalEntityPost<TPublicRequest, TPublicResponse>,
        ISupportsFluentLocalEntityGet<TPublicResponse>, new()
        where TPublicResponse : class
        where TPublicRequest : Base, ISupportsValidation
    {
        private readonly LocalEntityGet<TEntity, TPublicQuery, TPublicResponse> _localEntityGet;
        private readonly LocalEntityPost<TEntity, TPublicRequest, TPublicResponse> _localEntityPost;

        public LocalEntityContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _localEntityGet = new LocalEntityGet<TEntity, TPublicQuery, TPublicResponse>(sharedContext);
            _localEntityPost =
                new LocalEntityPost<TEntity, TPublicRequest, TPublicResponse>(sharedContext);
        }

        Task<IFluentResponse<TPublicResponse>> IPostLocalContext<TPublicRequest, TPublicResponse>.PostLocalAsync(
            TPublicRequest publicRequest,
            string? createdBy,
            string? apiRequestWriteFile,
            string? apiResponseWriteFile,
            string? apiResponseOverrideFile) =>
            _localEntityPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<TPublicResponse>> GetLocalAsync(Guid id) =>
            _localEntityGet.GetAsync(id, null);

        public Task<IFluentResponse<IQueryable<TPublicResponse>>> GetLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate) =>
            _localEntityGet.GetAsync(predicate);
    }
}
