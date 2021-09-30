// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response
{
    public interface IDomesticVrpPublicQuery : IBaseQuery
    {
        ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> BankApiResponse { get; }
    }

    public class DomesticVrpResponse : BaseResponse, IDomesticVrpPublicQuery
    {
        public DomesticVrpResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> bankApiResponse) : base(
            id,
            name,
            created,
            createdBy)
        {
            BankApiResponse = bankApiResponse;
        }

        public ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticResponse5> BankApiResponse { get; }
    }
}
