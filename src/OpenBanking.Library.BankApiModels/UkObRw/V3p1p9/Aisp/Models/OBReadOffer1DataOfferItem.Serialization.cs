// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p9.Aisp.Models
{
    public partial class OBReadOffer1DataOfferItem
    {
        public static OBReadOffer1DataOfferItem DeserializeOBReadOffer1DataOfferItem(JsonElement element)
        {
            string accountId = default;
            Optional<string> offerId = default;
            Optional<OBReadOffer1DataOfferTypeEnum> offerType = default;
            Optional<string> description = default;
            Optional<DateTimeOffset> startDateTime = default;
            Optional<DateTimeOffset> endDateTime = default;
            Optional<string> rate = default;
            Optional<int> value = default;
            Optional<string> term = default;
            Optional<string> url = default;
            Optional<OBReadOffer1DataOfferItemAmount> amount = default;
            Optional<OBReadOffer1DataOfferItemFee> fee = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AccountId"))
                {
                    accountId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("OfferId"))
                {
                    offerId = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("OfferType"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    offerType = property.Value.GetString().ToOBReadOffer1DataOfferTypeEnum();
                    continue;
                }
                if (property.NameEquals("Description"))
                {
                    description = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("StartDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    startDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("EndDateTime"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    endDateTime = property.Value.GetDateTimeOffset("O");
                    continue;
                }
                if (property.NameEquals("Rate"))
                {
                    rate = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Value"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    value = property.Value.GetInt32();
                    continue;
                }
                if (property.NameEquals("Term"))
                {
                    term = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("URL"))
                {
                    url = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Amount"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    amount = OBReadOffer1DataOfferItemAmount.DeserializeOBReadOffer1DataOfferItemAmount(property.Value);
                    continue;
                }
                if (property.NameEquals("Fee"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    fee = OBReadOffer1DataOfferItemFee.DeserializeOBReadOffer1DataOfferItemFee(property.Value);
                    continue;
                }
            }
            return new OBReadOffer1DataOfferItem(accountId, offerId.Value, Optional.ToNullable(offerType), description.Value, Optional.ToNullable(startDateTime), Optional.ToNullable(endDateTime), rate.Value, Optional.ToNullable(value), term.Value, url.Value, amount.Value, fee.Value);
        }
    }
}