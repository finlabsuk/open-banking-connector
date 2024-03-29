// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public static partial class OBInterestFixedVariableType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBInterestFixedVariableType1CodeEnum value) => value switch
        {
            OBInterestFixedVariableType1CodeEnum.Infi => "INFI",
            OBInterestFixedVariableType1CodeEnum.Inva => "INVA",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBInterestFixedVariableType1CodeEnum value.")
        };

        public static OBInterestFixedVariableType1CodeEnum ToOBInterestFixedVariableType1CodeEnum(this string value)
        {
            if (string.Equals(value, "INFI", StringComparison.InvariantCultureIgnoreCase)) return OBInterestFixedVariableType1CodeEnum.Infi;
            if (string.Equals(value, "INVA", StringComparison.InvariantCultureIgnoreCase)) return OBInterestFixedVariableType1CodeEnum.Inva;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBInterestFixedVariableType1CodeEnum value.");
        }
    }
}
