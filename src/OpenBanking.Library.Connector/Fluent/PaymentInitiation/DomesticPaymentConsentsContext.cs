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
using DomesticPaymentConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.DomesticPaymentConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.PaymentInitiation
{
    public interface IDomesticPaymentConsentsContext :
        IEntityContext<DomesticPaymentConsentRequest, IDomesticPaymentConsentPublicQuery,
            DomesticPaymentConsentResponse>
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        IDomesticPaymentConsentAuthContextsContext AuthContexts { get; }

        /// <summary>
        ///     GET entity by ID from Open Banking Connector.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IFluentResponse<DomesticPaymentConsentResponse>> GetFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal class DomesticPaymentConsentsConsentContext :
        ObjectContextBase<DomesticPaymentConsent>,
        IDomesticPaymentConsentsContext
    {
        private readonly DomesticPaymentConsentFundsConfirmationGet
            _domesticPaymentConsentFundsConfirmationGet;

        private readonly DomesticPaymentConsentsGet _domesticPaymentConsentsGet;
        private readonly DomesticPaymentConsentsPost _domesticPaymentConsentsPost;
        private readonly ISharedContext _sharedContext;

        public DomesticPaymentConsentsConsentContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            _domesticPaymentConsentsGet = new DomesticPaymentConsentsGet(sharedContext);
            _domesticPaymentConsentFundsConfirmationGet =
                new DomesticPaymentConsentFundsConfirmationGet(sharedContext);
            _domesticPaymentConsentsPost = new DomesticPaymentConsentsPost(sharedContext);
        }

        public IDomesticPaymentConsentAuthContextsContext AuthContexts =>
            new DomesticPaymentConsentAuthContextsContext(_sharedContext);

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> GetFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentConsentFundsConfirmationGet.GetAsync(
                id,
                modifiedBy,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> GetAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentConsentsGet.GetAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> PostAsync(
            DomesticPaymentConsentRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentConsentsPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticPaymentConsentResponse>>> GetLocalAsync(
            Expression<Func<IDomesticPaymentConsentPublicQuery, bool>> predicate) =>
            _domesticPaymentConsentsGet.GetAsync(predicate);

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> GetLocalAsync(Guid id)
            => GetAsync(id);
    }
}
