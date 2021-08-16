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

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    internal class DomesticPaymentsContext :
        ObjectContextBase<DomesticPayment>,
        IEntityContext<DomesticPaymentRequest, IDomesticPaymentPublicQuery, DomesticPaymentResponse>
    {
        private readonly DomesticPaymentsGet _domesticPaymentsGet;
        private readonly DomesticPaymentsPost _domesticPaymentsPost;

        public DomesticPaymentsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticPaymentsGet = new DomesticPaymentsGet(sharedContext);
            _domesticPaymentsPost = new DomesticPaymentsPost(sharedContext);
        }

        public Task<IFluentResponse<DomesticPaymentResponse>> GetAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentsGet.GetAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentResponse>> PostAsync(
            DomesticPaymentRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentsPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticPaymentResponse>>> GetLocalAsync(
            Expression<Func<IDomesticPaymentPublicQuery, bool>> predicate) =>
            _domesticPaymentsGet.GetAsync(predicate);

        public Task<IFluentResponse<DomesticPaymentResponse>> GetLocalAsync(Guid id) =>
            _domesticPaymentsGet.GetAsync(id, null);
    }
}
