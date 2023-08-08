// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    internal static partial class OBExternalBalanceSubType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBExternalBalanceSubType1CodeEnum value) => value switch
        {
            OBExternalBalanceSubType1CodeEnum.BaseCurrency => "BaseCurrency",
            OBExternalBalanceSubType1CodeEnum.LocalCurrency => "LocalCurrency",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalBalanceSubType1CodeEnum value.")
        };

        public static OBExternalBalanceSubType1CodeEnum ToOBExternalBalanceSubType1CodeEnum(this string value)
        {
            if (string.Equals(value, "BaseCurrency", StringComparison.InvariantCultureIgnoreCase)) return OBExternalBalanceSubType1CodeEnum.BaseCurrency;
            if (string.Equals(value, "LocalCurrency", StringComparison.InvariantCultureIgnoreCase)) return OBExternalBalanceSubType1CodeEnum.LocalCurrency;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalBalanceSubType1CodeEnum value.");
        }
    }
}