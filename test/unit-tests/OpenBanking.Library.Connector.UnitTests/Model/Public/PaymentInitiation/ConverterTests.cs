// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FluentAssertions;
using Xunit;
using OBAddressTypeCode =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.OBAddressTypeCode;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Public.PaymentInitiation
{
    public class TransformTests
    {
        public TransformTests()
        {
            _entityMapper = new EntityMapper();
        }

        private readonly EntityMapper _entityMapper;

        [Fact]
        public void OBWriteDomesticConsent_To_V111()
        {
            OBWriteDomesticConsent4 value = new OBWriteDomesticConsent4
            {
                Data = new OBWriteDomesticConsent4Data
                {
                    Initiation = new OBWriteDomestic2DataInitiation
                    {
                        CreditorAccount = new OBWriteDomestic2DataInitiationCreditorAccount
                        {
                            Identification = "id",
                            Name = "test",
                            SchemeName = "schema",
                            SecondaryIdentification = "secondary id"
                        },
                        CreditorPostalAddress = new ObModels.PaymentInitiation.V3p1p4.Model.OBPostalAddress6
                        {
                            AddressLine = new List<string> { "1 high street", "blexley" },
                            AddressType = OBAddressTypeCode.POBox,
                            BuildingNumber = "42",
                            Country = "UK",
                            CountrySubDivision = "England",
                            Department = "Home counties",
                            PostCode = "BL1 9ZZ",
                            StreetName = "high street",
                            SubDepartment = "SubDepartment",
                            TownName = "Blexley"
                        },
                        DebtorAccount = new OBWriteDomestic2DataInitiationDebtorAccount
                        {
                            Identification = "abc",
                            Name = "debtor name",
                            SchemeName = "schema",
                            SecondaryIdentification = "debtor secondary id"
                        },
                        EndToEndIdentification = "EndToEndIdentification",
                        InstructedAmount = new OBWriteDomestic2DataInitiationInstructedAmount
                        {
                            Amount = 1234.56.ToString(),
                            Currency = "GBP"
                        },
                        InstructionIdentification = "instruction identification",
                        LocalInstrument = "local instrument",
                        RemittanceInformation = new OBWriteDomestic2DataInitiationRemittanceInformation
                        {
                            Reference = "reference",
                            Unstructured = "unstructured"
                        },
                        SupplementaryData = new Dictionary<string, object>()
                    },
                    Authorisation = new OBWriteDomesticConsent4DataAuthorisation
                    {
                        AuthorisationType = OBWriteDomesticConsent4DataAuthorisation.AuthorisationTypeEnum.Single,
                        CompletionDateTime = DateTime.UtcNow
                    },
                    SCASupportData = new OBWriteDomesticConsent4DataSCASupportData
                    {
                        AppliedAuthenticationApproach = OBWriteDomesticConsent4DataSCASupportData.AppliedAuthenticationApproachEnum.SCA,
                        ReferencePaymentOrderId = "reference Payment Order Id",
                        RequestedSCAExemptionType = OBWriteDomesticConsent4DataSCASupportData.RequestedSCAExemptionTypeEnum.PartyToParty
                    }
                },
                Risk = new OBRisk1
                {
                    DeliveryAddress = new OBRisk1DeliveryAddress
                    {
                        TownName = "Accrington Stanley",
                        Country = "UK",
                        AddressLine = Enumerable.Range(start: 1, count: 3).Select(i => i.ToString()).ToList(),
                        BuildingNumber = "building number",
                        CountrySubDivision = Enumerable.Range(start: 10, count: 3).Select(i => i.ToString()).ToList(),
                        PostCode = "post code",
                        StreetName = "street name"
                    },
                    MerchantCategoryCode = "a",
                    PaymentContextCode = OBRisk1.PaymentContextCodeEnum.BillPayment,
                    MerchantCustomerIdentification = "merchant Customer Identification"
                }
            };
            ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomesticConsent2 result = _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomesticConsent2>(value);

            result.Should().NotBeNull();
        }

        [Fact]
        public void OBWriteDomesticDataInitiationCreditorAccount_To_V111()
        {
            OBWriteDomestic2DataInitiationCreditorAccount value = new OBWriteDomestic2DataInitiationCreditorAccount
            {
                Identification = "id",
                Name = "test",
                SchemeName = "schema",
                SecondaryIdentification = "secondary id"
            };

            ObModels.PaymentInitiation.V3p1p1.Model.OBCashAccountCreditor3 result = _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBCashAccountCreditor3>(value);

            result.Should().NotBeNull();
            result.Identification.Should().Be(value.Identification);
            result.Name.Should().Be(value.Name);
            result.SchemeName.Should().Be(value.SchemeName);
            result.SecondaryIdentification.Should().Be(value.SecondaryIdentification);
        }

        [Fact]
        public void OBPostalAddress_To_V111()
        {
            ObModels.PaymentInitiation.V3p1p4.Model.OBPostalAddress6 value = new ObModels.PaymentInitiation.V3p1p4.Model.OBPostalAddress6
            {
                AddressLine = new List<string> { "1 high street", "blexley" },
                AddressType = OBAddressTypeCode.POBox,
                BuildingNumber = "42",
                Country = "UK",
                CountrySubDivision = "England",
                Department = "Home counties",
                PostCode = "BL1 9ZZ",
                StreetName = "high street",
                SubDepartment = "SubDepartment",
                TownName = "Blexley"
            };

            ObModels.PaymentInitiation.V3p1p1.Model.OBPostalAddress6 result = _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBPostalAddress6>(value);

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
            result.AddressType.Value.ToString().Should().Be(value.AddressType.Value.ToString());
        }

        [Fact]
        public void OBWriteDomesticDataInitiationDebtorAccount_To_V111()
        {
            OBWriteDomestic2DataInitiationDebtorAccount value = new OBWriteDomestic2DataInitiationDebtorAccount
            {
                Identification = "abc",
                Name = "debtor name",
                SchemeName = "schema",
                SecondaryIdentification = "debtor secondary id"
            };

            ObModels.PaymentInitiation.V3p1p1.Model.OBCashAccountDebtor4 result = _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBCashAccountDebtor4>(value);

            result.Should().NotBeNull();
            result.Identification.Should().Be(value.Identification);
            result.Name.Should().Be(value.Name);
            result.SchemeName.Should().Be(value.SchemeName);
            result.SecondaryIdentification.Should().Be(value.SecondaryIdentification);
        }

        [Fact]
        public void OBWriteDomesticDataInitiationInstructedAmount_To_V111()
        {
            OBWriteDomestic2DataInitiationInstructedAmount value = new OBWriteDomestic2DataInitiationInstructedAmount
            {
                Amount = 1234.56.ToString(),
                Currency = "GBP"
            };

            ObModels.PaymentInitiation.V3p1p1.Model.OBDomestic2InstructedAmount result = _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBDomestic2InstructedAmount>(value);

            result.Should().NotBeNull();
            result.Amount.Should().Be(value.Amount);
            result.Currency.Should().Be(value.Currency);
        }

        [Fact]
        public void OBWriteDomesticDataInitiationRemittanceInformation_To_V111()
        {
            OBWriteDomestic2DataInitiationRemittanceInformation value =
                new OBWriteDomestic2DataInitiationRemittanceInformation
                {
                    Reference = "reference",
                    Unstructured = "unstructured"
                };

            ObModels.PaymentInitiation.V3p1p1.Model.OBRemittanceInformation1 result = _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBRemittanceInformation1>(value);

            result.Should().NotBeNull();
            result.Reference.Should().Be(value.Reference);
            result.Unstructured.Should().Be(value.Unstructured);
        }

        [Fact]
        public void OBWriteDomesticConsentDataAuthorisation_To_V111()
        {
            OBWriteDomesticConsent4DataAuthorisation value = new OBWriteDomesticConsent4DataAuthorisation
            {
                AuthorisationType = OBWriteDomesticConsent4DataAuthorisation.AuthorisationTypeEnum.Single,
                CompletionDateTime = DateTime.UtcNow
            };

            ObModels.PaymentInitiation.V3p1p1.Model.OBAuthorisation1 result = _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBAuthorisation1>(value);

            result.Should().NotBeNull();
            result.AuthorisationType.ToString().Should().Be(value.AuthorisationType.ToString());
            result.CompletionDateTime.Should().Be(value.CompletionDateTime);
        }

        [Fact]
        public void OBRisk_To_V111()
        {
            OBRisk1 value = new OBRisk1
            {
                DeliveryAddress = new OBRisk1DeliveryAddress
                {
                    TownName = "Accrington Stanley",
                    Country = "UK",
                    AddressLine = Enumerable.Range(start: 1, count: 3).Select(i => i.ToString()).ToList(),
                    BuildingNumber = "building number",
                    CountrySubDivision = Enumerable.Range(start: 10, count: 3).Select(i => i.ToString()).ToList(),
                    PostCode = "post code",
                    StreetName = "street name"
                },
                MerchantCategoryCode = "a",
                PaymentContextCode = OBRisk1.PaymentContextCodeEnum.BillPayment,
                MerchantCustomerIdentification = "merchant Customer Identification"
            };

            OBRisk1 result = _entityMapper.Map<OBRisk1>(value);

            result.Should().NotBeNull();
            result.DeliveryAddress.Should().NotBeNull();
            result.DeliveryAddress.TownName.Should().Be(value.DeliveryAddress.TownName);
            result.DeliveryAddress.AddressLine.Should().BeEquivalentTo(value.DeliveryAddress.AddressLine);
            result.DeliveryAddress.BuildingNumber.Should().Be(value.DeliveryAddress.BuildingNumber);
            result.DeliveryAddress.CountrySubDivision.Should().BeEquivalentTo(value.DeliveryAddress.CountrySubDivision);
            result.DeliveryAddress.PostCode.Should().Be(value.DeliveryAddress.PostCode);
            result.DeliveryAddress.StreetName.Should().Be(value.DeliveryAddress.StreetName);
            result.MerchantCategoryCode.Should().Be(value.MerchantCategoryCode);
            result.PaymentContextCode.ToString().Should().Be(value.PaymentContextCode.ToString());
            result.MerchantCustomerIdentification.Should().Be(value.MerchantCustomerIdentification);
        }
    }
}
