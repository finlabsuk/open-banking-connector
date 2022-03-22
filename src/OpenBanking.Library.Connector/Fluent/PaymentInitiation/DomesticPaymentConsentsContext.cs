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
using DomesticPaymentConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Request.
    DomesticPaymentConsentAuthContext;
using DomesticPaymentConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.PaymentInitiation.DomesticPaymentConsentAuthContext;

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
        ILocalEntityContext<DomesticPaymentConsentAuthContextRequest,
                IDomesticPaymentConsentAuthContextPublicQuery,
                DomesticPaymentConsentAuthContextCreateLocalResponse,
                DomesticPaymentConsentAuthContextReadLocalResponse>
            AuthContexts { get; }

        /// <summary>
        ///     READ funds confirmation by ID (includes GETing object from bank API).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="apiResponseWriteFile"></param>
        /// <param name="apiResponseOverrideFile"></param>
        /// <returns></returns>
        Task<IFluentResponse<DomesticPaymentConsentResponse>> ReadFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal class DomesticPaymentConsentsConsentContext :
        ObjectContextBase<DomesticPaymentConsent>,
        IDomesticPaymentConsentsContext
    {
        private readonly DomesticPaymentConsentFundsConfirmationRead
            _domesticPaymentConsentFundsConfirmationRead;

        private readonly DomesticPaymentConsentsCreate _domesticPaymentConsentsCreate;

        private readonly DomesticPaymentConsentsRead _domesticPaymentConsentsRead;
        private readonly ISharedContext _sharedContext;

        public DomesticPaymentConsentsConsentContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            _domesticPaymentConsentsRead = new DomesticPaymentConsentsRead(sharedContext);
            _domesticPaymentConsentFundsConfirmationRead =
                new DomesticPaymentConsentFundsConfirmationRead(sharedContext);
            _domesticPaymentConsentsCreate = new DomesticPaymentConsentsCreate(sharedContext);
        }

        public ILocalEntityContext<DomesticPaymentConsentAuthContextRequest,
            IDomesticPaymentConsentAuthContextPublicQuery,
            DomesticPaymentConsentAuthContextCreateLocalResponse,
            DomesticPaymentConsentAuthContextReadLocalResponse> AuthContexts =>
            new LocalEntityContext<DomesticPaymentConsentAuthContextPersisted,
                DomesticPaymentConsentAuthContextRequest,
                IDomesticPaymentConsentAuthContextPublicQuery,
                DomesticPaymentConsentAuthContextCreateLocalResponse,
                DomesticPaymentConsentAuthContextReadLocalResponse>(
                _sharedContext,
                new DomesticPaymentConsentAuthCreate(_sharedContext));

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> ReadFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentConsentFundsConfirmationRead.ReadAsync(
                id,
                modifiedBy,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> ReadAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentConsentsRead.ReadAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> CreateAsync(
            DomesticPaymentConsentRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticPaymentConsentsCreate.CreateAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticPaymentConsentResponse>> ReadLocalAsync(Guid id)
            => ReadAsync(id);

        public Task<IFluentResponse<IQueryable<DomesticPaymentConsentResponse>>> ReadLocalAsync(
            Expression<Func<IDomesticPaymentConsentPublicQuery, bool>> predicate) =>
            _domesticPaymentConsentsRead.ReadAsync(predicate);
    }
}
