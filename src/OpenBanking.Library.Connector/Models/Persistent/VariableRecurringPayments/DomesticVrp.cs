// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations.Schema;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using DomesticVrpRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request.DomesticVrp;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent.VariableRecurringPayments
{
    /// <summary>
    ///     Persisted type.
    ///     Internal to help ensure public request and response types used on public API.
    /// </summary>
    internal partial class DomesticVrp :
        EntityBase
    {
        public DomesticVrp() { }

        public DomesticVrp(
            Guid id,
            string? name,
            DomesticVrpRequest request,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest apiRequest,
            VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse apiResponse,
            string? createdBy,
            ITimeProvider timeProvider) : base(
            id,
            name,
            createdBy,
            timeProvider)
        {
            DomesticVrpConsentId = request.DomesticVrpConsentId;
            BankApiRequest = apiRequest;
            BankApiResponse =
                new ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse>(
                    apiResponse,
                    timeProvider,
                    createdBy);
            ExternalApiId = BankApiResponse.Data.Data.DomesticVRPId;
        }

        public Guid DomesticVrpConsentId { get; set; }

        [ForeignKey("DomesticVrpConsentId")]
        public DomesticVrpConsent DomesticVrpConsentNavigation { get; set; } = null!;

        public VariableRecurringPaymentsModelsPublic.OBDomesticVRPRequest BankApiRequest { get; set; } = null!;

        /// <summary>
        ///     External API ID, i.e. ID of object at bank. This should be unique between objects created at the
        ///     same bank but we do not assume global uniqueness between objects created at multiple banks.
        /// </summary>
        public string ExternalApiId { get; set; } = null!;

        public ReadWriteProperty<VariableRecurringPaymentsModelsPublic.OBDomesticVRPResponse> BankApiResponse
        {
            get;
            set;
        } = null!;
    }
}
