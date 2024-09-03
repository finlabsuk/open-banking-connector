// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Operations;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
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
        TestedUnskippedBanksById() =>
        TestedBanksById(false, true);

    [DataTestMethod]
    [DynamicData(nameof(TestedUnskippedBanksById), DynamicDataSourceType.Method)]
    public async Task GenericHostAppTests(
        BankTestData1 testGroup, // name chosen to customise label in test runner
        BankTestData2 bankProfile) // name chosen to customise label in test runner
    {
        await TestAllInner(testGroup, bankProfile, true);
    }

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

    public static IEnumerable<object[]> TestedBanksById(
        bool skippedNotUnskipped,
        bool genericAppNotPlainAppTest)
    {
        // Get bank test settings
        var bankTestSettings = AppConfiguration.GetSettings<BankTestSettings>();
        //var env = AppConfiguration.EnvironmentName;

        // Loop through test groups
        foreach ((string groupName, TestGroup testGroup) in bankTestSettings.TestGroups)
        {
            List<BankProfileEnum> testedBanks = genericAppNotPlainAppTest
                ? testGroup.GenericHostAppTests
                : testGroup.PlainAppTests;

            // Loop through tested banks
            foreach (BankProfileEnum bankProfileEnum in testedBanks)
            {
                // Get override for software statement and certificate profiles
                testGroup
                    .SoftwareStatementAndCertificateProfileOverrides
                    .TryGetValue(bankProfileEnum, out string? overrideCase);

                // Get external API BankRegistration ID
                testGroup
                    .BankRegistrationExternalApiIds
                    .TryGetValue(bankProfileEnum, out string? bankRegistrationExternalApiId);

                // Get external API BankRegistration secret
                testGroup
                    .BankRegistrationExternalApiSecrets
                    .TryGetValue(bankProfileEnum, out string? bankRegistrationExternalApiSecret);

                // Get external API BankRegistration registration access token
                testGroup
                    .BankRegistrationRegistrationAccessTokens
                    .TryGetValue(bankProfileEnum, out string? bankRegistrationRegistrationAccessToken);

                // Get external API AccountAccessConsent ID
                testGroup
                    .AccountAccessConsentExternalApiIds
                    .TryGetValue(bankProfileEnum, out string? accountAccessConsentExternalApiId);

                // Get external API AccountAccessConsent refresh token
                testGroup
                    .AccountAccessConsentRefreshTokens
                    .TryGetValue(bankProfileEnum, out string? accountAccessConsentRefreshToken);

                // Get external API AccountAccessConsent refresh token
                testGroup
                    .AccountAccessConsentAuthContextNonces
                    .TryGetValue(bankProfileEnum, out string? accountAccessConsentAuthContextNonce);

                // Get consent auth data (for sandboxes)
                bankTestSettings
                    .AuthData
                    .TryGetValue(bankProfileEnum, out AuthData? authData);

                testGroup
                    .TestDomesticPaymentConsent
                    .TryGetValue(bankProfileEnum, out bool testDomesticPaymentConsent);

                testGroup
                    .TestDomesticVrpConsent
                    .TryGetValue(bankProfileEnum, out bool testDomesticVrpConsent);

                bool? authDisable = authData?.DisableAuth;
                string? authUiInputUserName = authData?.UiInput?.UserName; // can only be null when UiInput null
                string? authUiInputPassword = authData?.UiInput?.Password; // can only be null when UiInput null
                string? authUiExtraWord1 = authData?.UiInput?.ExtraWord1;
                string? authUiExtraWord2 = authData?.UiInput?.ExtraWord2;
                string? authUiExtraWord3 = authData?.UiInput?.ExtraWord3;

                // Add test case to theory data if skip status matches that of theory data
                if (!skippedNotUnskipped)
                {
                    yield return
                    [
                        new BankTestData1
                        {
                            TestGroupName = groupName,
                            SoftwareStatementProfileId = testGroup.SoftwareStatementProfileId,
                            SoftwareStatementAndCertificateProfileOverride = overrideCase
                        },
                        new BankTestData2
                        {
                            BankProfileEnum = bankProfileEnum,
                            BankRegistrationExternalApiId = bankRegistrationExternalApiId,
                            BankRegistrationExternalApiSecret = bankRegistrationExternalApiSecret,
                            BankRegistrationRegistrationAccessToken =
                                bankRegistrationRegistrationAccessToken,
                            AccountAccessConsentExternalApiId = accountAccessConsentExternalApiId,
                            AccountAccessConsentAuthContextNonce = accountAccessConsentAuthContextNonce,
                            RegistrationScope = testGroup.RegistrationScope,
                            AuthDisable = authDisable,
                            AuthUiInputUserName = authUiInputUserName,
                            AuthUiInputPassword = authUiInputPassword,
                            AuthUiExtraWord1 = authUiExtraWord1,
                            AuthUiExtraWord2 = authUiExtraWord2,
                            AuthUiExtraWord3 = authUiExtraWord3,
                            TestDomesticPaymentConsent = testDomesticPaymentConsent,
                            TestDomesticVrpConsent = testDomesticVrpConsent
                        }
                    ];
                }
            }
        }
    }

    private async Task TestAllInner(BankTestData1 testData1, BankTestData2 testData2, bool genericNotPlainAppTest)
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
        var testName =
            $"{testData2.BankProfileEnum}_{testData1.SoftwareStatementProfileId}_{testData2.RegistrationScope.AbbreviatedName()}";
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
            bankProfileDefinitions.GetBankProfile(testData2.BankProfileEnum);

        // Get bank user
        BankUser? bankUser = testData2.AuthUiInputUserName is not null
            ? new BankUser
            {
                UserNameOrNumber = testData2.AuthUiInputUserName,
                Password = testData2.AuthUiInputPassword!, // not null when AuthUiInputUserName not null
                ExtraWord1 = testData2.AuthUiExtraWord1!,
                ExtraWord2 = testData2.AuthUiExtraWord2!,
                ExtraWord3 = testData2.AuthUiExtraWord3!
            }
            : null;

        // Get software statement profile
        var processedSoftwareStatementProfileStore =
            testServiceProvider.GetRequiredService<IProcessedSoftwareStatementProfileStore>();
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await processedSoftwareStatementProfileStore.GetAsync(
                testData1.SoftwareStatementProfileId,
                testData1.SoftwareStatementAndCertificateProfileOverride);

        // Get application memory cache
        var memoryCache = appServiceProvider.GetRequiredService<IMemoryCache>();

        // Create test data writers
        string topLevelFolderName = genericNotPlainAppTest ? "genericAppTests" : "plainAppTests";
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

        // Create consent auth if in use
        ConsentAuth? consentAuth;
        bool useConsentAuth = genericNotPlainAppTest;
        if (useConsentAuth)
        {
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

            consentAuth = new ConsentAuth(browserTypeLaunchOptions, emailOptions, bankProfileDefinitions);
        }
        else
        {
            consentAuth = null;
        }

        var modifiedBy = "Automated bank tests";

        // Create and read software statement (incl. certificates)
        (ObWacCertificateResponse obWacCertificateResponse, ObSealCertificateResponse obSealCertificateResponse,
            SoftwareStatementResponse softwareStatementResponse) = await SoftwareStatementCreate(
            processedSoftwareStatementProfile,
            modifiedBy,
            managementApiClient);
        Guid obWacCertificateId = obWacCertificateResponse.Id;
        Guid obSealCertificateId = obSealCertificateResponse.Id;
        Guid softwareStatementId = softwareStatementResponse.Id;

        // CREATE and READ bank configuration objects
        // Create bankRegistration or use existing
        RegistrationScopeEnum registrationScope = testData2.RegistrationScope;
        BankRegistration bankRegistrationRequest = await BankRegistrationGetRequest(
            bankProfile,
            softwareStatementId,
            registrationScope,
            testDataProcessorFluentRequestLogging,
            testNameUnique,
            modifiedBy);

        if (bankProfile.BankConfigurationApiSettings.TestTemporaryBankRegistration)
        {
            // Create fresh BankRegistration
            BankRegistrationResponse bankRegistrationResponseTmp =
                await BankRegistrationCreate(
                    bankRegistrationRequest,
                    bankProfile.BankConfigurationApiSettings.UseRegistrationGetEndpoint,
                    managementApiClient);

            // Delete BankRegistration (includes external API delete as appropriate)
            await BankRegistrationDelete(bankRegistrationResponseTmp.Id, false, managementApiClient);
        }

        // Create BankRegistration using existing external API registration
        bankRegistrationRequest.ExternalApiId =
            testData2.BankRegistrationExternalApiId ??
            throw new InvalidOperationException("No external API BankRegistration ID provided.");
        if (testData2.BankRegistrationExternalApiSecret is not null)
        {
            bankRegistrationRequest.ExternalApiSecretFromSecrets =
                new SecretDescription { Name = testData2.BankRegistrationExternalApiSecret };
        }
        bankRegistrationRequest.RegistrationAccessToken = testData2.BankRegistrationRegistrationAccessToken;
        BankRegistrationResponse bankRegistrationCreateResponse =
            await BankRegistrationCreate(
                bankRegistrationRequest,
                bankProfile.BankConfigurationApiSettings.UseRegistrationGetEndpoint,
                managementApiClient);
        Guid bankRegistrationId = bankRegistrationCreateResponse.Id;
        OAuth2ResponseMode defaultResponseMode =
            bankRegistrationCreateResponse.DefaultResponseModeOverride ?? bankProfile.DefaultResponseMode;

        // Run account access consent subtests
        string redirectUri = processedSoftwareStatementProfile.GetRedirectUri(
            defaultResponseMode,
            bankRegistrationCreateResponse.DefaultFragmentRedirectUri,
            bankRegistrationCreateResponse.DefaultQueryRedirectUri);
        string authUrlLeftPart =
            new Uri(redirectUri)
                .GetLeftPart(UriPartial.Authority);
        if (registrationScope.HasFlag(RegistrationScopeEnum.AccountAndTransaction))
        {
            foreach (AccountAccessConsentSubtestEnum subTest in
                     AccountAccessConsentSubtest.AccountAccessConsentSubtestsSupported(bankProfile))
            {
                await accountAccessConsentSubtest.RunTest(
                    subTest,
                    bankProfile,
                    testData2,
                    bankRegistrationId,
                    defaultResponseMode,
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

        if (registrationScope.HasFlag(RegistrationScopeEnum.PaymentInitiation))
        {
            // Run domestic payment consent subtests
            if (testData2.TestDomesticPaymentConsent)
            {
                foreach (DomesticPaymentSubtestEnum subTest in
                         DomesticPaymentConsentSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
                {
                    await domesticPaymentConsentSubtest.RunTest(
                        subTest,
                        bankProfile,
                        bankRegistrationId,
                        defaultResponseMode,
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
            if (testData2.TestDomesticVrpConsent)
            {
                foreach (DomesticVrpSubtestEnum subTest in
                         DomesticVrpConsentSubtest.DomesticVrpFunctionalSubtestsSupported(bankProfile))
                {
                    await domesticVrpConsentSubtest.RunTest(
                        subTest,
                        bankProfile,
                        bankRegistrationId,
                        defaultResponseMode,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("vrp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        authUrlLeftPart,
                        bankUser);
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
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile,
            string createdBy,
            ManagementApiClient managementApiClient)
    {
        // Create OBWAC certificate
        string obWacReference = processedSoftwareStatementProfile.TransportCertificateId;
        var obWacRequest = new ObWacCertificateRequest
        {
            Reference = obWacReference,
            CreatedBy = createdBy,
            AssociatedKey = new SecretDescription
            {
                Name =
                    $"OpenBankingConnector:TransportCertificateProfiles:{obWacReference}:AssociatedKey"
            },
            Certificate = processedSoftwareStatementProfile.TransportCertificate
        };
        ObWacCertificateResponse obWacCertificateResponse =
            await managementApiClient.ObWacCertificateCreate(obWacRequest);

        // Read OBWAC certificate
        _ = await managementApiClient.ObWacCertificateRead(obWacCertificateResponse.Id);

        // Create OBSeal certificate
        string obSealReference = processedSoftwareStatementProfile.SigningCertificateId;
        var obSealRequest = new ObSealCertificateRequest
        {
            Reference = obSealReference,
            CreatedBy = createdBy,
            AssociatedKeyId = processedSoftwareStatementProfile.OBSealKey.KeyId,
            AssociatedKey = new SecretDescription
            {
                Name =
                    $"OpenBankingConnector:SigningCertificateProfiles:{obSealReference}:AssociatedKey"
            },
            Certificate = processedSoftwareStatementProfile.SigningCertificate
        };
        ObSealCertificateResponse obSealCertificateResponse =
            await managementApiClient.ObSealCertificateCreate(obSealRequest);

        // Read OBSeal certificate
        _ = await managementApiClient.ObSealCertificateRead(obSealCertificateResponse.Id);

        // Create software statement
        string sReference = processedSoftwareStatementProfile.Id;
        var softwareStatementRequest = new SoftwareStatement
        {
            Reference = sReference,
            CreatedBy = createdBy,
            OrganisationId = processedSoftwareStatementProfile.OrganisationId,
            SoftwareId = processedSoftwareStatementProfile.SoftwareId,
            SandboxEnvironment = processedSoftwareStatementProfile.SandboxEnvironment,
            DefaultObWacCertificateId = obWacCertificateResponse.Id,
            DefaultObSealCertificateId = obSealCertificateResponse.Id,
            DefaultQueryRedirectUrl = processedSoftwareStatementProfile.DefaultQueryRedirectUrl,
            DefaultFragmentRedirectUrl = processedSoftwareStatementProfile.DefaultFragmentRedirectUrl
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

    private static async Task<BankRegistration> BankRegistrationGetRequest(
        BankProfile bankProfile,
        Guid softwareStatementId,
        RegistrationScopeEnum registrationScope,
        FilePathBuilder testDataProcessorFluentRequestLogging,
        string testNameUnique,
        string modifiedBy)
    {
        var registrationRequest = new BankRegistration
        {
            BankProfile = bankProfile.BankProfileEnum,
            SoftwareStatementId = default, // substitute logging placeholder
            RegistrationScope = registrationScope
        };
        await testDataProcessorFluentRequestLogging
            .AppendToPath("manage")
            .AppendToPath("bankRegistration")
            .AppendToPath("postRequest")
            .WriteFile(registrationRequest);
        registrationRequest.SoftwareStatementId = softwareStatementId;
        registrationRequest.Reference = testNameUnique;
        registrationRequest.CreatedBy = modifiedBy;
        return registrationRequest;
    }
}
