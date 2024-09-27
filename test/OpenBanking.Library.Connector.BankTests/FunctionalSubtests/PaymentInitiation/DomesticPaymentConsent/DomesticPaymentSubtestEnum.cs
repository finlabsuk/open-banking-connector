// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.
    DomesticPaymentConsent;

/// <summary>
///     Domestic payment functional subtest
/// </summary>
public enum DomesticPaymentSubtestEnum
{
    PaymentToSelf
}

public static class DomesticPaymentSubtestHelper
{
    static DomesticPaymentSubtestHelper()
    {
        AllDomesticPaymentSubtests = Enum.GetValues(typeof(DomesticPaymentSubtestEnum))
            .Cast<DomesticPaymentSubtestEnum>().ToHashSet();
    }

    public static ISet<DomesticPaymentSubtestEnum> AllDomesticPaymentSubtests { get; }
}
