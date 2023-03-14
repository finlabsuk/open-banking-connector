// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentAssertions;
using Xunit;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Public.PaymentInitiation;

public class TransformTests
{
    public TransformTests()
    {
        _apiVariantMapper = new ApiVariantMapper();
    }

    private readonly ApiVariantMapper _apiVariantMapper;

    [Fact]
    public void OBWriteDomesticConsent_To_V111()
    {
        var value =
            new PaymentInitiationModelsPublic.OBWriteDomesticConsent4
            {
                Data = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4Data
                {
                    Initiation = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiation
                    {
                        CreditorAccount =
                            new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiationCreditorAccount
                            {
                                Identification = "id",
                                Name = "test",
                                SchemeName = "schema",
                                SecondaryIdentification = "secondary id"
                            },
                        CreditorPostalAddress = new PaymentInitiationModelsPublic.OBPostalAddress6
                        {
                            AddressLine = new List<string> { "1 high street", "blexley" },
                            AddressType = PaymentInitiationModelsPublic.OBAddressTypeCodeEnum.POBox,
                            BuildingNumber = "42",
                            Country = "UK",
                            CountrySubDivision = "England",
                            Department = "Home counties",
                            PostCode = "BL1 9ZZ",
                            StreetName = "high street",
                            SubDepartment = "SubDepartment",
                            TownName = "Blexley"
                        },
                        DebtorAccount =
                            new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiationDebtorAccount
                            {
                                Identification = "abc",
                                Name = "debtor name",
                                SchemeName = "schema",
                                SecondaryIdentification = "debtor secondary id"
                            },
                        EndToEndIdentification = "EndToEndIdentification",
                        InstructedAmount =
                            new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiationInstructedAmount
                            {
                                Amount = 1234.56.ToString(CultureInfo.InvariantCulture),
                                Currency = "GBP"
                            },
                        InstructionIdentification = "instruction identification",
                        LocalInstrument = "local instrument",
                        RemittanceInformation =
                            new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiationRemittanceInformation
                            {
                                Reference = "reference",
                                Unstructured = "unstructured"
                            },
                        SupplementaryData = new Dictionary<string, object>()
                    },
                    Authorisation = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataAuthorisation
                    {
                        AuthorisationType = PaymentInitiationModelsPublic
                            .OBWriteDomesticConsent4DataAuthorisationAuthorisationTypeEnum.Single,
                        CompletionDateTime = DateTimeOffset.UtcNow
                    },
                    SCASupportData = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataSCASupportData
                    {
                        AppliedAuthenticationApproach = PaymentInitiationModelsPublic
                            .OBWriteDomesticConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA,
                        ReferencePaymentOrderId = "reference Payment Order Id",
                        RequestedSCAExemptionType = PaymentInitiationModelsPublic
                            .OBWriteDomesticConsent4DataSCASupportDataRequestedSCAExemptionTypeEnum.PartyToParty
                    }
                },
                Risk = new PaymentInitiationModelsPublic.OBRisk1
                {
                    DeliveryAddress = new PaymentInitiationModelsPublic.OBRisk1DeliveryAddress
                    {
                        TownName = "Accrington Stanley",
                        Country = "UK",
                        AddressLine = Enumerable.Range(1, 3).Select(i => i.ToString()).ToList(),
                        BuildingNumber = "building number",
                        CountrySubDivision = "country subdivision",
                        PostCode = "post code",
                        StreetName = "street name"
                    },
                    MerchantCategoryCode = "a",
                    PaymentContextCode = PaymentInitiationModelsPublic.OBRisk1PaymentContextCodeEnum.BillPayment,
                    MerchantCustomerIdentification = "merchant Customer Identification"
                }
            };
        _apiVariantMapper.Map(value, out PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4 result);

        result.Should().NotBeNull();
    }

    [Fact]
    public void OBWriteDomesticDataInitiationCreditorAccount_To_V111()
    {
        var value =
            new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationCreditorAccount
            {
                Identification = "id",
                Name = "test",
                SchemeName = "schema",
                SecondaryIdentification = "secondary id"
            };

        _apiVariantMapper.Map(
            value,
            out PaymentInitiationModelsV3p1p4.OBWriteDomestic2DataInitiationCreditorAccount result);

        result.Should().NotBeNull();
        result.Identification.Should().Be(value.Identification);
        result.Name.Should().Be(value.Name);
        result.SchemeName.Should().Be(value.SchemeName);
        result.SecondaryIdentification.Should().Be(value.SecondaryIdentification);
    }

    [Fact]
    public void OBPostalAddress_To_V111()
    {
        var value = new PaymentInitiationModelsPublic.OBPostalAddress6
        {
            AddressLine = new List<string> { "1 high street", "blexley" },
            AddressType = PaymentInitiationModelsPublic.OBAddressTypeCodeEnum.POBox,
            BuildingNumber = "42",
            Country = "UK",
            CountrySubDivision = "England",
            Department = "Home counties",
            PostCode = "BL1 9ZZ",
            StreetName = "high street",
            SubDepartment = "SubDepartment",
            TownName = "Blexley"
        };

        _apiVariantMapper.Map(value, out PaymentInitiationModelsV3p1p4.OBPostalAddress6 result);

        result.Should().NotBeNull();
        result.AddressLine.Should().BeEquivalentTo(value.AddressLine);
        result.BuildingNumber.Should().Be(value.BuildingNumber);
        result.Country.Should().Be(value.Country);
        result.CountrySubDivision.Should().Be(value.CountrySubDivision);
        result.Department.Should().Be(value.Department);
        result.PostCode.Should().Be(value.PostCode);
        result.StreetName.Should().Be(value.StreetName);
        result.SubDepartment.Should().Be(value.SubDepartment);
        result.TownName.Should().Be(value.TownName);
        result.AddressType.Should().NotBeNull();
        result.AddressType!.Value.ToString().Should().Be(value.AddressType.Value.ToString());
    }

