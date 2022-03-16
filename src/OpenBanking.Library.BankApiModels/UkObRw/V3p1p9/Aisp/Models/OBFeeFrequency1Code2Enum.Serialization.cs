// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    internal static partial class OBFeeFrequency1Code2EnumExtensions
    {
        public static string ToSerialString(this OBFeeFrequency1Code2Enum value) => value switch
        {
            OBFeeFrequency1Code2Enum.Feac => "FEAC",
            OBFeeFrequency1Code2Enum.Feao => "FEAO",
            OBFeeFrequency1Code2Enum.Fecp => "FECP",
            OBFeeFrequency1Code2Enum.Feda => "FEDA",
            OBFeeFrequency1Code2Enum.Feho => "FEHO",
            OBFeeFrequency1Code2Enum.FEI => "FEI",
            OBFeeFrequency1Code2Enum.Femo => "FEMO",
            OBFeeFrequency1Code2Enum.Feoa => "FEOA",
            OBFeeFrequency1Code2Enum.Feot => "FEOT",
            OBFeeFrequency1Code2Enum.Fepc => "FEPC",
            OBFeeFrequency1Code2Enum.Feph => "FEPH",
            OBFeeFrequency1Code2Enum.Fepo => "FEPO",
            OBFeeFrequency1Code2Enum.Feps => "FEPS",
            OBFeeFrequency1Code2Enum.Fept => "FEPT",
            OBFeeFrequency1Code2Enum.Fepta => "FEPTA",
            OBFeeFrequency1Code2Enum.Feptp => "FEPTP",
            OBFeeFrequency1Code2Enum.Fequ => "FEQU",
            OBFeeFrequency1Code2Enum.Fesm => "FESM",
            OBFeeFrequency1Code2Enum.Fest => "FEST",
            OBFeeFrequency1Code2Enum.Fewe => "FEWE",
            OBFeeFrequency1Code2Enum.Feye => "FEYE",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeFrequency1Code2Enum value.")
        };

        public static OBFeeFrequency1Code2Enum ToOBFeeFrequency1Code2Enum(this string value)
        {
            if (string.Equals(value, "FEAC", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feac;
            if (string.Equals(value, "FEAO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feao;
            if (string.Equals(value, "FECP", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fecp;
            if (string.Equals(value, "FEDA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feda;
            if (string.Equals(value, "FEHO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feho;
            if (string.Equals(value, "FEI", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.FEI;
            if (string.Equals(value, "FEMO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Femo;
            if (string.Equals(value, "FEOA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feoa;
            if (string.Equals(value, "FEOT", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feot;
            if (string.Equals(value, "FEPC", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fepc;
            if (string.Equals(value, "FEPH", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feph;
            if (string.Equals(value, "FEPO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fepo;
            if (string.Equals(value, "FEPS", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feps;
            if (string.Equals(value, "FEPT", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fept;
            if (string.Equals(value, "FEPTA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fepta;
            if (string.Equals(value, "FEPTP", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feptp;
            if (string.Equals(value, "FEQU", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fequ;
            if (string.Equals(value, "FESM", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fesm;
            if (string.Equals(value, "FEST", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fest;
            if (string.Equals(value, "FEWE", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Fewe;
            if (string.Equals(value, "FEYE", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code2Enum.Feye;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeFrequency1Code2Enum value.");
        }
    }
}
