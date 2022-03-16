// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    internal static partial class OBCreditDebitCode1EnumExtensions
    {
        public static string ToSerialString(this OBCreditDebitCode1Enum value) => value switch
        {
            OBCreditDebitCode1Enum.Credit => "Credit",
            OBCreditDebitCode1Enum.Debit => "Debit",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBCreditDebitCode1Enum value.")
        };

        public static OBCreditDebitCode1Enum ToOBCreditDebitCode1Enum(this string value)
        {
            if (string.Equals(value, "Credit", StringComparison.InvariantCultureIgnoreCase)) return OBCreditDebitCode1Enum.Credit;
            if (string.Equals(value, "Debit", StringComparison.InvariantCultureIgnoreCase)) return OBCreditDebitCode1Enum.Debit;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBCreditDebitCode1Enum value.");
        }
    }
}
