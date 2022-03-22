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
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;

namespace FinnovationLabs.OpenBanking.Library.Connector.Fluent.VariableRecurringPayments
{
    public interface IDomesticVrpsContext :
        IExternalEntityContext<DomesticVrpRequest, DomesticVrpResponse> { }

    internal class DomesticVrpsContext :
        ObjectContextBase<DomesticVrp>, IDomesticVrpsContext
    {
        private readonly DomesticVrpCreate _domesticVrpCreate;
        private readonly DomesticVrpRead _domesticVrpRead;

        public DomesticVrpsContext(ISharedContext sharedContext) : base(sharedContext)
        {
            _domesticVrpRead = new DomesticVrpRead(sharedContext);
            _domesticVrpCreate = new DomesticVrpCreate(sharedContext);
        }

        public Task<IFluentResponse<DomesticVrpResponse>> ReadAsync(
            Guid id,
            string? modifiedBy = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpRead.ReadAsync(id, modifiedBy, apiResponseWriteFile, apiResponseOverrideFile);

        public Task<IFluentResponse<DomesticVrpResponse>> CreateAsync(
            DomesticVrpRequest publicRequest,
            string? createdBy = null,
            string? apiRequestWriteFile = null,
            string? apiResponseWriteFile = null,
            string? apiResponseOverrideFile = null) =>
            _domesticVrpCreate.CreateAsync(
                publicRequest,
                createdBy,
                apiRequestWriteFile,
                apiResponseWriteFile,
                apiResponseOverrideFile);

        public Task<IFluentResponse<IQueryable<DomesticVrpResponse>>> ReadLocalAsync(
            Expression<Func<IDomesticVrpPublicQuery, bool>> predicate) =>
            _domesticVrpRead.ReadAsync(predicate);

        public Task<IFluentResponse<DomesticVrpResponse>> GetLocalAsync(Guid id) =>
            _domesticVrpRead.ReadAsync(id, null);
    }
}
