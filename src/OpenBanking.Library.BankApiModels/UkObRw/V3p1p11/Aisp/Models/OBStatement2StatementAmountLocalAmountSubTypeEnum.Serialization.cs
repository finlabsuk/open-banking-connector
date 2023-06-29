// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    internal static partial class OBStatement2StatementAmountLocalAmountSubTypeEnumExtensions
    {
        public static string ToSerialString(this OBStatement2StatementAmountLocalAmountSubTypeEnum value) => value switch
        {
            OBStatement2StatementAmountLocalAmountSubTypeEnum.BaseCurrency => "BaseCurrency",
            OBStatement2StatementAmountLocalAmountSubTypeEnum.LocalCurrency => "LocalCurrency",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBStatement2StatementAmountLocalAmountSubTypeEnum value.")
        };

        public static OBStatement2StatementAmountLocalAmountSubTypeEnum ToOBStatement2StatementAmountLocalAmountSubTypeEnum(this string value)
        {
            if (string.Equals(value, "BaseCurrency", StringComparison.InvariantCultureIgnoreCase)) return OBStatement2StatementAmountLocalAmountSubTypeEnum.BaseCurrency;
            if (string.Equals(value, "LocalCurrency", StringComparison.InvariantCultureIgnoreCase)) return OBStatement2StatementAmountLocalAmountSubTypeEnum.LocalCurrency;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBStatement2StatementAmountLocalAmountSubTypeEnum value.");
        }
    }
}
