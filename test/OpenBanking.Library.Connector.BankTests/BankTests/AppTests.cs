// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.BankGroups;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.
    DomesticPaymentConsent;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.
    DomesticVrpConsent;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Fapi;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Response;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using ObSealCertificateRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObSealCertificate;
using ObWacCertificateRequest =
    FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Management.Request.ObWacCertificate;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

[TestClass]
public class AppTests
{
    private static AppContextFixture _appContextFixture = null!;
    private static BankTestingFixture _classLevelWebApplicationFactory = null!;

    public static IEnumerable<object[]>
        GetDynamicClientRegistrationTestCases() =>
        GetTestCases(TestType.DynamicClientRegistration);

    [DataTestMethod]
    [DynamicData(nameof(GetDynamicClientRegistrationTestCases), DynamicDataSourceType.Method)]
    [DoNotParallelize]
    public async Task DynamicClientRegistration(BankTestData bankTestData) => await TestAllInner(bankTestData);

    public static IEnumerable<object[]>
        GetAccountAccessConsentTestCases() =>
        GetTestCases(TestType.AccountAccessConsent);

    [DataTestMethod]
    [DynamicData(nameof(GetAccountAccessConsentTestCases), DynamicDataSourceType.Method)]
    public async Task AccountAccessConsent(BankTestData bankTestData) => await TestAllInner(bankTestData);

    public static IEnumerable<object[]>
        GetDomesticPaymentConsentTestCases() =>
        GetTestCases(TestType.DomesticPaymentConsent);

    [DataTestMethod]
    [DynamicData(nameof(GetDomesticPaymentConsentTestCases), DynamicDataSourceType.Method)]
    public async Task DomesticPaymentConsent(BankTestData bankTestData) => await TestAllInner(bankTestData);

    public static IEnumerable<object[]>
        GetDomesticVrpConsentTestCases() =>
        GetTestCases(TestType.DomesticVrpConsent);

