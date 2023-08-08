// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p11.Aisp.Models
{
    internal static partial class OBFeeFrequency1Code0EnumExtensions
    {
        public static string ToSerialString(this OBFeeFrequency1Code0Enum value) => value switch
        {
            OBFeeFrequency1Code0Enum.Feac => "FEAC",
            OBFeeFrequency1Code0Enum.Feao => "FEAO",
            OBFeeFrequency1Code0Enum.Fecp => "FECP",
            OBFeeFrequency1Code0Enum.Feda => "FEDA",
            OBFeeFrequency1Code0Enum.Feho => "FEHO",
            OBFeeFrequency1Code0Enum.FEI => "FEI",
            OBFeeFrequency1Code0Enum.Femo => "FEMO",
            OBFeeFrequency1Code0Enum.Feoa => "FEOA",
            OBFeeFrequency1Code0Enum.Feot => "FEOT",
            OBFeeFrequency1Code0Enum.Fepc => "FEPC",
            OBFeeFrequency1Code0Enum.Feph => "FEPH",
            OBFeeFrequency1Code0Enum.Fepo => "FEPO",
            OBFeeFrequency1Code0Enum.Feps => "FEPS",
            OBFeeFrequency1Code0Enum.Fept => "FEPT",
            OBFeeFrequency1Code0Enum.Fepta => "FEPTA",
            OBFeeFrequency1Code0Enum.Feptp => "FEPTP",
            OBFeeFrequency1Code0Enum.Fequ => "FEQU",
            OBFeeFrequency1Code0Enum.Fesm => "FESM",
            OBFeeFrequency1Code0Enum.Fest => "FEST",
            OBFeeFrequency1Code0Enum.Fewe => "FEWE",
            OBFeeFrequency1Code0Enum.Feye => "FEYE",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeFrequency1Code0Enum value.")
        };

        public static OBFeeFrequency1Code0Enum ToOBFeeFrequency1Code0Enum(this string value)
        {
            if (string.Equals(value, "FEAC", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feac;
            if (string.Equals(value, "FEAO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feao;
            if (string.Equals(value, "FECP", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fecp;
            if (string.Equals(value, "FEDA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feda;
            if (string.Equals(value, "FEHO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feho;
            if (string.Equals(value, "FEI", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.FEI;
            if (string.Equals(value, "FEMO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Femo;
            if (string.Equals(value, "FEOA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feoa;
            if (string.Equals(value, "FEOT", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feot;
            if (string.Equals(value, "FEPC", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fepc;
            if (string.Equals(value, "FEPH", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feph;
            if (string.Equals(value, "FEPO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fepo;
            if (string.Equals(value, "FEPS", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feps;
            if (string.Equals(value, "FEPT", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fept;
            if (string.Equals(value, "FEPTA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fepta;
            if (string.Equals(value, "FEPTP", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feptp;
            if (string.Equals(value, "FEQU", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fequ;
            if (string.Equals(value, "FESM", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fesm;
            if (string.Equals(value, "FEST", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fest;
            if (string.Equals(value, "FEWE", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Fewe;
            if (string.Equals(value, "FEYE", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code0Enum.Feye;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeFrequency1Code0Enum value.");
        }
    }
}