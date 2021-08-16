// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p4.PaymentInitiation.Models;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.Connector.UkRwApi.V3p1p6.PaymentInitiation.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Mapping
{
    public class EntityMapperTests
    {
        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property OpenBankingToPublic_SimpleMap_V3_1_4_ToPublic_2(
            PaymentInitiationModelsV3p1p4.OBAddressTypeCodeEnum addressType,
            string department,
            string subDepartment,
            string streetName,
            string buildingNumber,
            string postCode,
            string townName,
            string countrySubDivision,
            string country,
            IList<string> addressLine)
        {
            ApiVariantMapper mapper = new ApiVariantMapper();

            Func<bool> rule = () =>
            {
                PaymentInitiationModelsV3p1p4.OBPostalAddress6 value =
                    new PaymentInitiationModelsV3p1p4.OBPostalAddress6(
                        addressType,
                        department,
                        subDepartment,
                        streetName,
                        buildingNumber,
                        postCode,
                        townName,
                        countrySubDivision,
                        country,
                        addressLine);

                mapper.Map(value, out PaymentInitiationModelsPublic.OBPostalAddress6 r1);

                return r1.AddressLine.SequenceEqual(value.AddressLine) &&
                       r1.BuildingNumber == value.BuildingNumber &&
                       r1.AddressType.ToString() == value.AddressType.ToString() &&
                       r1.Country == value.Country &&
                       r1.CountrySubDivision == value.CountrySubDivision &&
                       r1.Department == value.Department &&
                       r1.PostCode == value.PostCode &&
                       r1.StreetName == value.StreetName &&
                       r1.SubDepartment == value.SubDepartment &&
                       r1.TownName == value.TownName;
            };


            return rule.ToProperty();
        }

        [Fact]
        public void ConfigurationIsValid()
        {
            ApiVariantMappingConfiguration apiVariantMappingConfiguration = new ApiVariantMappingConfiguration();

            apiVariantMappingConfiguration.CreateMapperConfiguration().AssertConfigurationIsValid();
        }


        [Fact]
        public void OpenBankingToPublic_SimpleMap_V3_1_4_ToPublic()
        {
            ApiVariantMapper mapper = new ApiVariantMapper();

            PaymentInitiationModelsV3p1p4.Meta m1 = new PaymentInitiationModelsV3p1p4.Meta
            {
                FirstAvailableDateTime = DateTimeOffset.MaxValue,
                LastAvailableDateTime = DateTimeOffset.UnixEpoch,
                TotalPages = 123
            };

            mapper.Map(m1, out PaymentInitiationModelsPublic.Meta r1);

            r1.Should().NotBeNull();

            m1.FirstAvailableDateTime.Should().Be(r1.FirstAvailableDateTime ?? null);
            m1.LastAvailableDateTime.Should().Be(r1.LastAvailableDateTime ?? null);
            m1.TotalPages.Should().Be(r1.TotalPages);
        }
    }
}
