// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response
{
    public interface IDomesticVrpConsentPublicQuery : IBaseQuery
    {
        Guid BankRegistrationId { get; }

        Guid BankApiSetId { get; }

        public string ExternalApiId { get; }
    }


    /// <summary>
    ///     Response to ReadLocal requests
    /// </summary>
    public class DomesticVrpConsentReadLocalResponse : BaseResponse, IDomesticVrpConsentPublicQuery
    {
        public DomesticVrpConsentReadLocalResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId) : base(id, name, created, createdBy)
        {
            BankRegistrationId = bankRegistrationId;
            BankApiSetId = bankApiSetId;
            ExternalApiId = externalApiId;
        }

        /// <summary>
        ///     ID of associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }


        /// <summary>
        ///     ID of associated BankApiSet object
        /// </summary>
        public Guid BankApiSetId { get; }

        /// <summary>
        ///     External (bank) API ID for this object
        /// </summary>
        public string ExternalApiId { get; }
    }


    /// <summary>
    ///     Response to Read and Create requests
    /// </summary>
    public class DomesticVrpConsentReadResponse : DomesticVrpConsentReadLocalResponse
    {
        public DomesticVrpConsentReadResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse externalApiResponse) : base(
            id,
            name,
            created,
            createdBy,
            bankRegistrationId,
            bankApiSetId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse ExternalApiResponse { get; }
    }

    /// <summary>
    ///     Response to ReadFundsConfirmation requests
    /// </summary>
    public class DomesticVrpConsentReadFundsConfirmationResponse : DomesticVrpConsentReadLocalResponse

    {
        public DomesticVrpConsentReadFundsConfirmationResponse(
            Guid id,
            string? name,
            DateTimeOffset created,
            string? createdBy,
            Guid bankRegistrationId,
            Guid bankApiSetId,
            string externalApiId,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse externalApiResponse) : base(
            id,
            name,
            created,
            createdBy,
            bankRegistrationId,
            bankApiSetId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse ExternalApiResponse { get; }
    }
}
