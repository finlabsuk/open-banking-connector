// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.CustomBehaviour.PaymentInitiation;

public class DomesticPaymentCustomBehaviour : ReadWritePostCustomBehaviour
{
    public bool? PreferMisspeltContractPresentIndicator { get; set; }
    public bool? ResponseDataStatusMayBeWrong { get; set; }
    public bool? ResponseDataDebtorMayBeMissingOrWrong { get; set; }
    public bool? ResponseDataDebtorSchemeNameMayBeMissingOrWrong { get; set; }
    public bool? ResponseDataDebtorIdentificationMayBeMissingOrWrong { get; set; }
    public bool? ResponseDataRefundMayBeMissingOrWrong { get; set; }
    public bool? ResponseDataRefundAccountSchemeNameMayBeMissingOrWrong { get; set; }
    public bool? ResponseDataRefundAccountIdentificationMayBeMissingOrWrong { get; set; }

    public bool? UseB64JoseHeader { get; set; }
}
