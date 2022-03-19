// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public static partial class OBTransactionCardInstrument1AuthorisationTypeEnumExtensions
    {
        public static string ToSerialString(this OBTransactionCardInstrument1AuthorisationTypeEnum value) => value switch
        {
            OBTransactionCardInstrument1AuthorisationTypeEnum.ConsumerDevice => "ConsumerDevice",
            OBTransactionCardInstrument1AuthorisationTypeEnum.Contactless => "Contactless",
            OBTransactionCardInstrument1AuthorisationTypeEnum.None => "None",
            OBTransactionCardInstrument1AuthorisationTypeEnum.PIN => "PIN",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBTransactionCardInstrument1AuthorisationTypeEnum value.")
        };

        public static OBTransactionCardInstrument1AuthorisationTypeEnum ToOBTransactionCardInstrument1AuthorisationTypeEnum(this string value)
        {
            if (string.Equals(value, "ConsumerDevice", StringComparison.InvariantCultureIgnoreCase)) return OBTransactionCardInstrument1AuthorisationTypeEnum.ConsumerDevice;
            if (string.Equals(value, "Contactless", StringComparison.InvariantCultureIgnoreCase)) return OBTransactionCardInstrument1AuthorisationTypeEnum.Contactless;
            if (string.Equals(value, "None", StringComparison.InvariantCultureIgnoreCase)) return OBTransactionCardInstrument1AuthorisationTypeEnum.None;
            if (string.Equals(value, "PIN", StringComparison.InvariantCultureIgnoreCase)) return OBTransactionCardInstrument1AuthorisationTypeEnum.PIN;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBTransactionCardInstrument1AuthorisationTypeEnum value.");
        }
    }
}
