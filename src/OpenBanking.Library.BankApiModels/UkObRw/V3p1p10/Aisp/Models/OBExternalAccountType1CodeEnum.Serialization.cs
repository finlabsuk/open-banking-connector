// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public static partial class OBExternalAccountType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBExternalAccountType1CodeEnum value) => value switch
        {
            OBExternalAccountType1CodeEnum.Business => "Business",
            OBExternalAccountType1CodeEnum.Personal => "Personal",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalAccountType1CodeEnum value.")
        };

        public static OBExternalAccountType1CodeEnum ToOBExternalAccountType1CodeEnum(this string value)
        {
            if (string.Equals(value, "Business", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountType1CodeEnum.Business;
            if (string.Equals(value, "Personal", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountType1CodeEnum.Personal;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalAccountType1CodeEnum value.");
        }
    }
}
