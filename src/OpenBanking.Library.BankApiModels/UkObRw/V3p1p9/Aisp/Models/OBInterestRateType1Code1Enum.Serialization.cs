// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    internal static partial class OBInterestRateType1Code1EnumExtensions
    {
        public static string ToSerialString(this OBInterestRateType1Code1Enum value) => value switch
        {
            OBInterestRateType1Code1Enum.Inbb => "INBB",
            OBInterestRateType1Code1Enum.Infr => "INFR",
            OBInterestRateType1Code1Enum.Ingr => "INGR",
            OBInterestRateType1Code1Enum.Inlr => "INLR",
            OBInterestRateType1Code1Enum.Inne => "INNE",
            OBInterestRateType1Code1Enum.Inot => "INOT",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBInterestRateType1Code1Enum value.")
        };

        public static OBInterestRateType1Code1Enum ToOBInterestRateType1Code1Enum(this string value)
        {
            if (string.Equals(value, "INBB", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code1Enum.Inbb;
            if (string.Equals(value, "INFR", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code1Enum.Infr;
            if (string.Equals(value, "INGR", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code1Enum.Ingr;
            if (string.Equals(value, "INLR", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code1Enum.Inlr;
            if (string.Equals(value, "INNE", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code1Enum.Inne;
            if (string.Equals(value, "INOT", StringComparison.InvariantCultureIgnoreCase)) return OBInterestRateType1Code1Enum.Inot;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBInterestRateType1Code1Enum value.");
        }
    }
}
