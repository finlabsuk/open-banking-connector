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
    public interface IDomesticVrpConsentPublicQuery : IBaseQuery
    {
        ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> BankApiResponse { get; }

        Guid BankRegistrationId { get; }

        Guid BankApiSetId { get; }
    }

    /// <summary>
    ///     Respnose to GetLocal
    /// </summary>
    public class DomesticVrpConsentResponse : BaseResponse, IDomesticVrpConsentPublicQuery
    {
        public DomesticVrpConsentResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> bankApiResponse,
            Guid bankRegistrationId,
            Guid bankApiSetId) : base(id, name, created, createdBy)
        {
            BankApiResponse = bankApiResponse;
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
        }

        public ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> BankApiResponse { get; }
        public Guid BankRegistrationId { get; }
        public Guid BankApiSetId { get; }
    }
}
