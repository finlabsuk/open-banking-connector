// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    internal static partial class OBTransactionMutability1CodeEnumExtensions
    {
        public static string ToSerialString(this OBTransactionMutability1CodeEnum value) => value switch
        {
            OBTransactionMutability1CodeEnum.Mutable => "Mutable",
            OBTransactionMutability1CodeEnum.Immutable => "Immutable",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBTransactionMutability1CodeEnum value.")
        };

        public static OBTransactionMutability1CodeEnum ToOBTransactionMutability1CodeEnum(this string value)
        {
            if (string.Equals(value, "Mutable", StringComparison.InvariantCultureIgnoreCase)) return OBTransactionMutability1CodeEnum.Mutable;
            if (string.Equals(value, "Immutable", StringComparison.InvariantCultureIgnoreCase)) return OBTransactionMutability1CodeEnum.Immutable;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBTransactionMutability1CodeEnum value.");
        }
    }
}
