// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBTransactionCardInstrument1
    {
        internal static OBTransactionCardInstrument1 DeserializeOBTransactionCardInstrument1(JsonElement element)
        {
            OBTransactionCardInstrument1CardSchemeNameEnum cardSchemeName = default;
            Optional<OBTransactionCardInstrument1AuthorisationTypeEnum> authorisationType = default;
            Optional<string> name = default;
            Optional<string> identification = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("CardSchemeName"))
                {
                    cardSchemeName = property.Value.GetString().ToOBTransactionCardInstrument1CardSchemeNameEnum();
                    continue;
                }
                if (property.NameEquals("AuthorisationType"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    authorisationType = property.Value.GetString().ToOBTransactionCardInstrument1AuthorisationTypeEnum();
                    continue;
                }
                if (property.NameEquals("Name"))
                {
                    name = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Identification"))
                {
                    identification = property.Value.GetString();
                    continue;
                }
            }
            return new OBTransactionCardInstrument1(cardSchemeName, Optional.ToNullable(authorisationType), name.Value, identification.Value);
        }
    }
}
