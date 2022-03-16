// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p7.Aisp.Models
{
    public partial class OBParty2AddressItem
    {
        internal static OBParty2AddressItem DeserializeOBParty2AddressItem(JsonElement element)
        {
            Optional<OBAddressTypeCodeEnum> addressType = default;
            Optional<IReadOnlyList<string>> addressLine = default;
            Optional<string> streetName = default;
            Optional<string> buildingNumber = default;
            Optional<string> postCode = default;
            Optional<string> townName = default;
            Optional<string> countrySubDivision = default;
            string country = default;
            foreach (var property in element.EnumerateObject())
            {
                if (property.NameEquals("AddressType"))
                {
                    if (property.Value.ValueKind == JsonValueKind.Null)
                    {
                        property.ThrowNonNullablePropertyIsNull();
                        continue;
                    }
                    addressType = property.Value.GetString().ToOBAddressTypeCodeEnum();
                    continue;
                }
                if (property.NameEquals("AddressLine"))
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
                    addressLine = array;
                    continue;
                }
                if (property.NameEquals("StreetName"))
                {
                    streetName = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("BuildingNumber"))
                {
                    buildingNumber = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("PostCode"))
                {
                    postCode = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("TownName"))
                {
                    townName = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("CountrySubDivision"))
                {
                    countrySubDivision = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("Country"))
                {
                    country = property.Value.GetString();
                    continue;
                }
            }
            return new OBParty2AddressItem(Optional.ToNullable(addressType), Optional.ToList(addressLine), streetName.Value, buildingNumber.Value, postCode.Value, townName.Value, countrySubDivision.Value, country);
        }
    }
}
