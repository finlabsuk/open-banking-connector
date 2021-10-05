// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response
{
    public interface IDomesticVrpPublicQuery : IBaseQuery
    {
        ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> BankApiResponse { get; }
    }

    public class DomesticVrpResponse : BaseResponse, IDomesticVrpPublicQuery
    {
        public DomesticVrpResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> bankApiResponse) : base(
            id,
            name,
            created,
            createdBy)
        {
            BankApiResponse = bankApiResponse;
        }

        public ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> BankApiResponse { get; }
    }
}
