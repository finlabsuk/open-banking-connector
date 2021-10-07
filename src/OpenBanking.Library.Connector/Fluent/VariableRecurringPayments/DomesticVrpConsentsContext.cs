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
using DomesticVrpConsentRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrpConsent;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IDomesticVrpConsentsContext :
        IEntityContext<DomesticVrpConsentRequest, IDomesticVrpConsentPublicQuery, DomesticVrpConsentResponse>
    {
        /// <summary>
        ///     API for AuthorisationRedirectObject which corresponds to data received from bank following user
        ///     authorisation of consent.
        /// </summary>
        IDomesticVrpConsentAuthContextsContext AuthContexts { get; }

        Task<IFluentResponse<DomesticVrpConsentResponse>> GetFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null);
    }

    internal class DomesticVrpConsentsContext :
        ObjectContextBase<DomesticVrpConsent>, IDomesticVrpConsentsContext
    {
        private readonly DomesticVrpConsentFundsConfirmationGet
            _domesticVrpConsentFundsConfirmationGet;

        private readonly DomesticVrpConsentGet _domesticVrpConsentGet;
        private readonly DomesticVrpConsentPost _domesticVrpConsentPost;
        private readonly ISharedContext _sharedContext;

        public DomesticVrpConsentsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            _domesticVrpConsentGet = new DomesticVrpConsentGet(sharedContext);
            _domesticVrpConsentPost = new DomesticVrpConsentPost(sharedContext);
            _domesticVrpConsentFundsConfirmationGet =
                new DomesticVrpConsentFundsConfirmationGet(sharedContext);
        }

        public Task<IFluentResponse<DomesticVrpConsentResponse>> GetAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpConsentGet.GetAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticVrpConsentResponse>> PostAsync(
            DomesticVrpConsentRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpConsentPost.PostAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticVrpConsentResponse>>> GetLocalAsync(
            Expression<Func<IDomesticVrpConsentPublicQuery, bool>> predicate) =>
            _domesticVrpConsentGet.GetAsync(predicate);

        public Task<IFluentResponse<DomesticVrpConsentResponse>> GetFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpConsentFundsConfirmationGet.GetAsync(
                id,
                modifiedBy,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public IDomesticVrpConsentAuthContextsContext AuthContexts =>
            new DomesticVrpConsentAuthContextsContext(_sharedContext);

        public Task<IFluentResponse<DomesticVrpConsentResponse>> GetLocalAsync(Guid id) =>
            _domesticVrpConsentGet.GetAsync(id, null);
    }
}
