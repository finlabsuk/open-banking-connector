// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    internal static partial class OBStatement2StatementAmountSubTypeEnumExtensions
    {
        public static string ToSerialString(this OBStatement2StatementAmountSubTypeEnum value) => value switch
        {
            OBStatement2StatementAmountSubTypeEnum.BaseCurrency => "BaseCurrency",
            OBStatement2StatementAmountSubTypeEnum.LocalCurrency => "LocalCurrency",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBStatement2StatementAmountSubTypeEnum value.")
        };

        public static OBStatement2StatementAmountSubTypeEnum ToOBStatement2StatementAmountSubTypeEnum(this string value)
        {
            if (string.Equals(value, "BaseCurrency", StringComparison.InvariantCultureIgnoreCase)) return OBStatement2StatementAmountSubTypeEnum.BaseCurrency;
            if (string.Equals(value, "LocalCurrency", StringComparison.InvariantCultureIgnoreCase)) return OBStatement2StatementAmountSubTypeEnum.LocalCurrency;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBStatement2StatementAmountSubTypeEnum value.");
        }
    }
}
