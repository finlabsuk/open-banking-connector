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
    public interface IDomesticVrpConsentsContext :
        IEntityContext<DomesticPaymentRequest, IDomesticPaymentPublicQuery, DomesticPaymentResponse> { }

    internal class DomesticVrpConsentsContext : 
        ObjectContextBase<DomesticPayment>, IDomesticVrpConsentsContext
    {
        private readonly DomesticVrpConsentGet _domesticPaymentsConsentGet;
        private readonly DomesticVrpConsentPost _domesticPaymentsConsentPost;

        public DomesticVrpConsentsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticPaymentsConsentGet = new DomesticVrpConsentGet(sharedContext);
            _domesticPaymentsConsentPost = new DomesticVrpConsentPost(sharedContext);
        }

        public Task<IFluentResponse<DomesticPaymentResponse>> GetAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentsConsentGet.GetAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentResponse>> PostAsync(
            DomesticPaymentRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentsConsentPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticPaymentResponse>>> GetLocalAsync(
            Expression<Func<IDomesticPaymentPublicQuery, bool>> predicate) =>
            _domesticPaymentsConsentGet.GetAsync(predicate);

        public Task<IFluentResponse<DomesticPaymentResponse>> GetLocalAsync(Guid id) =>
            _domesticPaymentsConsentGet.GetAsync(id, null);
    }
}
