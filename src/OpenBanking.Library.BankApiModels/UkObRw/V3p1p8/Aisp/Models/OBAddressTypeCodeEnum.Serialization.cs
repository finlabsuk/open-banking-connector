// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace AccountAndTransactionAPISpecification.Models
{
    internal static partial class OBAddressTypeCodeEnumExtensions
    {
        public static string ToSerialString(this OBAddressTypeCodeEnum value) => value switch
        {
            OBAddressTypeCodeEnum.Business => "Business",
            OBAddressTypeCodeEnum.Correspondence => "Correspondence",
            OBAddressTypeCodeEnum.DeliveryTo => "DeliveryTo",
            OBAddressTypeCodeEnum.MailTo => "MailTo",
            OBAddressTypeCodeEnum.POBox => "POBox",
            OBAddressTypeCodeEnum.Postal => "Postal",
            OBAddressTypeCodeEnum.Residential => "Residential",
            OBAddressTypeCodeEnum.Statement => "Statement",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBAddressTypeCodeEnum value.")
        };

        public static OBAddressTypeCodeEnum ToOBAddressTypeCodeEnum(this string value)
        {
            if (string.Equals(value, "Business", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.Business;
            if (string.Equals(value, "Correspondence", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.Correspondence;
            if (string.Equals(value, "DeliveryTo", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.DeliveryTo;
            if (string.Equals(value, "MailTo", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.MailTo;
            if (string.Equals(value, "POBox", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.POBox;
            if (string.Equals(value, "Postal", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.Postal;
            if (string.Equals(value, "Residential", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.Residential;
            if (string.Equals(value, "Statement", StringComparison.InvariantCultureIgnoreCase)) return OBAddressTypeCodeEnum.Statement;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBAddressTypeCodeEnum value.");
        }
    }
}
