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
        private readonly LocalEntityRead<TEntity, TPublicQuery, TPublicResponse> _localEntityRead;
        private readonly LocalEntityCreate<TEntity, TPublicRequest, TPublicResponse> _localEntityCreate;

        public LocalEntityContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _localEntityRead = new LocalEntityRead<TEntity, TPublicQuery, TPublicResponse>(sharedContext);
            _localEntityCreate =
                new LocalEntityCreate<TEntity, TPublicRequest, TPublicResponse>(sharedContext);
        }

        Task<IFluentResponse<TPublicResponse>> ICreateLocalContext<TPublicRequest, TPublicResponse>.CreateLocalAsync(
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

        public Task<IFluentResponse<TPublicResponse>> GetLocalAsync(Guid id) =>
            _localEntityRead.ReadAsync(id, null);

        public Task<IFluentResponse<IQueryable<TPublicResponse>>> GetLocalAsync(
            Expression<Func<TPublicQuery, bool>> predicate) =>
            _localEntityRead.ReadAsync(predicate);
    }
}
