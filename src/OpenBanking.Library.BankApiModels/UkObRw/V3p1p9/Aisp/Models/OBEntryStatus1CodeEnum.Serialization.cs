// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public static partial class OBEntryStatus1CodeEnumExtensions
    {
        public static string ToSerialString(this OBEntryStatus1CodeEnum value) => value switch
        {
            OBEntryStatus1CodeEnum.Booked => "Booked",
            OBEntryStatus1CodeEnum.Pending => "Pending",
            OBEntryStatus1CodeEnum.Rejected => "Rejected",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBEntryStatus1CodeEnum value.")
        };

        public static OBEntryStatus1CodeEnum ToOBEntryStatus1CodeEnum(this string value)
        {
            if (string.Equals(value, "Booked", StringComparison.InvariantCultureIgnoreCase)) return OBEntryStatus1CodeEnum.Booked;
            if (string.Equals(value, "Pending", StringComparison.InvariantCultureIgnoreCase)) return OBEntryStatus1CodeEnum.Pending;
            if (string.Equals(value, "Rejected", StringComparison.InvariantCultureIgnoreCase)) return OBEntryStatus1CodeEnum.Rejected;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBEntryStatus1CodeEnum value.");
        }
    }
}