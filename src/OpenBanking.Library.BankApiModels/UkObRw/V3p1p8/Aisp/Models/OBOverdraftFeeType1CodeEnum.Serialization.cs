// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace AccountAndTransactionAPISpecification.Models
{
    internal static partial class OBOverdraftFeeType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBOverdraftFeeType1CodeEnum value) => value switch
        {
            OBOverdraftFeeType1CodeEnum.Fbao => "FBAO",
            OBOverdraftFeeType1CodeEnum.Fbar => "FBAR",
            OBOverdraftFeeType1CodeEnum.Fbeb => "FBEB",
            OBOverdraftFeeType1CodeEnum.Fbit => "FBIT",
            OBOverdraftFeeType1CodeEnum.Fbor => "FBOR",
            OBOverdraftFeeType1CodeEnum.Fbos => "FBOS",
            OBOverdraftFeeType1CodeEnum.Fbsc => "FBSC",
            OBOverdraftFeeType1CodeEnum.Fbto => "FBTO",
            OBOverdraftFeeType1CodeEnum.Fbub => "FBUB",
            OBOverdraftFeeType1CodeEnum.Fbut => "FBUT",
            OBOverdraftFeeType1CodeEnum.Ftot => "FTOT",
            OBOverdraftFeeType1CodeEnum.Ftut => "FTUT",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBOverdraftFeeType1CodeEnum value.")
        };

        public static OBOverdraftFeeType1CodeEnum ToOBOverdraftFeeType1CodeEnum(this string value)
        {
            if (string.Equals(value, "FBAO", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbao;
            if (string.Equals(value, "FBAR", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbar;
            if (string.Equals(value, "FBEB", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbeb;
            if (string.Equals(value, "FBIT", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbit;
            if (string.Equals(value, "FBOR", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbor;
            if (string.Equals(value, "FBOS", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbos;
            if (string.Equals(value, "FBSC", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbsc;
            if (string.Equals(value, "FBTO", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbto;
            if (string.Equals(value, "FBUB", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbub;
            if (string.Equals(value, "FBUT", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Fbut;
            if (string.Equals(value, "FTOT", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Ftot;
            if (string.Equals(value, "FTUT", StringComparison.InvariantCultureIgnoreCase)) return OBOverdraftFeeType1CodeEnum.Ftut;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBOverdraftFeeType1CodeEnum value.");
        }
    }
}
