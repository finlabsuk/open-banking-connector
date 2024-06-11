// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.BankApiModels.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.VariableRecurringPayments;

public class DomesticVrpPostCustomBehaviour : ReadWritePostCustomBehaviour
{
    public DomesticVrpRefundConverterOptions? RefundResponseJsonConverter { get; set; }

    public bool? PreferMisspeltContractPresentIndicator { get; set; }

    public bool? OmitPsuInteractionType { get; set; }

    public bool? OmitVrpType { get; set; }
}
