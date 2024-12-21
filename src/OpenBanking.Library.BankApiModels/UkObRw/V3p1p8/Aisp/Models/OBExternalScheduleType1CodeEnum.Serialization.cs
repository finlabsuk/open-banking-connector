// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public static partial class OBExternalScheduleType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBExternalScheduleType1CodeEnum value) => value switch
        {
            OBExternalScheduleType1CodeEnum.Arrival => "Arrival",
            OBExternalScheduleType1CodeEnum.Execution => "Execution",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalScheduleType1CodeEnum value.")
        };

        public static OBExternalScheduleType1CodeEnum ToOBExternalScheduleType1CodeEnum(this string value)
        {
            if (string.Equals(value, "Arrival", StringComparison.InvariantCultureIgnoreCase)) return OBExternalScheduleType1CodeEnum.Arrival;
            if (string.Equals(value, "Execution", StringComparison.InvariantCultureIgnoreCase)) return OBExternalScheduleType1CodeEnum.Execution;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalScheduleType1CodeEnum value.");
        }
    }
}