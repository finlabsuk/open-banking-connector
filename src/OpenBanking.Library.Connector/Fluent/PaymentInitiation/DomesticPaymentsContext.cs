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
    public interface IDomesticPaymentsContext :
        IEntityContext<DomesticPaymentRequest, IDomesticPaymentPublicQuery, DomesticPaymentResponse> { }

    internal class DomesticPaymentsContext : ObjectContextBase<DomesticPayment>, IDomesticPaymentsContext
    {
        private readonly DomesticPaymentsRead _domesticPaymentsRead;
        private readonly DomesticPaymentsCreate _domesticPaymentsCreate;

        public DomesticPaymentsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticPaymentsRead = new DomesticPaymentsRead(sharedContext);
            _domesticPaymentsCreate = new DomesticPaymentsCreate(sharedContext);
        }

        public Task<IFluentResponse<DomesticPaymentResponse>> ReadAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentsRead.ReadAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentResponse>> CreateAsync(
            DomesticPaymentRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentsCreate.CreateAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticPaymentResponse>>> ReadLocalAsync(
            Expression<Func<IDomesticPaymentPublicQuery, bool>> predicate) =>
            _domesticPaymentsRead.ReadAsync(predicate);

        public Task<IFluentResponse<DomesticPaymentResponse>> GetLocalAsync(Guid id) =>
            _domesticPaymentsRead.ReadAsync(id, null);
    }
}
