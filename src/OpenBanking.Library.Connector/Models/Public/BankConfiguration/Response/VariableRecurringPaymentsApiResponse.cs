// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Response
{
    public interface IVariableRecurringPaymentsApiQuery : IBaseQuery
    {
        Guid BankId { get; }
        VariableRecurringPaymentsApiVersion ApiVersion { get; }
        string BaseUrl { get; }
    }

    /// <summary>
    ///     Response to GetLocal
    /// </summary>
    public class VariableRecurringPaymentsApiResponse : BaseResponse, IVariableRecurringPaymentsApiQuery
    {
        internal VariableRecurringPaymentsApiResponse(
            Guid id,
            DateTimeOffset created,
            string? createdBy,
            string? reference,
            Guid bankId,
            VariableRecurringPaymentsApiVersion apiVersion,
            string baseUrl) : base(id, created, createdBy, reference)
        {
            BankId = bankId;
            ApiVersion = apiVersion;
            BaseUrl = baseUrl;
        }

        /// <summary>
        ///     Bank with which this API is associated.
        /// </summary>
        public Guid BankId { get; }

        /// <summary>
        ///     Version of UK Open Banking Account and Transaction API.
        /// </summary>
        public VariableRecurringPaymentsApiVersion ApiVersion { get; set; }

        /// <summary>
        ///     Base URL for UK Open Banking Account and Transaction API.
        /// </summary>
        public string BaseUrl { get; set; }
    }
}
