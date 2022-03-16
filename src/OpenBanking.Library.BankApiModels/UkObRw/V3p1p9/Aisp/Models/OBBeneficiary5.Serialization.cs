// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBBeneficiary5
    {
        internal static OBBeneficiary5 DeserializeOBBeneficiary5(JsonElement element)
        {
            Optional<string> accountId = default;
            Optional<string> beneficiaryId = default;
            Optional<OBBeneficiaryType1CodeEnum> beneficiaryType = default;
            Optional<string> reference = default;
            Optional<IReadOnlyDictionary<string, object>> supplementaryData = default;
            Optional<OBBranchAndFinancialInstitutionIdentification60> creditorAgent = default;
            Optional<OBCashAccount50> creditorAccount = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("BeneficiaryId"))
                {
                    beneficiaryId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("BeneficiaryType"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    beneficiaryType = property.Value.GetString().ToOBBeneficiaryType1CodeEnum();
                    continue;
                }
                if (property.NameEquals("Reference"))
                {
                    reference = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("SupplementaryData"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (var property0 in property.Value.EnumerateObject())
                    {
                        dictionary.Add(property0.Name, property0.Value.GetObject());
                    }
                    supplementaryData = dictionary;
                    continue;
                }
                if (property.NameEquals("CreditorAgent"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    creditorAgent = OBBranchAndFinancialInstitutionIdentification60.DeserializeOBBranchAndFinancialInstitutionIdentification60(property.Value);
                    continue;
                }
                if (property.NameEquals("CreditorAccount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    creditorAccount = OBCashAccount50.DeserializeOBCashAccount50(property.Value);
                    continue;
                }
            }
            return new OBBeneficiary5(accountId.Value, beneficiaryId.Value, Optional.ToNullable(beneficiaryType), reference.Value, Optional.ToDictionary(supplementaryData), creditorAgent.Value, creditorAccount.Value);
        }
    }
}
