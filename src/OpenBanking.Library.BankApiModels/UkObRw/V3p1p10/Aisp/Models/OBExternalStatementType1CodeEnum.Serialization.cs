// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p10.Aisp.Models
{
    public static partial class OBExternalStatementType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBExternalStatementType1CodeEnum value) => value switch
        {
            OBExternalStatementType1CodeEnum.AccountClosure => "AccountClosure",
            OBExternalStatementType1CodeEnum.AccountOpening => "AccountOpening",
            OBExternalStatementType1CodeEnum.Annual => "Annual",
            OBExternalStatementType1CodeEnum.Interim => "Interim",
            OBExternalStatementType1CodeEnum.RegularPeriodic => "RegularPeriodic",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalStatementType1CodeEnum value.")
        };

        public static OBExternalStatementType1CodeEnum ToOBExternalStatementType1CodeEnum(this string value)
        {
            if (string.Equals(value, "AccountClosure", StringComparison.InvariantCultureIgnoreCase)) return OBExternalStatementType1CodeEnum.AccountClosure;
            if (string.Equals(value, "AccountOpening", StringComparison.InvariantCultureIgnoreCase)) return OBExternalStatementType1CodeEnum.AccountOpening;
            if (string.Equals(value, "Annual", StringComparison.InvariantCultureIgnoreCase)) return OBExternalStatementType1CodeEnum.Annual;
            if (string.Equals(value, "Interim", StringComparison.InvariantCultureIgnoreCase)) return OBExternalStatementType1CodeEnum.Interim;
            if (string.Equals(value, "RegularPeriodic", StringComparison.InvariantCultureIgnoreCase)) return OBExternalStatementType1CodeEnum.RegularPeriodic;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalStatementType1CodeEnum value.");
        }
    }
}
