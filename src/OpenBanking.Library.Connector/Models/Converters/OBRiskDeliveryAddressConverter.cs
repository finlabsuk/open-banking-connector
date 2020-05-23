// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Converters
{
    public class OBRiskDeliveryAddressConverter : ITypeConverter<OBRiskDeliveryAddress, OBRisk1DeliveryAddress>
    {
        public OBRisk1DeliveryAddress Convert(
            OBRiskDeliveryAddress source,
            OBRisk1DeliveryAddress destination,
            ResolutionContext context)
        {
            return new OBRisk1DeliveryAddress(
                streetName: source.StreetName,
                countrySubDivision: source.CountrySubDivision.NullToEmpty().ToList(),
                addressLine: source.AddressLine.NullToEmpty().ToList(),
                buildingNumber: source.BuildingNumber,
                townName: source.TownName,
                country: source.Country,
                postCode: source.PostCode);
        }
    }
}
