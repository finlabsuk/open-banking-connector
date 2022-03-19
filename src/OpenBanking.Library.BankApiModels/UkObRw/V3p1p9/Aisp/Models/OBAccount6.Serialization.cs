// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBAccount6
    {
        public static OBAccount6 DeserializeOBAccount6(JsonElement element)
        {
            string accountId = default;
            Optional<OBAccountStatus1CodeEnum> status = default;
            Optional<DateTimeOffset> statusUpdateDateTime = default;
            Optional<string> currency = default;
            Optional<OBExternalAccountType1CodeEnum> accountType = default;
            Optional<OBExternalAccountSubType1CodeEnum> accountSubType = default;
            Optional<string> description = default;
            Optional<string> nickname = default;
            Optional<DateTimeOffset> openingDate = default;
            Optional<DateTimeOffset> maturityDate = default;
            Optional<string> switchStatus = default;
            Optional<IReadOnlyList<OBAccount6AccountItem>> account = default;
            Optional<OBBranchAndFinancialInstitutionIdentification50> servicer = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Status"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    status = property.Value.GetString().ToOBAccountStatus1CodeEnum();
                    continue;
                }
                if (property.NameEquals("StatusUpdateDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    statusUpdateDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("Currency"))
                {
                    currency = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("AccountType"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    accountType = property.Value.GetString().ToOBExternalAccountType1CodeEnum();
                    continue;
                }
                if (property.NameEquals("AccountSubType"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    accountSubType = property.Value.GetString().ToOBExternalAccountSubType1CodeEnum();
                    continue;
                }
                if (property.NameEquals("Description"))
                {
                    description = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Nickname"))
                {
                    nickname = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("OpeningDate"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    openingDate = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("MaturityDate"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    maturityDate = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("SwitchStatus"))
                {
                    switchStatus = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Account"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBAccount6AccountItem> array = new List<OBAccount6AccountItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBAccount6AccountItem.DeserializeOBAccount6AccountItem(item));
                    }
                    account = array;
                    continue;
                }
                if (property.NameEquals("Servicer"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    servicer = OBBranchAndFinancialInstitutionIdentification50.DeserializeOBBranchAndFinancialInstitutionIdentification50(property.Value);
                    continue;
                }
            }
            return new OBAccount6(accountId, Optional.ToNullable(status), Optional.ToNullable(statusUpdateDateTime), currency.Value, Optional.ToNullable(accountType), Optional.ToNullable(accountSubType), description.Value, nickname.Value, Optional.ToNullable(openingDate), Optional.ToNullable(maturityDate), switchStatus.Value, Optional.ToList(account), servicer.Value);
        }
    }
}
