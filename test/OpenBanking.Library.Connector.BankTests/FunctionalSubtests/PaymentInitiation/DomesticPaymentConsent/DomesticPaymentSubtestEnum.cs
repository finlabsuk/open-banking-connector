// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Templates.PaymentInitiation;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.
    DomesticPaymentConsent;

/// <summary>
///     Domestic payment functional subtest
/// </summary>
public enum DomesticPaymentSubtestEnum
{
    PersonToPersonSubtest,
    PersonToMerchantSubtest
}

public static class DomesticPaymentSubtestHelper
{
    static DomesticPaymentSubtestHelper()
    {
        AllDomesticPaymentSubtests = Enum.GetValues(typeof(DomesticPaymentSubtestEnum))
            .Cast<DomesticPaymentSubtestEnum>().ToHashSet();
    }

    public static ISet<DomesticPaymentSubtestEnum> AllDomesticPaymentSubtests { get; }

    public static DomesticPaymentTemplateType GetDomesticPaymentTemplateType(DomesticPaymentSubtestEnum subtestEnum) =>
        subtestEnum switch
        {
            DomesticPaymentSubtestEnum.PersonToPersonSubtest => DomesticPaymentTemplateType.PersonToPersonExample,
            DomesticPaymentSubtestEnum.PersonToMerchantSubtest => DomesticPaymentTemplateType
                .PersonToMerchantExample,
            _ => throw new ArgumentException(
                $"{nameof(subtestEnum)} is not valid {nameof(DomesticPaymentSubtestEnum)} or needs to be added to this switch statement.")
        };
}
