// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation.Response
{
    public interface IDomesticPaymentConsentPublicQuery : IBaseQuery
    {
        ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> BankApiResponse { get; }

        Guid BankRegistrationId { get; }

        Guid BankApiInformationId { get; }
    }

    /// <summary>
    ///     Respnose to GetLocal
    /// </summary>
    public class DomesticPaymentConsentResponse : BaseResponse, IDomesticPaymentConsentPublicQuery
    {
        public DomesticPaymentConsentResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> bankApiResponse,
            Guid bankRegistrationId,
            Guid bankApiInformationId) : base(id, name, created, createdBy)
        {
            BankApiResponse = bankApiResponse;
            BankRegistrationId = bankRegistrationId;
            BankApiInformationId = bankApiInformationId;
        }

        public ReadWriteProperty<PaymentInitiationModelsPublic.OBWriteDomesticConsentResponse5> BankApiResponse { get; }
        public Guid BankRegistrationId { get; }
        public Guid BankApiInformationId { get; }
    }
}
