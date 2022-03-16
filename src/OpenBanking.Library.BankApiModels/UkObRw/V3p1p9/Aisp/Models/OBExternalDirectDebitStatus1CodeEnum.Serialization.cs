// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    internal static partial class OBExternalDirectDebitStatus1CodeEnumExtensions
    {
        public static string ToSerialString(this OBExternalDirectDebitStatus1CodeEnum value) => value switch
        {
            OBExternalDirectDebitStatus1CodeEnum.Active => "Active",
            OBExternalDirectDebitStatus1CodeEnum.Inactive => "Inactive",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalDirectDebitStatus1CodeEnum value.")
        };

        public static OBExternalDirectDebitStatus1CodeEnum ToOBExternalDirectDebitStatus1CodeEnum(this string value)
        {
            if (string.Equals(value, "Active", StringComparison.InvariantCultureIgnoreCase)) return OBExternalDirectDebitStatus1CodeEnum.Active;
            if (string.Equals(value, "Inactive", StringComparison.InvariantCultureIgnoreCase)) return OBExternalDirectDebitStatus1CodeEnum.Inactive;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalDirectDebitStatus1CodeEnum value.");
        }
    }
}
