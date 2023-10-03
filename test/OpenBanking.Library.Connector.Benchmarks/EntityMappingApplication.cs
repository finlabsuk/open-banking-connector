// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Running;
using FinnovationLabs.OpenBanking.Library.Connector.Mapping;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using ClientRegistrationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UKObDcr.V3p3.Models;
using PaymentInitiationModelsV3p1p4 =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p4.Pisp.Models;
using PaymentInitiationModelsPublic =
    FinnovationLabs.OpenBanking.Library.BankApiModels.UkObRw.V3p1p6.Pisp.Models;


namespace FinnovationLabs.OpenBanking.Library.Connector.Benchmarks;

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
    private ApiVariantMapper _apiVariantMapper = null!;
    private BankRegistration _client = null!;
    private PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiation _dataInitiation = null!;
    private PaymentInitiationModelsPublic.OBWriteDomesticConsent4 _domesticConsent = null!;
    private PaymentInitiationModelsPublic.OBRisk1 _risk = null!;
    private SoftwareStatementProfile _softwareStatement = null!;
    private TransportCertificateProfile _transportCertificateProfile = null!;


    [GlobalSetup]
    public void GlobalSetup()
    {
        _apiVariantMapper = new ApiVariantMapper();

        _dataInitiation = CreateDataInitiation();
        _risk = CreateRisk();
        _domesticConsent = CreateDomesticConsent();
        _softwareStatement = CreateSoftwareStatement();
        _transportCertificateProfile = CreateObCertificateProfile();
        _client = CreateClient();
    }


    [Benchmark]
    public void MapSoftwareStatement()
    {
        //_entityMapper.Map<Models.KeySecrets.Cached.SoftwareStatementProfile>(_softwareStatement);
    }


    [Benchmark]
    public void MapClientProfile()
    {
        //_entityMapper.Map<Models.Persistent.BankRegistration>(_client);
    }


    [Benchmark]
    public void MapDomesticConsent()
    {
        _apiVariantMapper.Map(_domesticConsent, out PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4 result);
    }

    [Benchmark]
    public void MapRisk()
    {
        _apiVariantMapper.Map(_risk, out PaymentInitiationModelsV3p1p4.OBRisk1 result);
    }

    [Benchmark]
    public void MapDataInitiation()
    {
        _apiVariantMapper.Map(
            _dataInitiation,
            out PaymentInitiationModelsV3p1p4.OBWriteDomesticConsent4DataInitiation result);
    }

    private void OnExecute() => BenchmarkRunner.Run<EntityMappingApplication>();

    private PaymentInitiationModelsPublic.OBRisk1 CreateRisk() => new()
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

    private PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiation CreateDataInitiation() =>
        new()
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
                AddressLine = new List<string>
                {
                    "1 high street",
                    "blexley"
                },
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
            DebtorAccount = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataInitiationDebtorAccount
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
                    Amount = 1234.56.ToString(),
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
        };


    private PaymentInitiationModelsPublic.OBWriteDomesticConsent4 CreateDomesticConsent() =>
        new()
        {
            Data = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4Data
            {
                Initiation = CreateDataInitiation(),
                Authorisation = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataAuthorisation
                {
                    AuthorisationType =
                        PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataAuthorisationAuthorisationTypeEnum
                            .Single,
                    CompletionDateTime = DateTimeOffset.UtcNow
                },
                SCASupportData = new PaymentInitiationModelsPublic.OBWriteDomesticConsent4DataSCASupportData
                {
                    AppliedAuthenticationApproach =
                        PaymentInitiationModelsPublic
                            .OBWriteDomesticConsent4DataSCASupportDataAppliedAuthenticationApproachEnum.SCA,
                    ReferencePaymentOrderId = "reference Payment Order Id",
                    RequestedSCAExemptionType =
                        PaymentInitiationModelsPublic
                            .OBWriteDomesticConsent4DataSCASupportDataRequestedSCAExemptionTypeEnum.PartyToParty
                }
            },
            Risk = CreateRisk()
        };


    private BankRegistration CreateClient() => new()
    {
        SoftwareStatementProfileId = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"
        //XFapiFinancialId = "xfapi"
    };

    private SoftwareStatementProfile CreateSoftwareStatement() => new()
    {
        DefaultFragmentRedirectUrl = "https://aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.com",
        SoftwareStatement =
            "e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30=.e30="
    };

    private TransportCertificateProfile CreateObCertificateProfile() => new()
    {
        AssociatedKey = "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n",
        Certificate = "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
    };
}
