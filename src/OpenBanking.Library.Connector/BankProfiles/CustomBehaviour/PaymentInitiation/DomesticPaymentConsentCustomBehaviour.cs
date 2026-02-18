// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;

public class DomesticPaymentConsentCustomBehaviour : ReadWritePostCustomBehaviour
{
    public bool? PreferMisspeltContractPresentIndicator { get; set; }

    public bool? ResponseRiskContractPresentIndicatorMayBeMissingOrWrong { get; set; }

    public bool? ResponseDataFundsAvailableResultFundsAvailableMayBeWrong { get; set; }

    public bool? UseB64JoseHeader { get; set; }
}
