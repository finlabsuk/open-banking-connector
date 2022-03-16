// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    internal static partial class OBReadConsentResponse1DataPermissionsEnumExtensions
    {
        public static string ToSerialString(this OBReadConsentResponse1DataPermissionsEnum value) => value switch
        {
            OBReadConsentResponse1DataPermissionsEnum.ReadAccountsBasic => "ReadAccountsBasic",
            OBReadConsentResponse1DataPermissionsEnum.ReadAccountsDetail => "ReadAccountsDetail",
            OBReadConsentResponse1DataPermissionsEnum.ReadBalances => "ReadBalances",
            OBReadConsentResponse1DataPermissionsEnum.ReadBeneficiariesBasic => "ReadBeneficiariesBasic",
            OBReadConsentResponse1DataPermissionsEnum.ReadBeneficiariesDetail => "ReadBeneficiariesDetail",
            OBReadConsentResponse1DataPermissionsEnum.ReadDirectDebits => "ReadDirectDebits",
            OBReadConsentResponse1DataPermissionsEnum.ReadOffers => "ReadOffers",
            OBReadConsentResponse1DataPermissionsEnum.ReadPAN => "ReadPAN",
            OBReadConsentResponse1DataPermissionsEnum.ReadParty => "ReadParty",
            OBReadConsentResponse1DataPermissionsEnum.ReadPartyPSU => "ReadPartyPSU",
            OBReadConsentResponse1DataPermissionsEnum.ReadProducts => "ReadProducts",
            OBReadConsentResponse1DataPermissionsEnum.ReadScheduledPaymentsBasic => "ReadScheduledPaymentsBasic",
            OBReadConsentResponse1DataPermissionsEnum.ReadScheduledPaymentsDetail => "ReadScheduledPaymentsDetail",
            OBReadConsentResponse1DataPermissionsEnum.ReadStandingOrdersBasic => "ReadStandingOrdersBasic",
            OBReadConsentResponse1DataPermissionsEnum.ReadStandingOrdersDetail => "ReadStandingOrdersDetail",
            OBReadConsentResponse1DataPermissionsEnum.ReadStatementsBasic => "ReadStatementsBasic",
            OBReadConsentResponse1DataPermissionsEnum.ReadStatementsDetail => "ReadStatementsDetail",
            OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsBasic => "ReadTransactionsBasic",
            OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsCredits => "ReadTransactionsCredits",
            OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsDebits => "ReadTransactionsDebits",
            OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsDetail => "ReadTransactionsDetail",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadConsentResponse1DataPermissionsEnum value.")
        };

        public static OBReadConsentResponse1DataPermissionsEnum ToOBReadConsentResponse1DataPermissionsEnum(this string value)
        {
            if (string.Equals(value, "ReadAccountsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadAccountsBasic;
            if (string.Equals(value, "ReadAccountsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadAccountsDetail;
            if (string.Equals(value, "ReadBalances", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadBalances;
            if (string.Equals(value, "ReadBeneficiariesBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadBeneficiariesBasic;
            if (string.Equals(value, "ReadBeneficiariesDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadBeneficiariesDetail;
            if (string.Equals(value, "ReadDirectDebits", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadDirectDebits;
            if (string.Equals(value, "ReadOffers", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadOffers;
            if (string.Equals(value, "ReadPAN", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadPAN;
            if (string.Equals(value, "ReadParty", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadParty;
            if (string.Equals(value, "ReadPartyPSU", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadPartyPSU;
            if (string.Equals(value, "ReadProducts", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadProducts;
            if (string.Equals(value, "ReadScheduledPaymentsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadScheduledPaymentsBasic;
            if (string.Equals(value, "ReadScheduledPaymentsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadScheduledPaymentsDetail;
            if (string.Equals(value, "ReadStandingOrdersBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadStandingOrdersBasic;
            if (string.Equals(value, "ReadStandingOrdersDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadStandingOrdersDetail;
            if (string.Equals(value, "ReadStatementsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadStatementsBasic;
            if (string.Equals(value, "ReadStatementsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadStatementsDetail;
            if (string.Equals(value, "ReadTransactionsBasic", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsBasic;
            if (string.Equals(value, "ReadTransactionsCredits", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsCredits;
            if (string.Equals(value, "ReadTransactionsDebits", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsDebits;
            if (string.Equals(value, "ReadTransactionsDetail", StringComparison.InvariantCultureIgnoreCase)) return OBReadConsentResponse1DataPermissionsEnum.ReadTransactionsDetail;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadConsentResponse1DataPermissionsEnum value.");
        }
    }
}
