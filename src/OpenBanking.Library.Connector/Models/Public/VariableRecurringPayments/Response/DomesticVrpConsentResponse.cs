// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response
{
    public interface IDomesticVrpConsentPublicQuery : IBaseQuery
    {
        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     Associated VariableRecurringPaymentsApi object
        /// </summary>
        public Guid VariableRecurringPaymentsApiId { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }
    }


    /// <summary>
    ///     Response to ReadLocal requests
    /// </summary>
    public class DomesticVrpConsentReadLocalResponse : BaseResponse, IDomesticVrpConsentPublicQuery
    {
        internal DomesticVrpConsentReadLocalResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankRegistrationId,
            Guid variableRecurringPaymentsApiId,
            string externalApiId) : base(id, created, createdBy, reference)
        {
            BankRegistrationId = bankRegistrationId;
            VariableRecurringPaymentsApiId = variableRecurringPaymentsApiId;
            ExternalApiId = externalApiId;
        }

        /// <summary>
        ///     Associated BankRegistration object
        /// </summary>
        public Guid BankRegistrationId { get; }

        /// <summary>
        ///     Associated VariableRecurringPaymentsApi object
        /// </summary>
        public Guid VariableRecurringPaymentsApiId { get; }

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; }
    }


    /// <summary>
    ///     Response to Read and Create requests
    /// </summary>
    public class DomesticVrpConsentReadResponse : DomesticVrpConsentReadLocalResponse
    {
        internal DomesticVrpConsentReadResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankRegistrationId,
            Guid variableRecurringPaymentsApiId,
            string externalApiId,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            bankRegistrationId,
            variableRecurringPaymentsApiId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentResponse? ExternalApiResponse { get; }
    }

    /// <summary>
    ///     Response to ReadFundsConfirmation requests
    /// </summary>
    public class DomesticVrpConsentReadFundsConfirmationResponse : DomesticVrpConsentReadLocalResponse

    {
        internal DomesticVrpConsentReadFundsConfirmationResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankRegistrationId,
            Guid variableRecurringPaymentsApiId,
            string externalApiId,
            VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse externalApiResponse) : base(
            id,
            created,
            createdBy,
            reference,
            bankRegistrationId,
            variableRecurringPaymentsApiId,
            externalApiId)
        {
            ExternalApiResponse = externalApiResponse;
        }

        public VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse ExternalApiResponse { get; }
    }
}
