// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using DomesticPaymentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPayment;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IDomesticVrpContext :
        IEntityContext<DomesticPaymentRequest, IDomesticPaymentPublicQuery, DomesticPaymentResponse> { }

    internal class DomesticVrpContext : 
        ObjectContextBase<DomesticPayment>, IDomesticVrpContext
    {
        private readonly DomesticVrpGet _domesticVRPGet;
        private readonly DomesticVrpPost _domesticVRPPost;

        public DomesticVrpContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticVRPGet = new DomesticVrpGet(sharedContext);
            _domesticVRPPost = new DomesticVrpPost(sharedContext);
        }

        public Task<IFluentResponse<DomesticPaymentResponse>> GetAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVRPGet.GetAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentResponse>> PostAsync(
            DomesticPaymentRequest publicRequest,
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

        public Task<IFluentResponse<IQueryable<DomesticPaymentResponse>>> GetLocalAsync(
            Expression<Func<IDomesticPaymentPublicQuery, bool>> predicate) =>
            _domesticVRPGet.GetAsync(predicate);

        public Task<IFluentResponse<DomesticPaymentResponse>> GetLocalAsync(Guid id) =>
            _domesticVRPGet.GetAsync(id, null);
    }
}
