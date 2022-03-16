// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace AccountAndTransactionAPISpecification.Models
{
    internal static partial class OBInterestRateType1Code0EnumExtensions
    {
        public static string ToSerialString(this OBInterestRateType1Code0Enum value) => value switch
        {
            OBInterestRateType1Code0Enum.Inbb => "INBB",
            OBInterestRateType1Code0Enum.Infr => "INFR",
            OBInterestRateType1Code0Enum.Ingr => "INGR",
            OBInterestRateType1Code0Enum.Inlr => "INLR",
            OBInterestRateType1Code0Enum.Inne => "INNE",
            OBInterestRateType1Code0Enum.Inot => "INOT",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBInterestRateType1Code0Enum value.")
        };

        public static OBInterestRateType1Code0Enum ToOBInterestRateType1Code0Enum(this string value)
        {
            if (string.Equals(value, "INBB", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code0Enum.Inbb;
            if (string.Equals(value, "INFR", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code0Enum.Infr;
            if (string.Equals(value, "INGR", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code0Enum.Ingr;
            if (string.Equals(value, "INLR", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code0Enum.Inlr;
            if (string.Equals(value, "INNE", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code0Enum.Inne;
            if (string.Equals(value, "INOT", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code0Enum.Inot;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBInterestRateType1Code0Enum value.");
        }
    }
}
