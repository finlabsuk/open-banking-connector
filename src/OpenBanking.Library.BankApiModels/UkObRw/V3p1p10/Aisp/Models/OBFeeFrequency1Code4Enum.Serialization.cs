// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public static partial class OBFeeFrequency1Code4EnumExtensions
    {
        public static string ToSerialString(this OBFeeFrequency1Code4Enum value) => value switch
        {
            OBFeeFrequency1Code4Enum.Feac => "FEAC",
            OBFeeFrequency1Code4Enum.Feao => "FEAO",
            OBFeeFrequency1Code4Enum.Fecp => "FECP",
            OBFeeFrequency1Code4Enum.Feda => "FEDA",
            OBFeeFrequency1Code4Enum.Feho => "FEHO",
            OBFeeFrequency1Code4Enum.FEI => "FEI",
            OBFeeFrequency1Code4Enum.Femo => "FEMO",
            OBFeeFrequency1Code4Enum.Feoa => "FEOA",
            OBFeeFrequency1Code4Enum.Feot => "FEOT",
            OBFeeFrequency1Code4Enum.Fepc => "FEPC",
            OBFeeFrequency1Code4Enum.Feph => "FEPH",
            OBFeeFrequency1Code4Enum.Fepo => "FEPO",
            OBFeeFrequency1Code4Enum.Feps => "FEPS",
            OBFeeFrequency1Code4Enum.Fept => "FEPT",
            OBFeeFrequency1Code4Enum.Fepta => "FEPTA",
            OBFeeFrequency1Code4Enum.Feptp => "FEPTP",
            OBFeeFrequency1Code4Enum.Fequ => "FEQU",
            OBFeeFrequency1Code4Enum.Fesm => "FESM",
            OBFeeFrequency1Code4Enum.Fest => "FEST",
            OBFeeFrequency1Code4Enum.Fewe => "FEWE",
            OBFeeFrequency1Code4Enum.Feye => "FEYE",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeFrequency1Code4Enum value.")
        };

        public static OBFeeFrequency1Code4Enum ToOBFeeFrequency1Code4Enum(this string value)
        {
            if (string.Equals(value, "FEAC", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feac;
            if (string.Equals(value, "FEAO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feao;
            if (string.Equals(value, "FECP", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fecp;
            if (string.Equals(value, "FEDA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feda;
            if (string.Equals(value, "FEHO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feho;
            if (string.Equals(value, "FEI", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.FEI;
            if (string.Equals(value, "FEMO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Femo;
            if (string.Equals(value, "FEOA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feoa;
            if (string.Equals(value, "FEOT", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feot;
            if (string.Equals(value, "FEPC", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fepc;
            if (string.Equals(value, "FEPH", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feph;
            if (string.Equals(value, "FEPO", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fepo;
            if (string.Equals(value, "FEPS", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feps;
            if (string.Equals(value, "FEPT", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fept;
            if (string.Equals(value, "FEPTA", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fepta;
            if (string.Equals(value, "FEPTP", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feptp;
            if (string.Equals(value, "FEQU", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fequ;
            if (string.Equals(value, "FESM", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fesm;
            if (string.Equals(value, "FEST", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fest;
            if (string.Equals(value, "FEWE", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Fewe;
            if (string.Equals(value, "FEYE", StringComparison.InvariantCultureIgnoreCase)) return OBFeeFrequency1Code4Enum.Feye;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBFeeFrequency1Code4Enum value.");
        }
    }
}
