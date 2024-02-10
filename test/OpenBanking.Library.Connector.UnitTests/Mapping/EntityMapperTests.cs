// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AutoMapper;
using FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentAssertions;
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
using SupportedAlgorithmsEnum =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p1.Models.SupportedAlgorithmsEnum;


namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Mapping;

public class EntityMapperTests
{
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
                .CreateMapper(); // possible alternative (?): var mapper = new ApiVariantMapper();

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
            Scope = new List<string>
            {
                "Value1",
                "Value2"
            }
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

        output.Scope.Should().NotBeNull();

        output.Scope.Should().Equal(
            new List<string>
            {
                "Value1",
                "Value2"
            });
    }
}
