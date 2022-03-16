// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    internal static partial class OBCreditDebitCode2EnumExtensions
    {
        public static string ToSerialString(this OBCreditDebitCode2Enum value) => value switch
        {
            OBCreditDebitCode2Enum.Credit => "Credit",
            OBCreditDebitCode2Enum.Debit => "Debit",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBCreditDebitCode2Enum value.")
        };

        public static OBCreditDebitCode2Enum ToOBCreditDebitCode2Enum(this string value)
        {
            if (string.Equals(value, "Credit", StringComparison.InvariantCultureIgnoreCase)) return OBCreditDebitCode2Enum.Credit;
            if (string.Equals(value, "Debit", StringComparison.InvariantCultureIgnoreCase)) return OBCreditDebitCode2Enum.Debit;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBCreditDebitCode2Enum value.");
        }
    }
}
