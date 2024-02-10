// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.VariableRecurringPayments.Response;

/// <summary>
///     Response to DomesticVrpConsent ReadFundsConfirmation requests.
/// </summary>
public class DomesticVrpConsentFundsConfirmationResponse : BaseResponse

{
    public required VariableRecurringPaymentsModelsPublic.OBVRPFundsConfirmationResponse ExternalApiResponse
    {
        get;
        init;
    }
}
