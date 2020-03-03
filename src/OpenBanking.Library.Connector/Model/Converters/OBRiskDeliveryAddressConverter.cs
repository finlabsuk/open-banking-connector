// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using AutoMapper;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p1.Model;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Converters
{
    public class OBRiskDeliveryAddressConverter : ITypeConverter<OBRiskDeliveryAddress, OBRisk1DeliveryAddress>
    {
        public OBRisk1DeliveryAddress Convert(OBRiskDeliveryAddress source, OBRisk1DeliveryAddress destination,
            ResolutionContext context)
        {
            return new OBRisk1DeliveryAddress(source.StreetName,
                source.CountrySubDivision.NullToEmpty().ToList(),
                source.AddressLine.NullToEmpty().ToList(),
                source.BuildingNumber, source.TownName, source.Country, source.PostCode);
        }
    }
}
