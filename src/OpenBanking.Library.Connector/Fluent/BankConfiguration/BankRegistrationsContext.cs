// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using BankRegistrationPersisted = FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.BankRegistration;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.BankConfiguration
{
    public interface IBankRegistrationsContext :
        IPostContext<BankRegistration, BankRegistrationResponse>,
        IGetLocalContext<IBankRegistrationPublicQuery, BankRegistrationResponse>,
        IDeleteLocalContext,
        IDeleteContext { }

    internal class BankRegistrationsContext :
        ObjectContextBase<BankRegistrationPersisted>,
        IBankRegistrationsContext
    {
        private readonly BankRegistrationDelete _bankRegistrationDelete;
        private readonly BankRegistrationsPost _bankRegistrationsPost;

        private readonly LocalEntityGet<BankRegistrationPersisted, IBankRegistrationPublicQuery,
            BankRegistrationResponse> _localEntityGet;

        public BankRegistrationsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _bankRegistrationsPost = new BankRegistrationsPost(sharedContext);
            _localEntityGet =
                new LocalEntityGet<BankRegistrationPersisted, IBankRegistrationPublicQuery,
                    BankRegistrationResponse>(sharedContext);
            _bankRegistrationDelete = new BankRegistrationDelete(sharedContext);
        }

        public Task<IFluentResponse<IQueryable<BankRegistrationResponse>>> GetLocalAsync(
            Expression<Func<IBankRegistrationPublicQuery, bool>> predicate) =>
            _localEntityGet.GetAsync(predicate);

        public Task<IFluentResponse<BankRegistrationResponse>>
            GetLocalAsync(Guid id) =>
            _localEntityGet.GetAsync(id, null);

        public Task<IFluentResponse<BankRegistrationResponse>> PostAsync(
            BankRegistration publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _bankRegistrationsPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse> DeleteAsync(Guid id, string? modifiedBy, bool useRegistrationAccessToken) =>
            _bankRegistrationDelete.DeleteAsync(id, modifiedBy, useRegistrationAccessToken);
    }
}
