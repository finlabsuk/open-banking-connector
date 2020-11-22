﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;
using Meta = FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model.Meta;
using OBAddressTypeCode =
    FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model.OBAddressTypeCode;
using OBPostalAddress6 = FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p2.Model.OBPostalAddress6;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Model.Mapping
{
    public class EntityMapperTests
    {
        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property OpenBankingToPublic_SimpleMap_V3_1_2_RC1_ToPublic_2(ObModels.PaymentInitiation.V3p1p4.Model.OBPostalAddress6 value)
        {
            var mapper = new EntityMapper();

            Func<bool> rule = () =>
            {
                var r1 = mapper.Map(value, typeof(OBPostalAddress6)) as
                    OBPostalAddress6;

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


            return rule.When(value != null);
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property OpenBankingToPublic_ComplexTree_V3_1_2_ToPublic(
            string name,
            ObModels.PaymentInitiation.V3p1p4.Model.OBAddressTypeCode obAddressTypeCode)
        {
            var mapper = new EntityMapper();

            var obPostalAddress = new ObModels.PaymentInitiation.V3p1p4.Model.OBPostalAddress6
            {
                AddressType = obAddressTypeCode
            };
            var value = new OBWriteInternational3DataInitiationCreditor(name, obPostalAddress);

            Func<bool> rule = () =>
            {
                var r1 = mapper.Map(value,
                        typeof(OBWriteInternational2DataInitiationCreditor)) as
                    OBWriteInternational2DataInitiationCreditor;

                return r1 != null &&
                       r1.Name == value.Name &&
                       r1.PostalAddress != null &&
                       r1.PostalAddress.BuildingNumber == value.PostalAddress.BuildingNumber;
            };


            return rule.When(value != null && value.PostalAddress != null);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public Property OpenBankingToPublic_ComplexTree_V3_1_2_ToPublic_2(OBAddressTypeCode addrType, string name)
        {
            var mapper = new EntityMapper();

            Func<bool> rule = () =>
            {
                var value =
                    new OBWriteInternational2DataInitiationCreditor(name, new OBPostalAddress6(addrType));

                var r1 = mapper.Map(value,
                        typeof(OBWriteInternational3DataInitiationCreditor)) as
                    OBWriteInternational3DataInitiationCreditor;

                return r1 != null &&
                       r1.Name == value.Name &&
                       r1.PostalAddress != null &&
                       r1.PostalAddress.BuildingNumber == value.PostalAddress.BuildingNumber;
            };


            return rule.When(name != null);
        }


        [Fact]
        public void PublicToOpenBanking_SimpleMap_To_v3_1_1()
        {
            var mapper = new EntityMapper();

            var m1 = new Meta
            {
                FirstAvailableDateTime = DateTime.MaxValue,
                LastAvailableDateTime = DateTime.UnixEpoch,
                TotalPages = 123
            };

            var r1 =
                mapper.Map(m1, typeof(ObModels.PaymentInitiation.V3p1p1.Model.Meta)) as
                    ObModels.PaymentInitiation.V3p1p1.Model.Meta;

            r1.Should().NotBeNull();
            m1.FirstAvailableDateTime.Should().Be(r1.FirstAvailableDateTime);
            m1.LastAvailableDateTime.Should().Be(r1.LastAvailableDateTime);
            m1.TotalPages.Should().Be(r1.TotalPages);
        }

        [Fact]
        public void PublicToOpenBanking_SimpleMap_To_v3_1_2()
        {
            var mapper = new EntityMapper();

            var m1 = new Meta
            {
                FirstAvailableDateTime = DateTimeOffset.MaxValue,
                LastAvailableDateTime = DateTimeOffset.UnixEpoch,
                TotalPages = 123
            };

            var r1 =
                mapper.Map(m1, typeof(ObModels.PaymentInitiation.V3p1p2.Model.Meta)) as
                    ObModels.PaymentInitiation.V3p1p2.Model.Meta;

            r1.Should().NotBeNull();
            m1.FirstAvailableDateTime.Should().Be(r1.FirstAvailableDateTime);
            m1.LastAvailableDateTime.Should().Be(r1.LastAvailableDateTime);
            m1.TotalPages.Should().Be(r1.TotalPages);
        }


        [Fact]
        public void OpenBankingToPublic_SimpleMap_V3_1_1_ToPublic()
        {
            var mapper = new EntityMapper();

            var m1 = new ObModels.PaymentInitiation.V3p1p1.Model.Meta
            {
                FirstAvailableDateTime = DateTime.MaxValue,
                LastAvailableDateTime = DateTime.UnixEpoch,
                TotalPages = 123
            };

            var r1 = mapper.Map(m1, typeof(Meta)) as Meta;

            r1.Should().NotBeNull();

            m1.FirstAvailableDateTime.Should().Be(r1.FirstAvailableDateTime);
            m1.LastAvailableDateTime.Should().Be(r1.LastAvailableDateTime);
            m1.TotalPages.Should().Be(r1.TotalPages);
        }

        [Fact]
        public void OpenBankingToPublic_SimpleMap_V3_1_2_ToPublic()
        {
            var mapper = new EntityMapper();

            var m1 = new ObModels.PaymentInitiation.V3p1p2.Model.Meta
            {
                FirstAvailableDateTime = DateTimeOffset.MaxValue,
                LastAvailableDateTime = DateTimeOffset.UnixEpoch,
                TotalPages = 123
            };

            var r1 = mapper.Map(m1, typeof(Meta)) as Meta;

            r1.Should().NotBeNull();

            m1.FirstAvailableDateTime.Should().Be(r1.FirstAvailableDateTime);
            m1.LastAvailableDateTime.Should().Be(r1.LastAvailableDateTime);
            m1.TotalPages.Should().Be(r1.TotalPages);
        }
    }
}
