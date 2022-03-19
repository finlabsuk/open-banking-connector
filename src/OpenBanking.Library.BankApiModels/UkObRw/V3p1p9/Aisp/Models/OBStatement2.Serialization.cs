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
    public partial class OBStatement2
    {
        public static OBStatement2 DeserializeOBStatement2(JsonElement element)
        {
            string accountId = default;
            Optional<string> statementId = default;
            Optional<string> statementReference = default;
            OBExternalStatementType1CodeEnum type = default;
            DateTimeOffset startDateTime = default;
            DateTimeOffset endDateTime = default;
            DateTimeOffset creationDateTime = default;
            Optional<IReadOnlyList<string>> statementDescription = default;
            Optional<IReadOnlyList<OBStatement2StatementBenefitItem>> statementBenefit = default;
            Optional<IReadOnlyList<OBStatement2StatementFeeItem>> statementFee = default;
            Optional<IReadOnlyList<OBStatement2StatementInterestItem>> statementInterest = default;
            Optional<IReadOnlyList<OBStatement2StatementAmountItem>> statementAmount = default;
            Optional<IReadOnlyList<OBStatement2StatementDateTimeItem>> statementDateTime = default;
            Optional<IReadOnlyList<OBStatement2StatementRateItem>> statementRate = default;
            Optional<IReadOnlyList<OBStatement2StatementValueItem>> statementValue = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("StatementId"))
                {
                    statementId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("StatementReference"))
                {
                    statementReference = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Type"))
                {
                    type = property.Value.GetString().ToOBExternalStatementType1CodeEnum();
                    continue;
                }
                if (property.NameEquals("StartDateTime"))
                {
                    startDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("EndDateTime"))
                {
                    endDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("CreationDateTime"))
                {
                    creationDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("StatementDescription"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<string> array = new List<string>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(item.GetString());
                    }
                    statementDescription = array;
                    continue;
                }
                if (property.NameEquals("StatementBenefit"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2StatementBenefitItem> array = new List<OBStatement2StatementBenefitItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2StatementBenefitItem.DeserializeOBStatement2StatementBenefitItem(item));
                    }
                    statementBenefit = array;
                    continue;
                }
                if (property.NameEquals("StatementFee"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2StatementFeeItem> array = new List<OBStatement2StatementFeeItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2StatementFeeItem.DeserializeOBStatement2StatementFeeItem(item));
                    }
                    statementFee = array;
                    continue;
                }
                if (property.NameEquals("StatementInterest"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2StatementInterestItem> array = new List<OBStatement2StatementInterestItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2StatementInterestItem.DeserializeOBStatement2StatementInterestItem(item));
                    }
                    statementInterest = array;
                    continue;
                }
                if (property.NameEquals("StatementAmount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2StatementAmountItem> array = new List<OBStatement2StatementAmountItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2StatementAmountItem.DeserializeOBStatement2StatementAmountItem(item));
                    }
                    statementAmount = array;
                    continue;
                }
                if (property.NameEquals("StatementDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2StatementDateTimeItem> array = new List<OBStatement2StatementDateTimeItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2StatementDateTimeItem.DeserializeOBStatement2StatementDateTimeItem(item));
                    }
                    statementDateTime = array;
                    continue;
                }
                if (property.NameEquals("StatementRate"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2StatementRateItem> array = new List<OBStatement2StatementRateItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2StatementRateItem.DeserializeOBStatement2StatementRateItem(item));
                    }
                    statementRate = array;
                    continue;
                }
                if (property.NameEquals("StatementValue"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    List<OBStatement2StatementValueItem> array = new List<OBStatement2StatementValueItem>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(OBStatement2StatementValueItem.DeserializeOBStatement2StatementValueItem(item));
                    }
                    statementValue = array;
                    continue;
                }
            }
            return new OBStatement2(accountId, statementId.Value, statementReference.Value, type, startDateTime, endDateTime, creationDateTime, Optional.ToList(statementDescription), Optional.ToList(statementBenefit), Optional.ToList(statementFee), Optional.ToList(statementInterest), Optional.ToList(statementAmount), Optional.ToList(statementDateTime), Optional.ToList(statementRate), Optional.ToList(statementValue));
        }
    }
}
