// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent.Primitives;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;
using DomesticVrpConsentAuthContextRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.
    DomesticVrpConsentAuthContext;
using DomesticVrpConsentAuthContextPersisted =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments.
    DomesticVrpConsentAuthContext;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IDomesticVrpConsentAuthContextsContext :
        ICreateLocalContext<DomesticVrpConsentAuthContextRequest, DomesticVrpConsentAuthContextPostResponse>,
        IReadLocalContext<IDomesticVrpConsentAuthContextPublicQuery, DomesticVrpConsentAuthContextResponse>,
        IDeleteLocalContext { }

    internal class DomesticVrpConsentAuthContextsContext :
        ObjectContextBase<DomesticVrpConsentAuthContextPersisted>,
        IDomesticVrpConsentAuthContextsContext
    {
        private readonly DomesticVrpConsentAuthCreate _domesticPaymentConsentAuthCreate;

        private readonly
            LocalEntityRead<DomesticVrpConsentAuthContextPersisted,
                IDomesticVrpConsentAuthContextPublicQuery, DomesticVrpConsentAuthContextResponse>
            _localEntityRead;

        public DomesticVrpConsentAuthContextsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticPaymentConsentAuthCreate =
                new DomesticVrpConsentAuthCreate(sharedContext);
            _localEntityRead =
                new LocalEntityRead<DomesticVrpConsentAuthContextPersisted,
                    IDomesticVrpConsentAuthContextPublicQuery, DomesticVrpConsentAuthContextResponse>(sharedContext);
        }

        public Task<IFluentResponse<DomesticVrpConsentAuthContextPostResponse>> CreateLocalAsync(
            DomesticVrpConsentAuthContextRequest publicRequest,
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

        public Task<IFluentResponse<DomesticVrpConsentAuthContextResponse>> GetLocalAsync(Guid id) =>
            _localEntityRead.ReadAsync(id, null);

        public Task<IFluentResponse<IQueryable<DomesticVrpConsentAuthContextResponse>>> GetLocalAsync(
            Expression<Func<IDomesticVrpConsentAuthContextPublicQuery, bool>> predicate) =>
            _localEntityRead.ReadAsync(predicate);
    }
}
