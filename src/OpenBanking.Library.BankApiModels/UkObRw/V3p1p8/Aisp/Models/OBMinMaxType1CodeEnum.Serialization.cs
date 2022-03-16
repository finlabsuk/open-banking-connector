// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace AccountAndTransactionAPISpecification.Models
{
    internal static partial class OBMinMaxType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBMinMaxType1CodeEnum value) => value switch
        {
            OBMinMaxType1CodeEnum.Fmmn => "FMMN",
            OBMinMaxType1CodeEnum.Fmmx => "FMMX",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBMinMaxType1CodeEnum value.")
        };

        public static OBMinMaxType1CodeEnum ToOBMinMaxType1CodeEnum(this string value)
        {
            if (string.Equals(value, "FMMN", StringComparison.InvariantCultureIgnoreCase)) return OBMinMaxType1CodeEnum.Fmmn;
            if (string.Equals(value, "FMMX", StringComparison.InvariantCultureIgnoreCase)) return OBMinMaxType1CodeEnum.Fmmx;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBMinMaxType1CodeEnum value.");
        }
    }
}
