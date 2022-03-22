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
        LocalEntityContext<TEntity, TPublicRequest, TPublicQuery, TPublicCreateLocalResponse,
            TPublicReadLocalResponse> :
            ObjectContextBase<TEntity>,
            ILocalEntityContext<TPublicRequest, TPublicQuery, TPublicCreateLocalResponse, TPublicReadLocalResponse>
        where TEntity : class, ISupportsFluentDeleteLocal<TEntity>,
        ISupportsFluentLocalEntityGet<TPublicReadLocalResponse>,
        ISupportsFluentLocalEntityPost<TPublicRequest, TPublicCreateLocalResponse, TEntity>, new()
        where TPublicCreateLocalResponse : class
        where TPublicReadLocalResponse : class
        where TPublicRequest : Base, ISupportsValidation
    {
        private readonly CreateBase<TPublicRequest, TPublicCreateLocalResponse> _localEntityCreate;
        private readonly LocalEntityRead<TEntity, TPublicQuery, TPublicReadLocalResponse> _localEntityRead;

        public LocalEntityContext(ISharedContext sharedContext, CreateBase<TPublicRequest, TPublicCreateLocalResponse> entityCreate) : base(sharedContext)
        {
            _localEntityRead = new LocalEntityRead<TEntity, TPublicQuery, TPublicReadLocalResponse>(sharedContext);
            _localEntityCreate = entityCreate;
        }

        Task<IFluentResponse<TPublicCreateLocalResponse>>
            ICreateLocalContext<TPublicRequest, TPublicCreateLocalResponse>.CreateLocalAsync(
                TPublicRequest publicRequest,
                string? createdBy,
                string? apiRequestWriteFile,
                string? apiResponseWriteFile,
                string? apiResponseOverrideFile) =>
            _localEntityCreate.CreateAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<TPublicReadLocalResponse>> ReadLocalAsync(Guid id) =>
            _localEntityRead.ReadAsync(id, null);

        public Task<IFluentResponse<IQueryable<TPublicReadLocalResponse>>> ReadLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate) =>
            _localEntityRead.ReadAsync(predicate);
    }
}
