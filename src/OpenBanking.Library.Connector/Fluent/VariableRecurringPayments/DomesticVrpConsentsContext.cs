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
        private readonly DomesticVrpConsentFundsConfirmationRead
            _domesticVrpConsentFundsConfirmationRead;

        private readonly DomesticVrpConsentRead _domesticVrpConsentRead;
        private readonly DomesticVrpConsentCreate _domesticVrpConsentCreate;
        private readonly ISharedContext _sharedContext;

        public DomesticVrpConsentsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _sharedContext = sharedContext;
            _domesticVrpConsentRead = new DomesticVrpConsentRead(sharedContext);
            _domesticVrpConsentCreate = new DomesticVrpConsentCreate(sharedContext);
            _domesticVrpConsentFundsConfirmationRead =
                new DomesticVrpConsentFundsConfirmationRead(sharedContext);
        }

        public Task<IFluentResponse<DomesticVrpConsentResponse>> ReadAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpConsentRead.ReadAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticVrpConsentResponse>> CreateAsync(
            DomesticVrpConsentRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpConsentCreate.CreateAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticVrpConsentResponse>>> ReadLocalAsync(
            Expression<Func<IDomesticVrpConsentPublicQuery, bool>> predicate) =>
            _domesticVrpConsentRead.ReadAsync(predicate);

        public Task<IFluentResponse<DomesticVrpConsentResponse>> GetFundsConfirmationAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpConsentFundsConfirmationRead.ReadAsync(
                id,
                modifiedBy,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public IDomesticVrpConsentAuthContextsContext AuthContexts =>
            new DomesticVrpConsentAuthContextsContext(_sharedContext);

        public Task<IFluentResponse<DomesticVrpConsentResponse>> GetLocalAsync(Guid id) =>
            _domesticVrpConsentRead.ReadAsync(id, null);
    }
}
