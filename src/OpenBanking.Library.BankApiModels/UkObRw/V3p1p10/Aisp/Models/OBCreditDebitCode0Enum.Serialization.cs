// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public static partial class OBCreditDebitCode0EnumExtensions
    {
        public static string ToSerialString(this OBCreditDebitCode0Enum value) => value switch
        {
            OBCreditDebitCode0Enum.Credit => "Credit",
            OBCreditDebitCode0Enum.Debit => "Debit",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBCreditDebitCode0Enum value.")
        };

        public static OBCreditDebitCode0Enum ToOBCreditDebitCode0Enum(this string value)
        {
            if (string.Equals(value, "Credit", StringComparison.InvariantCultureIgnoreCase)) return OBCreditDebitCode0Enum.Credit;
            if (string.Equals(value, "Debit", StringComparison.InvariantCultureIgnoreCase)) return OBCreditDebitCode0Enum.Debit;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBCreditDebitCode0Enum value.");
        }
    }
}
