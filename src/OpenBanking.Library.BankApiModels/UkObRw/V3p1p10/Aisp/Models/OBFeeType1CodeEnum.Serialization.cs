// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public static partial class OBFeeType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBFeeType1CodeEnum value) => value switch
        {
            OBFeeType1CodeEnum.Fepf => "FEPF",
            OBFeeType1CodeEnum.Ftot => "FTOT",
            OBFeeType1CodeEnum.Fyaf => "FYAF",
            OBFeeType1CodeEnum.Fyam => "FYAM",
            OBFeeType1CodeEnum.Fyaq => "FYAQ",
            OBFeeType1CodeEnum.Fycp => "FYCP",
            OBFeeType1CodeEnum.Fydb => "FYDB",
            OBFeeType1CodeEnum.Fymi => "FYMI",
            OBFeeType1CodeEnum.Fyxx => "FYXX",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeType1CodeEnum value.")
        };

        public static OBFeeType1CodeEnum ToOBFeeType1CodeEnum(this string value)
        {
            if (string.Equals(value, "FEPF", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fepf;
            if (string.Equals(value, "FTOT", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Ftot;
            if (string.Equals(value, "FYAF", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fyaf;
            if (string.Equals(value, "FYAM", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fyam;
            if (string.Equals(value, "FYAQ", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fyaq;
            if (string.Equals(value, "FYCP", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fycp;
            if (string.Equals(value, "FYDB", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fydb;
            if (string.Equals(value, "FYMI", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fymi;
            if (string.Equals(value, "FYXX", StringComparison.InvariantCultureIgnoreCase)) return OBFeeType1CodeEnum.Fyxx;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeType1CodeEnum value.");
        }
    }
}
