// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment
{
    public static partial class TestDefinitions
    {
        public static DomesticPaymentFunctionalSubtest PersonToPersonSubtest { get; } =
            new DomesticPaymentFunctionalSubtest(
                domesticPaymentFunctionalSubtestEnum: DomesticPaymentFunctionalSubtestEnum.PersonToPersonSubtest);
    }
}
