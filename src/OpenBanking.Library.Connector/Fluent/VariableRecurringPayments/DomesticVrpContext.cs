// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IDomesticVrpContext :
        IEntityContext<DomesticVrpRequest, IDomesticVrpPublicQuery, DomesticVrpResponse> { }

    internal class DomesticVrpContext :
        ObjectContextBase<DomesticVrp>, IDomesticVrpContext
    {
        private readonly DomesticVrpGet _domesticVRPGet;
        private readonly DomesticVrpPost _domesticVRPPost;

        public DomesticVrpContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticVRPGet = new DomesticVrpGet(sharedContext);
            _domesticVRPPost = new DomesticVrpPost(sharedContext);
        }

        public Task<IFluentResponse<DomesticVrpResponse>> GetAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVRPGet.GetAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticVrpResponse>> PostAsync(
            DomesticVrpRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVRPPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticVrpResponse>>> GetLocalAsync(
            Expression<Func<IDomesticVrpPublicQuery, bool>> predicate) =>
            _domesticVRPGet.GetAsync(predicate);

        public Task<IFluentResponse<DomesticVrpResponse>> GetLocalAsync(Guid id) =>
            _domesticVRPGet.GetAsync(id, null);
    }
}
