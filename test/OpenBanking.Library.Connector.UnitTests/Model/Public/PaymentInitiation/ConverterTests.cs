// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Public.PaymentInitiation;

public class TransformTests
{
    private readonly ApiVariantMapper _apiVariantMapper;

    public TransformTests()
    {
        _apiVariantMapper = new ApiVariantMapper();
    }

    [Fact]
    public void OBWriteDomesticConsent_To_OBWriteDomestic()
    {
        var value =
            new PaymentInitiationModelsPublic.OBWriteDomesticConsent4
            {
                Data = new PaymentInitiationModelsPublic.Data2
                {
                    Initiation = new PaymentInitiationModelsPublic.Initiation2
                    {
                        CreditorAccount =
                            new PaymentInitiationModelsPublic.CreditorAccount2
                            {
                                Identification = "id",
                                Name = "test",
                                SchemeName = "schema",
                                SecondaryIdentification = "secondary id"
                            },
                        CreditorPostalAddress = new PaymentInitiationModelsPublic.OBPostalAddress6
                        {
                            AddressLine = new List<string>
                            {
                                "1 high street",
                                "blexley"
                            },
                            AddressType = PaymentInitiationModelsPublic.OBAddressTypeCode.POBox,
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
                            new PaymentInitiationModelsPublic.DebtorAccount2
                            {
                                Identification = "abc",
                                Name = "debtor name",
                                SchemeName = "schema",
                                SecondaryIdentification = "debtor secondary id"
                            },
                        EndToEndIdentification = "EndToEndIdentification",
                        InstructedAmount =
                            new PaymentInitiationModelsPublic.InstructedAmount2
                            {
                                Amount = 1234.56.ToString(CultureInfo.InvariantCulture),
                                Currency = "GBP"
                            },
                        InstructionIdentification = "instruction identification",
                        LocalInstrument = "local instrument",
                        RemittanceInformation =
                            new PaymentInitiationModelsPublic.RemittanceInformation2
                            {
                                Reference = "reference",
                                Unstructured = "unstructured"
                            }
                    },
                    Authorisation =
                        new PaymentInitiationModelsPublic.Authorisation
                        {
                            AuthorisationType =
                                PaymentInitiationModelsPublic.AuthorisationType.Single,
                            CompletionDateTime = DateTimeOffset.UtcNow
                        },
                    SCASupportData =
                        new PaymentInitiationModelsPublic.OBSCASupportData1
                        {
                            AppliedAuthenticationApproach =
                                PaymentInitiationModelsPublic.OBSCASupportData1AppliedAuthenticationApproach.SCA,
                            ReferencePaymentOrderId = "reference Payment Order Id",
                            RequestedSCAExemptionType =
                                PaymentInitiationModelsPublic.OBSCASupportData1RequestedSCAExemptionType.PartyToParty
                        }
                },
                Risk = new PaymentInitiationModelsPublic.OBRisk1
                {
                    DeliveryAddress = new PaymentInitiationModelsPublic.DeliveryAddress
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
                    PaymentContextCode = PaymentInitiationModelsPublic.OBRisk1PaymentContextCode.BillPayment,
                    MerchantCustomerIdentification = "merchant Customer Identification"
                }
            };
        _apiVariantMapper.Map(value, out PaymentInitiationModelsPublic.OBWriteDomestic2 result);

        result.Should().NotBeNull();

        // Check creditor account
        result.Data.Initiation.CreditorAccount.Identification.Should()
            .Be(value.Data.Initiation.CreditorAccount.Identification);
        result.Data.Initiation.CreditorAccount.Name.Should().Be(value.Data.Initiation.CreditorAccount.Name);
        result.Data.Initiation.CreditorAccount.SchemeName.Should().Be(value.Data.Initiation.CreditorAccount.SchemeName);
        result.Data.Initiation.CreditorAccount.SecondaryIdentification.Should()
            .Be(value.Data.Initiation.CreditorAccount.SecondaryIdentification);

        // Check debtor account
        result.Data.Initiation.DebtorAccount.Should().NotBeNull();
        result.Data.Initiation.DebtorAccount!.Identification.Should()
            .Be(value.Data.Initiation.DebtorAccount.Identification);
        result.Data.Initiation.DebtorAccount.Name.Should().Be(value.Data.Initiation.DebtorAccount.Name);
        result.Data.Initiation.DebtorAccount.SchemeName.Should().Be(value.Data.Initiation.DebtorAccount.SchemeName);
        result.Data.Initiation.DebtorAccount.SecondaryIdentification.Should()
            .Be(value.Data.Initiation.DebtorAccount.SecondaryIdentification);

        // Check instructed amount
        result.Data.Initiation.InstructedAmount.Amount.Should().Be(value.Data.Initiation.InstructedAmount.Amount);
        result.Data.Initiation.InstructedAmount.Currency.Should().Be(value.Data.Initiation.InstructedAmount.Currency);

        // Check remittance information
        result.Data.Initiation.RemittanceInformation.Should().NotBeNull();
        result.Data.Initiation.RemittanceInformation!.Reference.Should()
            .Be(value.Data.Initiation.RemittanceInformation.Reference);
        result.Data.Initiation.RemittanceInformation.Unstructured.Should()
            .Be(value.Data.Initiation.RemittanceInformation.Unstructured);

        // Check Risk
        result.Risk.DeliveryAddress.Should().NotBeNull();
        result.Risk.DeliveryAddress!.AddressLine.Should().BeEquivalentTo(value.Risk.DeliveryAddress.AddressLine);
        result.Risk.DeliveryAddress.BuildingNumber.Should().Be(value.Risk.DeliveryAddress.BuildingNumber);
        result.Risk.DeliveryAddress.Country.Should().Be(value.Risk.DeliveryAddress.Country);
        result.Risk.DeliveryAddress.CountrySubDivision.Should().Be(value.Risk.DeliveryAddress.CountrySubDivision);
        result.Risk.DeliveryAddress.PostCode.Should().Be(value.Risk.DeliveryAddress.PostCode);
        result.Risk.DeliveryAddress.StreetName.Should().Be(value.Risk.DeliveryAddress.StreetName);
        result.Risk.DeliveryAddress.TownName.Should().Be(value.Risk.DeliveryAddress.TownName);
        result.Risk.MerchantCategoryCode.Should().Be(value.Risk.MerchantCategoryCode);
        result.Risk.PaymentContextCode.ToString().Should().Be(value.Risk.PaymentContextCode.ToString());
        result.Risk.MerchantCustomerIdentification.Should().Be(value.Risk.MerchantCustomerIdentification);
    }
}