    [Fact]
    public void OBWriteDomesticDataInitiationDebtorAccount_To_V111()
    {
        var value =
            new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationDebtorAccount
            {
                Identification = "abc",
                Name = "debtor name",
                SchemeName = "schema",
                SecondaryIdentification = "debtor secondary id"
            };

        _apiVariantMapper.Map(
            value,
            out PaymentInitiationModelsV3p1p4.OBWriteDomestic2DataInitiationDebtorAccount result);

        result.Should().NotBeNull();
        result.Identification.Should().Be(value.Identification);
        result.Name.Should().Be(value.Name);
        result.SchemeName.Should().Be(value.SchemeName);
        result.SecondaryIdentification.Should().Be(value.SecondaryIdentification);
    }

    [Fact]
    public void OBWriteDomesticDataInitiationInstructedAmount_To_V111()
    {
        var value =
            new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationInstructedAmount
            {
                Amount = 1234.56.ToString(CultureInfo.InvariantCulture),
                Currency = "GBP"
            };

        _apiVariantMapper.Map(
            value,
            out PaymentInitiationModelsV3p1p4.OBWriteDomestic2DataInitiationInstructedAmount result);

        result.Should().NotBeNull();
        result.Amount.Should().Be(value.Amount);
        result.Currency.Should().Be(value.Currency);
    }

    [Fact]
    public void OBWriteDomesticDataInitiationRemittanceInformation_To_V111()
    {
        var value =
            new PaymentInitiationModelsPublic.OBWriteDomestic2DataInitiationRemittanceInformation
            {
                Reference = "reference",
                Unstructured = "unstructured"
            };

        _apiVariantMapper.Map(
            value,
            out PaymentInitiationModelsV3p1p4.OBWriteDomestic2DataInitiationRemittanceInformation result);

        result.Should().NotBeNull();
        result.Reference.Should().Be(value.Reference);
        result.Unstructured.Should().Be(value.Unstructured);
    }

    [Fact]
    public void OBWriteDomesticConsentDataAuthorisation_To_V111()
    {
        var value =
            new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataAuthorisation
            {
                AuthorisationType = PaymentInitiationModelsPublic
                    .OBWriteDomesticConsent4DataAuthorisationAuthorisationTypeEnum.Single,
                CompletionDateTime = DateTimeOffset.UtcNow
            };

        _apiVariantMapper.Map(
            value,
            out PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4DataAuthorisation result);

        result.Should().NotBeNull();
        result.AuthorisationType.ToString().Should().Be(value.AuthorisationType.ToString());
        result.CompletionDateTime.Should().Be(value.CompletionDateTime.Value);
    }

    [Fact]
    public void OBRisk_To_V111()
    {
        var value = new PaymentInitiationModelsPublic.OBRisk1
        {
            DeliveryAddress = new PaymentInitiationModelsPublic.OBRisk1DeliveryAddress
            {
                TownName = "Accrington Stanley",
                Country = "UK",
                AddressLine = Enumerable.Range(1, 3).Select(i => i.ToString()).ToList(),
                BuildingNumber = "building number",
                CountrySubDivision = "country subdivision",
                PostCode = "post code",
                StreetName = "street name"
            },
            MerchantCategoryCode = "a",
            PaymentContextCode = PaymentInitiationModelsPublic.OBRisk1PaymentContextCodeEnum.BillPayment,
            MerchantCustomerIdentification = "merchant Customer Identification"
        };

        _apiVariantMapper.Map(value, out PaymentInitiationModelsV3p1p4.OBRisk1 result);

        result.Should().NotBeNull();
        result.DeliveryAddress.Should().NotBeNull();
        result.DeliveryAddress.TownName.Should().Be(value.DeliveryAddress.TownName);
        result.DeliveryAddress.AddressLine.Should().BeEquivalentTo(value.DeliveryAddress.AddressLine);
        result.DeliveryAddress.BuildingNumber.Should().Be(value.DeliveryAddress.BuildingNumber);
        result.DeliveryAddress.CountrySubDivision.Should()
            .BeEquivalentTo(value.DeliveryAddress.CountrySubDivision.Split(" "));
        result.DeliveryAddress.PostCode.Should().Be(value.DeliveryAddress.PostCode);
        result.DeliveryAddress.StreetName.Should().Be(value.DeliveryAddress.StreetName);
        result.MerchantCategoryCode.Should().Be(value.MerchantCategoryCode);
        result.PaymentContextCode.ToString().Should().Be(value.PaymentContextCode.ToString());
        result.MerchantCustomerIdentification.Should().Be(value.MerchantCustomerIdentification);
    }
}
