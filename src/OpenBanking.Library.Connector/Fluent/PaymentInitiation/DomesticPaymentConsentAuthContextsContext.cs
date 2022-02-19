// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response;
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public interface IDomesticPaymentConsentAuthContextsContext :
        ICreateLocalContext<DomesticPaymentConsentAuthContextRequest, DomesticPaymentConsentAuthContextPostResponse>,
        IReadLocalContext<IDomesticPaymentConsentAuthContextPublicQuery, DomesticPaymentConsentAuthContextResponse>,
        IDeleteLocalContext { }

    internal class DomesticPaymentConsentAuthContextsContext :
        ObjectContextBase<DomesticPaymentConsentAuthContextPersisted>,
        IDomesticPaymentConsentAuthContextsContext
    {
        private readonly DomesticPaymentConsentAuthCreate _domesticPaymentConsentAuthCreate;

        private readonly
            LocalEntityRead<DomesticPaymentConsentAuthContextPersisted,
                IDomesticPaymentConsentAuthContextPublicQuery, DomesticPaymentConsentAuthContextResponse>
            _localEntityRead;

        public DomesticPaymentConsentAuthContextsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticPaymentConsentAuthCreate =
                new DomesticPaymentConsentAuthCreate(sharedContext);
            _localEntityRead =
                new LocalEntityRead<DomesticPaymentConsentAuthContextPersisted,
                    IDomesticPaymentConsentAuthContextPublicQuery, DomesticPaymentConsentAuthContextResponse>(
                    sharedContext);
        }

        public Task<IFluentResponse<DomesticPaymentConsentAuthContextPostResponse>> CreateLocalAsync(
            DomesticPaymentConsentAuthContextRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentConsentAuthCreate.CreateAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentConsentAuthContextResponse>> GetLocalAsync(Guid id) =>
            _localEntityRead.ReadAsync(id, null);

        public Task<IFluentResponse<IQueryable<DomesticPaymentConsentAuthContextResponse>>> GetLocalAsync(
            Expression<Func<IDomesticPaymentConsentAuthContextPublicQuery, bool>> predicate) =>
            _localEntityRead.ReadAsync(predicate);
    }
}
