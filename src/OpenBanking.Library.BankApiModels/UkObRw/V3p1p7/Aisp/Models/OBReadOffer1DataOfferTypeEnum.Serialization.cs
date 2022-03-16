// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    internal static partial class OBReadOffer1DataOfferTypeEnumExtensions
    {
        public static string ToSerialString(this OBReadOffer1DataOfferTypeEnum value) => value switch
        {
            OBReadOffer1DataOfferTypeEnum.BalanceTransfer => "BalanceTransfer",
            OBReadOffer1DataOfferTypeEnum.LimitIncrease => "LimitIncrease",
            OBReadOffer1DataOfferTypeEnum.MoneyTransfer => "MoneyTransfer",
            OBReadOffer1DataOfferTypeEnum.Other => "Other",
            OBReadOffer1DataOfferTypeEnum.PromotionalRate => "PromotionalRate",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadOffer1DataOfferTypeEnum value.")
        };

        public static OBReadOffer1DataOfferTypeEnum ToOBReadOffer1DataOfferTypeEnum(this string value)
        {
            if (string.Equals(value, "BalanceTransfer", StringComparison.InvariantCultureIgnoreCase)) return OBReadOffer1DataOfferTypeEnum.BalanceTransfer;
            if (string.Equals(value, "LimitIncrease", StringComparison.InvariantCultureIgnoreCase)) return OBReadOffer1DataOfferTypeEnum.LimitIncrease;
            if (string.Equals(value, "MoneyTransfer", StringComparison.InvariantCultureIgnoreCase)) return OBReadOffer1DataOfferTypeEnum.MoneyTransfer;
            if (string.Equals(value, "Other", StringComparison.InvariantCultureIgnoreCase)) return OBReadOffer1DataOfferTypeEnum.Other;
            if (string.Equals(value, "PromotionalRate", StringComparison.InvariantCultureIgnoreCase)) return OBReadOffer1DataOfferTypeEnum.PromotionalRate;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadOffer1DataOfferTypeEnum value.");
        }
    }
}
