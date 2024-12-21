// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public static partial class OBExternalAccountSubType1CodeEnumExtensions
    {
        public static string ToSerialString(this OBExternalAccountSubType1CodeEnum value) => value switch
        {
            OBExternalAccountSubType1CodeEnum.ChargeCard => "ChargeCard",
            OBExternalAccountSubType1CodeEnum.CreditCard => "CreditCard",
            OBExternalAccountSubType1CodeEnum.CurrentAccount => "CurrentAccount",
            OBExternalAccountSubType1CodeEnum.EMoney => "EMoney",
            OBExternalAccountSubType1CodeEnum.Loan => "Loan",
            OBExternalAccountSubType1CodeEnum.Mortgage => "Mortgage",
            OBExternalAccountSubType1CodeEnum.PrePaidCard => "PrePaidCard",
            OBExternalAccountSubType1CodeEnum.Savings => "Savings",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalAccountSubType1CodeEnum value.")
        };

        public static OBExternalAccountSubType1CodeEnum ToOBExternalAccountSubType1CodeEnum(this string value)
        {
            if (string.Equals(value, "ChargeCard", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.ChargeCard;
            if (string.Equals(value, "CreditCard", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.CreditCard;
            if (string.Equals(value, "CurrentAccount", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.CurrentAccount;
            if (string.Equals(value, "EMoney", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.EMoney;
            if (string.Equals(value, "Loan", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.Loan;
            if (string.Equals(value, "Mortgage", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.Mortgage;
            if (string.Equals(value, "PrePaidCard", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.PrePaidCard;
            if (string.Equals(value, "Savings", StringComparison.InvariantCultureIgnoreCase)) return OBExternalAccountSubType1CodeEnum.Savings;
            throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown OBExternalAccountSubType1CodeEnum value.");
        }
    }
}