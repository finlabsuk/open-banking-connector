// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Request;
using VariableRecurringPaymentsModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Vrp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles
{
    public enum DomesticVrpTypeEnum
    {
        VrpWithDebtorAccountSpecifiedByPisp
    }

    public partial class BankProfile
    {
        public DomesticVrpConsent VrpRequest(
            Guid bankRegistrationId,
            Guid bankApiSetId,
            DomesticVrpTypeEnum domesticVrpType,
            string instructionIdentification,
            string endToEndIdentification,
            string? name)
        {
            var domesticVrpConsentRequest = new DomesticVrpConsent
            {
                Name = name,
                BankApiSetId = bankApiSetId,
                BankRegistrationId = bankRegistrationId,
                OBDomesticVRPConsentRequest = new VariableRecurringPaymentsModelsPublic.OBDomesticVRPConsentRequest
                {
                    Data = null,
                    Risk = null
                }
            };
            return VariableRecurringPaymentsApiSettings.DomesticVrpConsentAdjustments(domesticVrpConsentRequest);
        }
    }
}
