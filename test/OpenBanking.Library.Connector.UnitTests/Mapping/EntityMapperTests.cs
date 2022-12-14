// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using OBClientRegistration1 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models.OBClientRegistration1;
using OBRegistrationProperties1applicationTypeEnum =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models.OBRegistrationProperties1applicationTypeEnum;
using OBRegistrationProperties1grantTypesItemEnum =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models.OBRegistrationProperties1grantTypesItemEnum;
using OBRegistrationProperties1tokenEndpointAuthMethodEnum =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models.
    OBRegistrationProperties1tokenEndpointAuthMethodEnum;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;
using SupportedAlgorithmsEnum =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models.SupportedAlgorithmsEnum;


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
            var mapper = new ApiVariantMapper();

            Func<bool> rule = () =>
            {
                var value =
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
            MapperConfiguration mapperConfiguration =
                new ApiVariantMappingConfiguration()
                    .CreateMapperConfiguration();

            mapperConfiguration.AssertConfigurationIsValid();
        }

        [Fact]
        public void OpenBankingDcr_V3p1Scope_MapToV3p3()
        {
            IMapper mapper =
                new ApiVariantMappingConfiguration()
                    .CreateMapperConfiguration()
                    .CreateMapper();

            var input = new OBClientRegistration1(
                new List<string>(),
                OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth,
                new List<OBRegistrationProperties1grantTypesItemEnum>(),
                string.Empty,
                OBRegistrationProperties1applicationTypeEnum.Web,
                SupportedAlgorithmsEnum.PS256,
                SupportedAlgorithmsEnum.PS256,
                string.Empty,
                string.Empty,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                string.Empty,
                string.Empty)
            {
                Scope = new List<string> { "Value1", "Value2" }
            };


            var output = mapper.Map<OBClientRegistration1Response>(input);

            output.Scope.Should().Be("Value1 Value2");
        }


        [Fact]
        public void OpenBankingDcr_V3p3Scope_MapToV3p1()
        {
            IMapper mapper =
                new ApiVariantMappingConfiguration()
                    .CreateMapperConfiguration()
                    .CreateMapper();

            var input = new BankApiModels.UKObDcr.V3p3.Models.OBClientRegistration1(
                new List<string>(),
                BankApiModels.UKObDcr.V3p3.Models.OBRegistrationProperties1tokenEndpointAuthMethodEnum.TlsClientAuth,
                new List<BankApiModels.UKObDcr.V3p3.Models.OBRegistrationProperties1grantTypesItemEnum>(),
                "Value1 Value2",
                string.Empty,
                BankApiModels.UKObDcr.V3p3.Models.OBRegistrationProperties1applicationTypeEnum.Web,
                BankApiModels.UKObDcr.V3p3.Models.SupportedAlgorithmsEnum.PS256,
                BankApiModels.UKObDcr.V3p3.Models.SupportedAlgorithmsEnum.PS256,
                string.Empty,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                string.Empty,
                string.Empty);


            var output = mapper.Map<OBClientRegistration1>(input);

            output.Scope.Should().Equal(new List<string> { "Value1", "Value2" });
        }


        [Fact]
        public void OpenBankingToPublic_SimpleMap_V3_1_4_ToPublic()
        {
            var mapper = new ApiVariantMapper();

            var m1 = new PaymentInitiationModelsV3p1p4.Meta
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
