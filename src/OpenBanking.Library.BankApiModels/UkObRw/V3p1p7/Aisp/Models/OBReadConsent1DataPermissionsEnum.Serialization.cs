// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    internal static partial class OBReadConsent1DataPermissionsEnumExtensions
    {
        public static string ToSerialString(this OBReadConsent1DataPermissionsEnum value) => value switch
        {
            OBReadConsent1DataPermissionsEnum.ReadAccountsBasic => "ReadAccountsBasic",
            OBReadConsent1DataPermissionsEnum.ReadAccountsDetail => "ReadAccountsDetail",
            OBReadConsent1DataPermissionsEnum.ReadBalances => "ReadBalances",
            OBReadConsent1DataPermissionsEnum.ReadBeneficiariesBasic => "ReadBeneficiariesBasic",
            OBReadConsent1DataPermissionsEnum.ReadBeneficiariesDetail => "ReadBeneficiariesDetail",
            OBReadConsent1DataPermissionsEnum.ReadDirectDebits => "ReadDirectDebits",
            OBReadConsent1DataPermissionsEnum.ReadOffers => "ReadOffers",
            OBReadConsent1DataPermissionsEnum.ReadPAN => "ReadPAN",
            OBReadConsent1DataPermissionsEnum.ReadParty => "ReadParty",
            OBReadConsent1DataPermissionsEnum.ReadPartyPSU => "ReadPartyPSU",
            OBReadConsent1DataPermissionsEnum.ReadProducts => "ReadProducts",
            OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsBasic => "ReadScheduledPaymentsBasic",
            OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsDetail => "ReadScheduledPaymentsDetail",
            OBReadConsent1DataPermissionsEnum.ReadStandingOrdersBasic => "ReadStandingOrdersBasic",
            OBReadConsent1DataPermissionsEnum.ReadStandingOrdersDetail => "ReadStandingOrdersDetail",
            OBReadConsent1DataPermissionsEnum.ReadStatementsBasic => "ReadStatementsBasic",
            OBReadConsent1DataPermissionsEnum.ReadStatementsDetail => "ReadStatementsDetail",
            OBReadConsent1DataPermissionsEnum.ReadTransactionsBasic => "ReadTransactionsBasic",
            OBReadConsent1DataPermissionsEnum.ReadTransactionsCredits => "ReadTransactionsCredits",
            OBReadConsent1DataPermissionsEnum.ReadTransactionsDebits => "ReadTransactionsDebits",
            OBReadConsent1DataPermissionsEnum.ReadTransactionsDetail => "ReadTransactionsDetail",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadConsent1DataPermissionsEnum value.")
        };

        public static OBReadConsent1DataPermissionsEnum ToOBReadConsent1DataPermissionsEnum(this string value)
        {
            if (string.Equals(value, "ReadAccountsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadAccountsBasic;
            if (string.Equals(value, "ReadAccountsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadAccountsDetail;
            if (string.Equals(value, "ReadBalances", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadBalances;
            if (string.Equals(value, "ReadBeneficiariesBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadBeneficiariesBasic;
            if (string.Equals(value, "ReadBeneficiariesDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadBeneficiariesDetail;
            if (string.Equals(value, "ReadDirectDebits", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadDirectDebits;
            if (string.Equals(value, "ReadOffers", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadOffers;
            if (string.Equals(value, "ReadPAN", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadPAN;
            if (string.Equals(value, "ReadParty", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadParty;
            if (string.Equals(value, "ReadPartyPSU", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadPartyPSU;
            if (string.Equals(value, "ReadProducts", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadProducts;
            if (string.Equals(value, "ReadScheduledPaymentsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsBasic;
            if (string.Equals(value, "ReadScheduledPaymentsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadScheduledPaymentsDetail;
            if (string.Equals(value, "ReadStandingOrdersBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadStandingOrdersBasic;
            if (string.Equals(value, "ReadStandingOrdersDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadStandingOrdersDetail;
            if (string.Equals(value, "ReadStatementsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadStatementsBasic;
            if (string.Equals(value, "ReadStatementsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadStatementsDetail;
            if (string.Equals(value, "ReadTransactionsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadTransactionsBasic;
            if (string.Equals(value, "ReadTransactionsCredits", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadTransactionsCredits;
            if (string.Equals(value, "ReadTransactionsDebits", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadTransactionsDebits;
            if (string.Equals(value, "ReadTransactionsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsent1DataPermissionsEnum.ReadTransactionsDetail;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadConsent1DataPermissionsEnum value.");
        }
    }
}
