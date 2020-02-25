// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation;
using FinnovationLabs.OpenBanking.Library.Connector.ObModel.PaymentInitiation.V3p1p1.Model;
using FluentAssertions;
using Xunit;
using OBAddressTypeCode =
    FinnovationLabs.OpenBanking.Library.Connector.Model.Public.PaymentInitiation.OBAddressTypeCode;

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
            var value = new OBWriteDomesticConsent
            {
                Data = new OBWriteDomesticConsentData
                {
                    Initiation = new OBWriteDomesticDataInitiation
                    {
                        CreditorAccount = new OBWriteDomesticDataInitiationCreditorAccount
                        {
                            Identification = "id",
                            Name = "test",
                            SchemeName = "schema",
                            SecondaryIdentification = "secondary id"
                        },
                        CreditorPostalAddress = new OBPostalAddress
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
                        DebtorAccount = new OBWriteDomesticDataInitiationDebtorAccount
                        {
                            Identification = "abc",
                            Name = "debtor name",
                            SchemeName = "schema",
                            SecondaryIdentification = "debtor secondary id"
                        },
                        EndToEndIdentification = "EndToEndIdentification",
                        InstructedAmount = new OBWriteDomesticDataInitiationInstructedAmount
                        {
                            Amount = 1234.56.ToString(),
                            Currency = "GBP"
                        },
                        InstructionIdentification = "instruction identification",
                        LocalInstrument = "local instrument",
                        RemittanceInformation = new OBWriteDomesticDataInitiationRemittanceInformation
                        {
                            Reference = "reference",
                            Unstructured = "unstructured"
                        },
                        SupplementaryData = new object()
                    },
                    Authorisation = new OBWriteDomesticConsentDataAuthorisation
                    {
                        AuthorisationType = AuthorisationType.Single,
                        CompletionDateTime = DateTime.UtcNow
                    },
                    SCASupportData = new OBWriteDomesticConsentDataSCASupportData
                    {
                        AppliedAuthenticationApproach = AppliedAuthenticationApproach.SCA,
                        ReferencePaymentOrderId = "reference Payment Order Id",
                        RequestedSCAExemptionType = RequestedSCAExemptionType.PartyToParty
                    }
                },
                Risk = new OBRisk
                {
                    DeliveryAddress = new OBRiskDeliveryAddress
                    {
                        TownName = "Accrington Stanley",
                        Country = "UK",
                        AddressLine = Enumerable.Range(1, 3).Select(i => i.ToString()).ToList(),
                        BuildingNumber = "building number",
                        CountrySubDivision = Enumerable.Range(10, 3).Select(i => i.ToString()).ToList(),
                        PostCode = "post code",
                        StreetName = "street name"
                    },
                    MerchantCategoryCode = "a",
                    PaymentContextCode = PaymentContextCode.BillPayment,
                    MerchantCustomerIdentification = "merchant Customer Identification"
                }
            };
            var result = _entityMapper.Map<OBWriteDomesticConsent2>(value);

            result.Should().NotBeNull();
        }

        [Fact]
        public void OBWriteDomesticDataInitiationCreditorAccount_To_V111()
        {
            var value = new OBWriteDomesticDataInitiationCreditorAccount
            {
                Identification = "id",
                Name = "test",
                SchemeName = "schema",
                SecondaryIdentification = "secondary id"
            };

            var result = _entityMapper.Map<OBCashAccountCreditor3>(value);

            result.Should().NotBeNull();
            result.Identification.Should().Be(value.Identification);
            result.Name.Should().Be(value.Name);
            result.SchemeName.Should().Be(value.SchemeName);
            result.SecondaryIdentification.Should().Be(value.SecondaryIdentification);
        }

        [Fact]
        public void OBPostalAddress_To_V111()
        {
            var value = new OBPostalAddress
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

            var result = _entityMapper.Map<OBPostalAddress6>(value);

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
            var value = new OBWriteDomesticDataInitiationDebtorAccount
            {
                Identification = "abc",
                Name = "debtor name",
                SchemeName = "schema",
                SecondaryIdentification = "debtor secondary id"
            };

            var result = _entityMapper.Map<OBCashAccountDebtor4>(value);

            result.Should().NotBeNull();
            result.Identification.Should().Be(value.Identification);
            result.Name.Should().Be(value.Name);
            result.SchemeName.Should().Be(value.SchemeName);
            result.SecondaryIdentification.Should().Be(value.SecondaryIdentification);
        }

        [Fact]
        public void OBWriteDomesticDataInitiationInstructedAmount_To_V111()
        {
            var value = new OBWriteDomesticDataInitiationInstructedAmount
            {
                Amount = 1234.56.ToString(),
                Currency = "GBP"
            };

            var result = _entityMapper.Map<OBInternational2InstructedAmount>(value);

            result.Should().NotBeNull();
            result.Amount.Should().Be(value.Amount);
            result.Currency.Should().Be(value.Currency);
        }

        [Fact]
        public void OBWriteDomesticDataInitiationRemittanceInformation_To_V111()
        {
            var value = new OBWriteDomesticDataInitiationRemittanceInformation
            {
                Reference = "reference",
                Unstructured = "unstructured"
            };

            var result = _entityMapper.Map<OBRemittanceInformation1>(value);

            result.Should().NotBeNull();
            result.Reference.Should().Be(value.Reference);
            result.Unstructured.Should().Be(value.Unstructured);
        }

        [Fact]
        public void OBWriteDomesticConsentDataAuthorisation_To_V111()
        {
            var value = new OBWriteDomesticConsentDataAuthorisation
            {
                AuthorisationType = AuthorisationType.Single,
                CompletionDateTime = DateTime.UtcNow
            };

            var result = _entityMapper.Map<OBAuthorisation1>(value);

            result.Should().NotBeNull();
            result.AuthorisationType.ToString().Should().Be(value.AuthorisationType.ToString());
            result.CompletionDateTime.Should().Be(value.CompletionDateTime);
        }

        [Fact]
        public void OBRisk_To_V111()
        {
            var value = new OBRisk
            {
                DeliveryAddress = new OBRiskDeliveryAddress
                {
                    TownName = "Accrington Stanley",
                    Country = "UK",
                    AddressLine = Enumerable.Range(1, 3).Select(i => i.ToString()).ToList(),
                    BuildingNumber = "building number",
                    CountrySubDivision = Enumerable.Range(10, 3).Select(i => i.ToString()).ToList(),
                    PostCode = "post code",
                    StreetName = "street name"
                },
                MerchantCategoryCode = "a",
                PaymentContextCode = PaymentContextCode.BillPayment,
                MerchantCustomerIdentification = "merchant Customer Identification"
            };

            var result = _entityMapper.Map<OBRisk1>(value);

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
