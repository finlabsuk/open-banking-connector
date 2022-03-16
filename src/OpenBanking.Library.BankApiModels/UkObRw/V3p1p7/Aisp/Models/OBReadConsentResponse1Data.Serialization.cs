// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBReadConsentResponse1Data
    {
        internal static OBReadConsentResponse1Data DeserializeOBReadConsentResponse1Data(JsonElement element)
        {
            string consentId = default;
            DateTimeOffset creationDateTime = default;
            OBReadConsentResponse1DataStatusEnum status = default;
            DateTimeOffset statusUpdateDateTime = default;
            IReadOnlyList<OBReadConsentResponse1DataPermissionsEnum> permissions = default;
            Optional<DateTimeOffset> expirationDateTime = default;
            Optional<DateTimeOffset> transactionFromDateTime = default;
            Optional<DateTimeOffset> transactionToDateTime = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("ConsentId"))
                {
                    consentId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("CreationDateTime"))
                {
                    creationDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("Status"))
                {
                    status = property.Value.GetString().ToOBReadConsentResponse1DataStatusEnum();
                    continue;
                }
                if (property.NameEquals("StatusUpdateDateTime"))
                {
                    statusUpdateDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("Permissions"))
                {
                    List<OBReadConsentResponse1DataPermissionsEnum> array = new List<OBReadConsentResponse1DataPermissionsEnum>();
                    foreach (var item in property.Value.EnumerateArray())
                    {
                        array.Add(item.GetString().ToOBReadConsentResponse1DataPermissionsEnum());
                    }
                    permissions = array;
                    continue;
                }
                if (property.NameEquals("ExpirationDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    expirationDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("TransactionFromDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    transactionFromDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("TransactionToDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    transactionToDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
            }
            return new OBReadConsentResponse1Data(consentId, creationDateTime, status, statusUpdateDateTime, permissions, Optional.ToNullable(expirationDateTime), Optional.ToNullable(transactionFromDateTime), Optional.ToNullable(transactionToDateTime));
        }
    }
}
