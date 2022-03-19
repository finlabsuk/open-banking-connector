// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public static partial class OBBeneficiaryType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBBeneficiaryType1CodeEnum value) => value switch
        {
            OBBeneficiaryType1CodeEnum.Trusted => "Trusted",
            OBBeneficiaryType1CodeEnum.Ordinary => "Ordinary",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBBeneficiaryType1CodeEnum value.")
        };

        public static OBBeneficiaryType1CodeEnum ToOBBeneficiaryType1CodeEnum(this string value)
        {
            if (string.Equals(value, "Trusted", StringComparison.InvariantCultureIgnoreCase)) return OBBeneficiaryType1CodeEnum.Trusted;
            if (string.Equals(value, "Ordinary", StringComparison.InvariantCultureIgnoreCase)) return OBBeneficiaryType1CodeEnum.Ordinary;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBBeneficiaryType1CodeEnum value.");
        }
    }
}
