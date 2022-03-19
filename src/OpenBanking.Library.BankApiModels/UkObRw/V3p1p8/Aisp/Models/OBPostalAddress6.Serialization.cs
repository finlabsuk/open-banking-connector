// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System.Collections.Generic;
using System.Text.Json;
using Azure.Core;

namespace FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p8.Aisp.Models
{
    public partial class OBPostalAddress6
    {
        public static OBPostalAddress6 DeserializeOBPostalAddress6(JsonElement element)
        {
            Optional<OBAddressTypeCodeEnum> addressType = default;
            Optional<string> department = default;
            Optional<string> subDepartment = default;
            Optional<string> streetName = default;
            Optional<string> buildingNumber = default;
            Optional<string> postCode = default;
            Optional<string> townName = default;
            Optional<string> countrySubDivision = default;
            Optional<string> country = default;
            Optional<IReadOnlyList<string>> addressLine = default;
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
                if (property.NameEquals("Department"))
                {
                    department = property.Value.GetString();
                    continue;
                }
                if (property.NameEquals("SubDepartment"))
                {
                    subDepartment = property.Value.GetString();
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
            }
            return new OBPostalAddress6(Optional.ToNullable(addressType), department.Value, subDepartment.Value, streetName.Value, buildingNumber.Value, postCode.Value, townName.Value, countrySubDivision.Value, country.Value, Optional.ToList(addressLine));
        }
    }
}
