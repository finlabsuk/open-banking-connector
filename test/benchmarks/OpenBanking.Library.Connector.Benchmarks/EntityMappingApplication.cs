// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request;
using FinnovationLabs.OpenBanking.Library.Connector.ObModels.PaymentInitiation.V3p1p4.Model;
using McMaster.Extensions.CommandLineUtils;

namespace FinnovationLabs.OpenBanking.Library.Connector.Benchmarks
{
    [Command(Name = "entitymapping", Description = "Run Entity Mapping benchmarks")]
    [InProcess]
    [MemoryDiagnoser]
    [RankColumn]
    [MinColumn]
    [MaxColumn]
    [Q1Column]
    [Q3Column]
    [AllStatisticsColumn]
    [JsonExporterAttribute.Full]
    [CsvMeasurementsExporter]
    [CsvExporter(CsvSeparator.Comma)]
    [HtmlExporter]
    [MarkdownExporterAttribute.GitHub]
    [GcServer(true)]
    public class EntityMappingApplication
    {
        private BankClientProfile _client;
        private OBWriteDomestic2DataInitiation _dataInitiation;
        private OBWriteDomesticConsent4 _domesticConsent;
        private EntityMapper _entityMapper;
        private OBRisk1 _risk;
        private SoftwareStatementProfile _softwareStatement;


        [GlobalSetup]
        public void GlobalSetup()
        {
            _entityMapper = new EntityMapper();

            _dataInitiation = CreateDataInitiation();
            _risk = CreateRisk();
            _domesticConsent = CreateDomesticConsent();
            _softwareStatement = CreateSoftwareStatement();
            _client = CreateClient();
        }


        [Benchmark]
        public void MapSoftwareStatement()
        {
            _entityMapper.Map<Models.Persistent.SoftwareStatementProfile>(_softwareStatement);
        }


        [Benchmark]
        public void MapClientProfile()
        {
            _entityMapper.Map<Models.Persistent.BankClientProfile>(_client);
        }


        [Benchmark]
        public void MapDomesticConsent()
        {
            _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBWriteDomesticConsent2>(_domesticConsent);
        }

        [Benchmark]
        public void MapRisk()
        {
            _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBRisk1>(_risk);
        }

        [Benchmark]
        public void MapDataInitiation()
        {
            _entityMapper.Map<ObModels.PaymentInitiation.V3p1p1.Model.OBDomestic2>(_dataInitiation);
        }

        private void OnExecute() => BenchmarkRunner.Run<EntityMappingApplication>();

        private OBRisk1 CreateRisk() => new OBRisk1
        {
            DeliveryAddress = new OBRisk1DeliveryAddress()
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

        private OBWriteDomestic2DataInitiation CreateDataInitiation() => new OBWriteDomestic2DataInitiation
        {
            CreditorAccount = new OBWriteDomestic2DataInitiationCreditorAccount
            {
                Identification = "id",
                Name = "test",
                SchemeName = "schema",
                SecondaryIdentification = "secondary id"
            },
            CreditorPostalAddress = new OBPostalAddress6
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
        };


        private OBWriteDomesticConsent4 CreateDomesticConsent() => new OBWriteDomesticConsent4
        {
            Data = new OBWriteDomesticConsent4Data
            {
                Initiation = CreateDataInitiation(),
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
            Risk = CreateRisk()
        };


        private BankClientProfile CreateClient() => new BankClientProfile
        {
            IssuerUrl = "https://aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.com",
            HttpMtlsConfigurationOverrides = new HttpMtlsConfigurationOverrides
            {
                TlsCertificateVerification = "aaaa",
                TlsRenegotiationSupport = "bbb"
            },
            BankClientRegistrationDataOverrides = new BankClientRegistrationDataOverrides
            {
                GrantTypes = Enumerable.Range(start: 1, count: 10).Select(i => i.ToString()).ToList()
            },
            BankClientRegistrationClaimsOverrides = new BankClientRegistrationClaimsOverrides
            {
                GrantTypes = Enumerable.Range(start: 1, count: 10).Select(i => i.ToString()).ToList(),
                RequestAudience = "audience",
                ScopeUseStringArray = false,
                SsaIssuer = "issuer",
                TokenEndpointAuthMethod = "method",
                TokenEndpointAuthSigningAlgorithm = "alg"
            },
            OpenIdConfigurationOverrides = new OpenIdConfigurationOverrides
            {
                RegistrationEndpointUrl = "https://ccccccccccccccccccccccccccccccccccccc.com"
            },
            SoftwareStatementProfileId = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
            XFapiFinancialId = "xfapi"
        };

        private SoftwareStatementProfile CreateSoftwareStatement() => new SoftwareStatementProfile
        {
            DefaultFragmentRedirectUrl = "https://aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.com",
            SigningKey = "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n",
            SigningKeyId = "b",
            SigningCertificate = "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n",
            TransportKey = "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n",
            TransportCertificate = "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n",
            SoftwareStatement =
                "e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30="
        };
    }
}