    [DataTestMethod]
    [DynamicData(nameof(GetDomesticVrpConsentTestCases), DynamicDataSourceType.Method)]
    public async Task DomesticVrpConsent(BankTestData bankTestData) => await TestAllInner(bankTestData);

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        Console.WriteLine("AppTest ClassInitialize");
        _classLevelWebApplicationFactory = new BankTestingFixture();
        using HttpClient
            httpClient = _classLevelWebApplicationFactory
                .CreateClient(); // seems required to ensure application fully set up
        _appContextFixture = new AppContextFixture();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _appContextFixture.Dispose();
        _classLevelWebApplicationFactory.Dispose();
    }

    public static IEnumerable<object[]> GetTestCases(TestType testType)
    {
        // Read bank registrations list
        string bankRegistrationEnvFile = Path.Combine(
            AppConfiguration.RequestsDirectory,
            "BankRegistration",
            "http-client.private.env.json");
        BankRegistrationEnvFile bankRegistrationEnvs = DataFile.ReadFile<BankRegistrationEnvFile>(
            bankRegistrationEnvFile,
            new JsonSerializerOptions()).GetAwaiter().GetResult();

        // Loop through test groups
        foreach (BankRegistrationEnv bankRegistrationEnv in bankRegistrationEnvs.Values)
        {
            bool testConsent = testType switch
            {
                TestType.DynamicClientRegistration => bankRegistrationEnv.TestDynamicClientRegistration,
                TestType.AccountAccessConsent => bankRegistrationEnv.TestAccountAccessConsent,
                TestType.DomesticPaymentConsent => bankRegistrationEnv.TestDomesticPaymentConsent,
                TestType.DomesticVrpConsent => bankRegistrationEnv.TestDomesticVrpConsent,
                _ => throw new ArgumentOutOfRangeException(nameof(testType), testType, null)
            };

            if (!testConsent)
            {
                continue;
            }

            string referenceName = bankRegistrationEnv.ReferenceName;
            if (referenceName.Length > 15)
            {
                throw new InvalidOperationException("ReferenceName too long.");
            }
            string softwareStatement = bankRegistrationEnv.SoftwareStatement;
            RegistrationScopeEnum registrationScope = bankRegistrationEnv.RegistrationScope;
            BankProfileEnum bankProfileFromEnv = bankRegistrationEnv.BankProfile;
            string bankRegistrationExternalApiId = bankRegistrationEnv.ExternalApiBankRegistrationId;
            string? bankRegistrationExternalApiSecretName = bankRegistrationEnv.ExternalApiClientSecretName;
            string? bankRegistrationRegistrationAccessTokenName =
                bankRegistrationEnv.ExternalApiRegistrationAccessTokenName;

            // Get info specific to test type
            bool testAuthFromEnv;
            string? accountAccessConsentExternalApiId = null;
            string? testCreditorAccount = null;
            switch (testType)
            {
                case TestType.AccountAccessConsent:
                    accountAccessConsentExternalApiId = bankRegistrationEnv.ExternalApiAccountAccessConsentId;
                    testAuthFromEnv = bankRegistrationEnv.TestAccountAccessConsentAuth;
                    break;
                case TestType.DomesticPaymentConsent:
                    testCreditorAccount =
                        bankRegistrationEnv.TestCreditorAccountDomesticPaymentConsent ??
                        throw new InvalidOperationException(
                            "No TestCreditorAccountDomesticPaymentConsent specified for domestic payment consent test.");
                    testAuthFromEnv = bankRegistrationEnv.TestDomesticPaymentConsentAuth;
                    break;
                case TestType.DomesticVrpConsent:
                    testCreditorAccount =
                        bankRegistrationEnv.TestCreditorAccountDomesticVrpConsent ??
                        throw new InvalidOperationException(
                            "No TestCreditorAccountDomesticVrpConsent specified for domestic VRP consent test.");
                    testAuthFromEnv = bankRegistrationEnv.TestDomesticVrpConsentAuth;
                    break;
                default:
                    testAuthFromEnv = false;
                    break;
            }

            // Get consent auth data (for sandboxes)
            string? authUiInputUserName = bankRegistrationEnv.SandboxAuthUserName;
            string? authUiInputPassword = bankRegistrationEnv.SandboxAuthPassword;
            string? authUiExtraWord1 = bankRegistrationEnv.SandboxAuthExtraWord1;
            string? authUiExtraWord2 = bankRegistrationEnv.SandboxAuthExtraWord2;
            string? authUiExtraWord3 = bankRegistrationEnv.SandboxAuthExtraWord3;

            // Add test case
            IEnumerable<BankProfileEnum> bankProfiles;
            if (bankRegistrationEnv.TestAllRegistrationGroup)
            {
                BankGroup bankGroup = bankProfileFromEnv.GetBankGroup();
                bankProfiles = bankGroup switch
                {
                    BankGroup.Barclays => GetAllInRegistrationGroup<BarclaysBank, BarclaysRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Cooperative => GetAllInRegistrationGroup<CooperativeBank, CooperativeRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Danske => GetAllInRegistrationGroup<DanskeBank, DanskeRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Hsbc => GetAllInRegistrationGroup<HsbcBank, HsbcRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Lloyds => GetAllInRegistrationGroup<LloydsBank, LloydsRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Monzo => GetAllInRegistrationGroup<MonzoBank, MonzoRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Nationwide => GetAllInRegistrationGroup<NationwideBank, NationwideRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.NatWest => GetAllInRegistrationGroup<NatWestBank, NatWestRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Obie => GetAllInRegistrationGroup<ObieBank, ObieRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Revolut => GetAllInRegistrationGroup<RevolutBank, RevolutRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Santander => GetAllInRegistrationGroup<SantanderBank, SantanderRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    BankGroup.Starling => GetAllInRegistrationGroup<StarlingBank, StarlingRegistrationGroup>(
                        bankProfileFromEnv,
                        registrationScope,
                        bankGroup),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            else
            {
                bankProfiles = [bankProfileFromEnv];
            }

            foreach (BankProfileEnum bankProfile in bankProfiles)
            {
                var testAuth = false;
                if (bankProfile == bankProfileFromEnv)
                {
                    testAuth = testAuthFromEnv;
                }

                yield return
                [
                    new BankTestData
                    {
                        ReferenceName = referenceName,
                        SoftwareStatement = softwareStatement,
                        BankProfile = bankProfile,
                        BankRegistrationExternalApiId = bankRegistrationExternalApiId,
                        BankRegistrationExternalApiSecretName = bankRegistrationExternalApiSecretName,
                        BankRegistrationRegistrationAccessTokenName =
                            bankRegistrationRegistrationAccessTokenName,
                        AccountAccessConsentExternalApiId = accountAccessConsentExternalApiId,
                        AccountAccessConsentAuthContextNonce = null,
                        RegistrationScope = registrationScope,
                        AuthUiInputUserName = authUiInputUserName,
                        AuthUiInputPassword = authUiInputPassword,
                        AuthUiExtraWord1 = authUiExtraWord1,
                        AuthUiExtraWord2 = authUiExtraWord2,
                        AuthUiExtraWord3 = authUiExtraWord3,
                        TestType = testType,
                        TestAuth = testAuth,
                        TestCreditorAccount = testCreditorAccount
                    }
                ];
            }
        }

        static IEnumerable<BankProfileEnum> GetAllInRegistrationGroup<TBank, TRegistrationGroup>(
            BankProfileEnum bankProfile,
            RegistrationScopeEnum registrationScope,
            BankGroup bankGroup)
            where TBank : struct, Enum
            where TRegistrationGroup : struct, Enum
        {
            IBankGroupData<TBank, TRegistrationGroup> bankGroupData =
                bankGroup.GetBankGroupData<TBank, TRegistrationGroup>();
            TBank bank = bankGroupData.GetBank(bankProfile);
            TRegistrationGroup registrationGroup = bankGroupData.GetRegistrationGroup(bank, registrationScope);
            IEnumerable<BankProfileEnum> bankProfiles =
                Enum.GetValues<TBank>()
                    .Where(
                        x =>
                            registrationGroup.Equals(bankGroupData.GetRegistrationGroup(x, registrationScope)))
                    .Select(x => bankGroupData.GetBankProfile(x));
            return bankProfiles;
        }
    }

    private async Task TestAllInner(BankTestData testData)
    {
        Console.WriteLine("AppTest Start");
        BankTestingFixture testLevelWebApplicationFactory = _classLevelWebApplicationFactory;
        using HttpClient httpClient = testLevelWebApplicationFactory.CreateClient();

        IServiceProvider testServiceProvider = _appContextFixture.Host.Services;
        IServiceProvider appServiceProvider = testLevelWebApplicationFactory.Services;
        var webAppClient = new WebAppClient(httpClient);
        var managementApiClient = new ManagementApiClient(webAppClient);
        var authContextsApiClient = new AuthContextsApiClient(webAppClient);
        var accountAccessConsentSubtest =
            new AccountAccessConsentSubtest(new AccountAndTransactionApiClient(webAppClient), authContextsApiClient);
        var domesticPaymentConsentSubtest =
            new DomesticPaymentConsentSubtest(new PaymentInitiationApiClient(webAppClient), authContextsApiClient);
        var domesticVrpConsentSubtest =
            new DomesticVrpConsentSubtest(new VariableRecurringPaymentsApiClient(webAppClient), authContextsApiClient);

        // Set test name
        string testName =
            $"{testData.BankProfile}_{testData.SoftwareStatement}_" +
            $"{testData.RegistrationScope.AbbreviatedName()}_{testData.TestType}";
        var testNameUnique = $"{testName}_{Guid.NewGuid()}";

        // Get bank test settings
        BankTestSettings bankTestSettings =
            testServiceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

        // Get logger
        var instrumentationClient = testServiceProvider.GetRequiredService<IInstrumentationClient>();

        // Get bank profile definitions
        var bankProfileDefinitions =
            testServiceProvider.GetRequiredService<IBankProfileService>();
        BankProfile bankProfile =
            bankProfileDefinitions.GetBankProfile(testData.BankProfile);

        // Get bank user
        BankUser? bankUser = testData.AuthUiInputUserName is not null
            ? new BankUser
            {
                UserNameOrNumber = testData.AuthUiInputUserName,
                Password = testData.AuthUiInputPassword ?? string.Empty,
                ExtraWord1 = testData.AuthUiExtraWord1 ?? string.Empty,
                ExtraWord2 = testData.AuthUiExtraWord2 ?? string.Empty,
                ExtraWord3 = testData.AuthUiExtraWord3 ?? string.Empty
            }
            : null;

        // Get application memory cache
        var memoryCache = appServiceProvider.GetRequiredService<IMemoryCache>();

        // Create test data writers
        var topLevelFolderName = "genericAppTests";
        var testDataProcessorFluentRequestLogging = new FilePathBuilder(
            Path.Combine(bankTestSettings.GetDataDirectoryForCurrentOs(), $"{topLevelFolderName}/fluent"),
            testName,
            ".json");

        FilePathBuilder? testDataProcessorApiLogging = null;
        if (bankTestSettings.LogExternalApiData)
        {
            testDataProcessorApiLogging = new FilePathBuilder(
                Path.Combine(bankTestSettings.GetDataDirectoryForCurrentOs(), $"{topLevelFolderName}/api"),
                testName,
                ".json");
        }

        // Create consent auth
        PlaywrightLaunchOptions launchOptions =
            bankTestSettings.Auth.PlaywrightLaunch;
        if (launchOptions is null)
        {
            throw new ArgumentNullException($"{nameof(launchOptions)}");
        }
        var browserTypeLaunchOptions = new BrowserTypeLaunchOptions
        {
            Args = launchOptions.ProcessedArgs,
            ExecutablePath = launchOptions.ProcessedExecutablePath,
            Headless = launchOptions.Headless,
            SlowMo = launchOptions.ProcessedSlowMo,
            Timeout = launchOptions.TimeOut
        };
        EmailOptions emailOptions = bankTestSettings.Auth.Email;
        var consentAuth = new ConsentAuth(browserTypeLaunchOptions, emailOptions, bankProfileDefinitions);

        var modifiedBy = "Automated bank tests";

        // Create and read software statement (incl. certificates)
        string softwareStatementEnvFile = Path.Combine(
            AppConfiguration.RequestsDirectory,
            "SoftwareStatement",
            "http-client.private.env.json");
        var softwareStatementEnvs = await DataFile.ReadFile<SoftwareStatementEnvFile>(
            softwareStatementEnvFile,
            new JsonSerializerOptions());
        SoftwareStatementEnv softwareStatementEnv =
            softwareStatementEnvs.Values.FirstOrDefault(x => x.SoftwareStatementName == testData.SoftwareStatement) ??
            throw new InvalidOperationException(
                $"Software statement {testData.SoftwareStatement} specified but not found.");
        (ObWacCertificateResponse obWacCertificateResponse, ObSealCertificateResponse obSealCertificateResponse,
            SoftwareStatementResponse softwareStatementResponse) = await SoftwareStatementCreate(
            softwareStatementEnv,
            modifiedBy,
            testNameUnique,
            managementApiClient);
        Guid obWacCertificateId = obWacCertificateResponse.Id;
        Guid obSealCertificateId = obSealCertificateResponse.Id;
        Guid softwareStatementId = softwareStatementResponse.Id;

        // Create BankRegistrationRequest
        var bankRegistrationRequest = new BankRegistration
        {
            BankProfile = bankProfile.BankProfileEnum,
            SoftwareStatementId = softwareStatementId,
            RegistrationScope = testData.RegistrationScope,
            Reference = testNameUnique,
            CreatedBy = modifiedBy
        };

        if (testData.TestType is TestType.DynamicClientRegistration)
        {
            // Create fresh BankRegistration
            BankRegistrationResponse bankRegistrationResponseTmp =
                await BankRegistrationCreate(
                    bankRegistrationRequest,
                    bankProfile.BankConfigurationApiSettings.UseRegistrationGetEndpoint,
                    managementApiClient);

            // Delete BankRegistration (includes external API delete as appropriate)
            await BankRegistrationDelete(bankRegistrationResponseTmp.Id, false, managementApiClient);

            return;
        }

        // Create BankRegistration using existing external API registration
        bankRegistrationRequest.ExternalApiId =
            testData.BankRegistrationExternalApiId ??
            throw new InvalidOperationException("No external API BankRegistration ID provided.");
        if (testData.BankRegistrationExternalApiSecretName is not null)
        {
            bankRegistrationRequest.ExternalApiSecretFromSecrets =
                new SecretDescription { Name = testData.BankRegistrationExternalApiSecretName };
        }
        if (testData.BankRegistrationRegistrationAccessTokenName is not null)
        {
            bankRegistrationRequest.RegistrationAccessTokenFromSecrets =
                new SecretDescription { Name = testData.BankRegistrationRegistrationAccessTokenName };
        }
        BankRegistrationResponse bankRegistrationCreateResponse =
            await BankRegistrationCreate(
                bankRegistrationRequest,
                bankProfile.BankConfigurationApiSettings.UseRegistrationGetEndpoint,
                managementApiClient);
        Guid bankRegistrationId = bankRegistrationCreateResponse.Id;
        OAuth2ResponseMode defaultResponseMode =
            bankRegistrationCreateResponse.DefaultResponseModeOverride ?? bankProfile.DefaultResponseMode;

        string redirectUri = GetRedirectUri(
            defaultResponseMode,
            bankRegistrationCreateResponse.DefaultFragmentRedirectUri,
            bankRegistrationCreateResponse.DefaultQueryRedirectUri,
            softwareStatementResponse);
        string authUrlLeftPart =
            new Uri(redirectUri)
                .GetLeftPart(UriPartial.Authority);

        // Run account access consent subtests
        if (testData.TestType is TestType.AccountAccessConsent)
        {
            if (!testData.RegistrationScope.HasFlag(RegistrationScopeEnum.AccountAndTransaction))
            {
                throw new InvalidOperationException(
                    "Cannot test AccountAndTransaction API due to missing registration scope.");
            }

            if (bankProfile.AccountAndTransactionApi is null)
            {
                throw new InvalidOperationException(
                    "Cannot test AccountAndTransaction API as no API specified in bank profile.");
            }

            foreach (AccountAccessConsentSubtestEnum subTest in
                     AccountAccessConsentSubtest.AccountAccessConsentSubtestsSupported(bankProfile))
            {
                await accountAccessConsentSubtest.RunTest(
                    subTest,
                    bankProfile,
                    testData,
                    bankRegistrationId,
                    defaultResponseMode,
                    testData.TestAuth,
                    testNameUnique,
                    modifiedBy,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("aisp")
                        .AppendToPath($"{subTest.ToString()}"),
                    consentAuth,
                    authUrlLeftPart,
                    bankUser,
                    appServiceProvider,
                    memoryCache);
            }
        }

        if (testData.TestType is TestType.DomesticPaymentConsent or TestType.DomesticVrpConsent)
        {
            // Read payments .env file
            string paymentsEnvFileName = Path.Combine(
                AppConfiguration.RequestsDirectory,
                "Payments",
                "http-client.private.env.json");
            var paymentsEnvFile = await DataFile.ReadFile<PaymentsEnvFile>(
                paymentsEnvFileName,
                new JsonSerializerOptions());
            string creditorAccount =
                testData.TestCreditorAccount ??
                throw new InvalidOperationException("No test creditor account specified for test.");
            if (!paymentsEnvFile.TryGetValue(
                    creditorAccount,
                    out PaymentsEnv? paymentsEnv))
            {
                throw new InvalidOperationException($"Creditor account {creditorAccount} specified but not found.");
            }

            // Run domestic payment consent subtests
            if (testData.TestType is TestType.DomesticPaymentConsent)
            {
                if (!testData.RegistrationScope.HasFlag(RegistrationScopeEnum.PaymentInitiation))
                {
                    throw new InvalidOperationException(
                        "Cannot test PaymentInitiation API due to missing registration scope.");
                }

                if (bankProfile.PaymentInitiationApi is null)
                {
                    throw new InvalidOperationException(
                        "Cannot test PaymentInitiation API as no API specified in bank profile.");
                }

                foreach (DomesticPaymentSubtestEnum subTest in
                         DomesticPaymentConsentSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
                {
                    await domesticPaymentConsentSubtest.RunTest(
                        subTest,
                        bankProfile,
                        bankRegistrationId,
                        defaultResponseMode,
                        testData.TestAuth,
                        testData.ReferenceName,
                        paymentsEnv,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("pisp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        authUrlLeftPart,
                        bankUser);
                }
            }

            // Run domestic VRP consent subtests
            if (testData.TestType is TestType.DomesticVrpConsent)
            {
                if (!testData.RegistrationScope.HasFlag(RegistrationScopeEnum.PaymentInitiation))
                {
                    throw new InvalidOperationException(
                        "Cannot test VariableRecurringPayments API due to missing registration scope.");
                }

                if (bankProfile.VariableRecurringPaymentsApi is null)
                {
                    throw new InvalidOperationException(
                        "Cannot test VariableRecurringPayments API as no API specified in bank profile.");
                }

                foreach (DomesticVrpSubtestEnum subTest in
                         DomesticVrpConsentSubtest.DomesticVrpFunctionalSubtestsSupported(bankProfile))
                {
                    await domesticVrpConsentSubtest.RunTest(
                        subTest,
                        bankProfile,
                        bankRegistrationId,
                        defaultResponseMode,
                        testData.TestAuth,
                        testData.ReferenceName,
                        paymentsEnv,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("vrp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        authUrlLeftPart,
                        bankUser,
                        appServiceProvider,
                        memoryCache);
                }
            }
        }

        // Delete BankRegistration (excludes external API delete)
        await BankRegistrationDelete(bankRegistrationId, true, managementApiClient);

        await SoftwareStatementDelete(
            obWacCertificateId,
            obSealCertificateId,
            softwareStatementId,
            managementApiClient);
    }

    private string GetRedirectUri(
        OAuth2ResponseMode responseMode,
        string? registrationFragmentRedirectUrl,
        string? registrationQueryRedirectUrl,
        SoftwareStatementResponse softwareStatementResponse) =>
        responseMode switch
        {
            OAuth2ResponseMode.Query =>
                registrationQueryRedirectUrl ??
                softwareStatementResponse.DefaultQueryRedirectUrl,
            OAuth2ResponseMode.Fragment =>
                registrationFragmentRedirectUrl ??
                softwareStatementResponse.DefaultFragmentRedirectUrl,
            //OAuth2ResponseMode.FormPost => expr,
            _ => throw new ArgumentOutOfRangeException(nameof(responseMode), responseMode, null)
        };

    private async
        Task<(BaseResponse obWacCertificateDeleteResponse, BaseResponse obSealCertificateDeleteResponse, BaseResponse
            softwareStatementDeleteResponse)> SoftwareStatementDelete(
            Guid obWacCertificateId,
            Guid obSealCertificateId,
            Guid softwareStatementId,
            ManagementApiClient managementApiClient)
    {
        BaseResponse obWacCertificateDeleteResponse =
            await managementApiClient.ObWacCertificateDelete(obWacCertificateId);

        BaseResponse obSealCertificateDeleteResponse =
            await managementApiClient.ObSealCertificateDelete(obSealCertificateId);

        BaseResponse softwareStatementDeleteResponse =
            await managementApiClient.SoftwareStatementDelete(softwareStatementId);

        return (obWacCertificateDeleteResponse, obSealCertificateDeleteResponse, softwareStatementDeleteResponse);
    }

    private async
        Task<(ObWacCertificateResponse obWacCertificateResponse, ObSealCertificateResponse obSealCertificateResponse,
            SoftwareStatementResponse softwareStatementResponse)> SoftwareStatementCreate(
            SoftwareStatementEnv softwareStatementEnv,
            string reference,
            string createdBy,
            ManagementApiClient managementApiClient)
    {
        // Create OBWAC certificate
        var obWacRequest = new ObWacCertificateRequest
        {
            Reference = reference,
            CreatedBy = createdBy,
            AssociatedKey = new SecretDescription { Name = softwareStatementEnv.ObWacAssociatedKeyName },
            Certificate = softwareStatementEnv.ObWacCertificate
        };
        ObWacCertificateResponse obWacCertificateResponse =
            await managementApiClient.ObWacCertificateCreate(obWacRequest);

        // Read OBWAC certificate
        _ = await managementApiClient.ObWacCertificateRead(obWacCertificateResponse.Id);

        // Create OBSeal certificate
        var obSealRequest = new ObSealCertificateRequest
        {
            Reference = reference,
            CreatedBy = createdBy,
            AssociatedKeyId = softwareStatementEnv.ObSealAssociatedKeyId,
            AssociatedKey = new SecretDescription { Name = softwareStatementEnv.ObSealAssociatedKeyName },
            Certificate = softwareStatementEnv.ObSealCertificate
        };
        ObSealCertificateResponse obSealCertificateResponse =
            await managementApiClient.ObSealCertificateCreate(obSealRequest);

        // Read OBSeal certificate
        _ = await managementApiClient.ObSealCertificateRead(obSealCertificateResponse.Id);

        // Create software statement
        var softwareStatementRequest = new SoftwareStatement
        {
            Reference = reference,
            CreatedBy = createdBy,
            OrganisationId = softwareStatementEnv.OrganisationId,
            SoftwareId = softwareStatementEnv.SoftwareId,
            SandboxEnvironment = softwareStatementEnv.SandboxEnvironment,
            DefaultObWacCertificateId = obWacCertificateResponse.Id,
            DefaultObSealCertificateId = obSealCertificateResponse.Id,
            DefaultQueryRedirectUrl = softwareStatementEnv.DefaultQueryRedirectUrl,
            DefaultFragmentRedirectUrl = softwareStatementEnv.DefaultFragmentRedirectUrl
        };
        SoftwareStatementResponse softwareStatementResponse =
            await managementApiClient.SoftwareStatementCreate(softwareStatementRequest);

        // Read software statement
        _ = await managementApiClient.SoftwareStatementRead(softwareStatementResponse.Id);

        return (obWacCertificateResponse, obSealCertificateResponse, softwareStatementResponse);
    }

    private async Task<BankRegistrationResponse> BankRegistrationCreate(
        BankRegistration bankRegistrationRequest,
        bool bankProfileUseRegistrationGetEndpoint,
        ManagementApiClient managementApiClient)
    {
        // Create BankRegistration
        BankRegistrationResponse bankRegistrationCreateResponse =
            await managementApiClient.BankRegistrationCreate(bankRegistrationRequest);

        // Checks
        if (bankRegistrationRequest.ExternalApiId is not null)
        {
            bankRegistrationCreateResponse.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            bankRegistrationCreateResponse.ExternalApiResponse.Should().NotBeNull();
        }

        // Read BankRegistration
        await BankRegistrationRead(
            bankRegistrationCreateResponse.Id,
            false,
            bankProfileUseRegistrationGetEndpoint,
            managementApiClient);

        return bankRegistrationCreateResponse;
    }

    private async Task<BankRegistrationResponse> BankRegistrationRead(
        Guid bankRegistrationId,
        bool excludeExternalApiOperation,
        bool bankProfileUseRegistrationGetEndpoint,
        ManagementApiClient managementApiClient)
    {
        BankRegistrationResponse bankRegistrationReadResponse = await managementApiClient.BankRegistrationRead(
            new BankRegistrationReadParams
            {
                ExcludeExternalApiOperation = excludeExternalApiOperation,
                Id = bankRegistrationId,
                ModifiedBy = null
            });

        // Check ExternalApiResponse
        bool noExternalApiOperation =
            excludeExternalApiOperation ||
            !bankProfileUseRegistrationGetEndpoint;
        if (noExternalApiOperation)
        {
            bankRegistrationReadResponse.ExternalApiResponse.Should().BeNull();
        }
        else
        {
            bankRegistrationReadResponse.ExternalApiResponse.Should().NotBeNull();
        }

        return bankRegistrationReadResponse;
    }

    private async Task<BaseResponse> BankRegistrationDelete(
        Guid bankRegistrationId,
        bool excludeExternalApiOperation,
        ManagementApiClient managementApiClient)
    {
        BaseResponse baseResponse = await managementApiClient.BankRegistrationDelete(
            new BankRegistrationDeleteParams
            {
                ExcludeExternalApiOperation = excludeExternalApiOperation,
                Id = bankRegistrationId,
                ModifiedBy = null
            });

        return baseResponse;
    }
}
