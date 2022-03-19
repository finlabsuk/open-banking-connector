// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public static partial class OBReadBalance1DataBalanceCreditLineTypeEnumExtensions
    {
        public static string ToSerialString(this OBReadBalance1DataBalanceCreditLineTypeEnum value) => value switch
        {
            OBReadBalance1DataBalanceCreditLineTypeEnum.Available => "Available",
            OBReadBalance1DataBalanceCreditLineTypeEnum.Credit => "Credit",
            OBReadBalance1DataBalanceCreditLineTypeEnum.Emergency => "Emergency",
            OBReadBalance1DataBalanceCreditLineTypeEnum.PreAgreed => "Pre-Agreed",
            OBReadBalance1DataBalanceCreditLineTypeEnum.Temporary => "Temporary",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadBalance1DataBalanceCreditLineTypeEnum value.")
        };

        public static OBReadBalance1DataBalanceCreditLineTypeEnum ToOBReadBalance1DataBalanceCreditLineTypeEnum(this string value)
        {
            if (string.Equals(value, "Available", StringComparison.InvariantCultureIgnoreCase)) return OBReadBalance1DataBalanceCreditLineTypeEnum.Available;
            if (string.Equals(value, "Credit", StringComparison.InvariantCultureIgnoreCase)) return OBReadBalance1DataBalanceCreditLineTypeEnum.Credit;
            if (string.Equals(value, "Emergency", StringComparison.InvariantCultureIgnoreCase)) return OBReadBalance1DataBalanceCreditLineTypeEnum.Emergency;
            if (string.Equals(value, "Pre-Agreed", StringComparison.InvariantCultureIgnoreCase)) return OBReadBalance1DataBalanceCreditLineTypeEnum.PreAgreed;
            if (string.Equals(value, "Temporary", StringComparison.InvariantCultureIgnoreCase)) return OBReadBalance1DataBalanceCreditLineTypeEnum.Temporary;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBReadBalance1DataBalanceCreditLineTypeEnum value.");
        }
    }
}
