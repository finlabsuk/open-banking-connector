// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.BrowserInteraction;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.AccountAndTransaction.
    AccountAccessConsent;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.DomesticVrp;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public abstract class AppTests
{
    public enum AccountAccessConsentOptions
    {
        OnlyCreateConsent,
        TestConsent,
        OnlyDeleteConsent
    }

    public enum BankRegistrationOptions
    {
        /// <summary>
        ///     Test creation of bank registration.
        /// </summary>
        OnlyCreateRegistration,

        /// <summary>
        ///     Test creation then deletion of bank registration.
        ///     If possible, force external bank registration deletion even if
        ///     ExternalApi registration supplied via BankRegistrationExternalApiIds rather than created.
        /// </summary>
        OnlyDeleteRegistration,

        /// <summary>
        ///     Test all possible endpoints (maximal test).
        /// </summary>
        TestRegistration
    }

    private readonly AppContextFixture _appContextFixture;
    private readonly ITestOutputHelper _outputHelper;
    protected readonly IServiceProvider _serviceProvider;

    protected AppTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture)
    {
        _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
        _serviceProvider = appContextFixture.Host.Services;
        _appContextFixture = appContextFixture;
    }

    public static TheoryData<BankTestData1, BankTestData2>
        TestedSkippedBanksById(bool genericAppNotPlainAppTest) =>
        TestedBanksById(true, genericAppNotPlainAppTest);

    public static TheoryData<BankTestData1, BankTestData2>
        TestedUnskippedBanksById(bool genericAppNotPlainAppTest) =>
        TestedBanksById(false, genericAppNotPlainAppTest);

    public static TheoryData<BankTestData1, BankTestData2> TestedBanksById(
        bool skippedNotUnskipped,
        bool genericAppNotPlainAppTest)
    {
        // Get bank test settings
        var bankTestSettings = AppConfiguration.GetSettings<BankTestSettings>();
        //var env = AppConfiguration.EnvironmentName;

        // Get bank profile definitions
        var bankProfilesSettings = AppConfiguration.GetSettings<BankProfilesSettings>();
        var bankProfilesSettingsProvider =
            new DefaultSettingsProvider<BankProfilesSettings>(bankProfilesSettings);
        var bankProfileDefinitions =
            new BankProfileService(bankProfilesSettingsProvider);
        var data =
            new TheoryData<BankTestData1, BankTestData2>();

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

                // Determine whether test case should be skipped based on registration scope
                BankProfile bankProfile = bankProfileDefinitions.GetBankProfile(bankProfileEnum);
                bool registrationScopeValid =
                    bankProfile.BankConfigurationApiSettings.RegistrationScopeIsValid(testGroup.RegistrationScope);
                bool testCaseShouldBeSkipped = !registrationScopeValid;

                // Add test case to theory data if skip status matches that of theory data
                if (testCaseShouldBeSkipped == skippedNotUnskipped)
                {
                    data.Add(
                        new BankTestData1
                        {
                            TestGroupName = groupName,
                            SoftwareStatementProfileId = testGroup.SoftwareStatementProfileId,
                            SoftwareStatementAndCertificateProfileOverride = overrideCase,
                            RegistrationScope = testGroup.RegistrationScope
                        },
                        new BankTestData2
                        {
                            BankProfileEnum = bankProfileEnum,
                            BankRegistrationExternalApiId = bankRegistrationExternalApiId,
                            BankRegistrationExternalApiSecret = bankRegistrationExternalApiSecret,
                            BankRegistrationRegistrationAccessToken =
                                bankRegistrationRegistrationAccessToken,
                            AccountAccessConsentExternalApiId = accountAccessConsentExternalApiId,
                            AccountAccessConsentRefreshToken = accountAccessConsentRefreshToken,
                            AccountAccessConsentAuthContextNonce = accountAccessConsentAuthContextNonce
                        });
                }
            }
        }

        return data;
    }

    protected async Task TestAllInner(
        BankTestData1 testData1,
        BankTestData2 testData2,
        Func<IRequestBuilderContainer> requestBuilderGenerator,
        bool genericNotPlainAppTest)
    {
        // Set test name
        var testName =
            $"{testData2.BankProfileEnum}_{testData1.SoftwareStatementProfileId}_{testData1.RegistrationScope.AbbreviatedName()}";
        var testNameUnique = $"{testName}_{Guid.NewGuid()}";

        // Set test type
        var testType = BankRegistrationOptions.TestRegistration;

        // Get bank test settings
        BankTestSettings bankTestSettings =
            _serviceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

        // Get bank profile definitions
        var bankProfileDefinitions =
            _serviceProvider.GetRequiredService<IBankProfileService>();
        BankProfile bankProfile = bankProfileDefinitions.GetBankProfile(testData2.BankProfileEnum);

        // Get bank users
        List<BankUser> bankUserList =
            _serviceProvider.GetRequiredService<BankUserStore>()
                .GetRequiredBankUserList(testData2.BankProfileEnum);

        // Get API client
        var processedSoftwareStatementProfileStore =
            _serviceProvider.GetRequiredService<IProcessedSoftwareStatementProfileStore>();
        ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
            await processedSoftwareStatementProfileStore.GetAsync(
                testData1.SoftwareStatementProfileId,
                testData1.SoftwareStatementAndCertificateProfileOverride);
        IApiClient apiClient = processedSoftwareStatementProfile.ApiClient;

        // Get request builder
        using IRequestBuilderContainer requestBuilderContainer = requestBuilderGenerator();
        IRequestBuilder requestBuilder = requestBuilderContainer.RequestBuilder;

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
        bool useConsentAuth = genericNotPlainAppTest && testType is BankRegistrationOptions.TestRegistration;
        if (useConsentAuth)
        {
            PlaywrightLaunchOptions launchOptions =
                bankTestSettings.ConsentAuth.PlaywrightLaunch;

            if (launchOptions is null)
            {
                throw new ArgumentNullException($"{nameof(launchOptions)}");
            }

            var browserTypeLaunchOptions = new BrowserTypeLaunchOptions
            {
                Args = launchOptions.ProcessedArgs,
                Devtools = launchOptions.DevTools,
                ExecutablePath = launchOptions.ProcessedExecutablePath,
                Headless = launchOptions.Headless,
                SlowMo = launchOptions.ProcessedSlowMo,
                Timeout = launchOptions.TimeOut
            };

            EmailOptions emailOptions = bankTestSettings.ConsentAuth.Email;

            consentAuth = new ConsentAuth(browserTypeLaunchOptions, emailOptions, bankProfileDefinitions);
        }
        else
        {
            consentAuth = null;
        }

        var modifiedBy = "Automated bank tests";

        // CREATE and READ bank configuration objects
        Guid bankRegistrationId =
            await BankConfigurationSubtests.PostAndGetObjects(
                testData1,
                testData2,
                requestBuilder,
                bankProfile,
                testNameUnique,
                modifiedBy,
                testDataProcessorFluentRequestLogging
                    .AppendToPath("config"));

        if (testType is BankRegistrationOptions.TestRegistration)
        {
            // Run account access consent subtests
            if (testData1.RegistrationScope.HasFlag(RegistrationScopeEnum.AccountAndTransaction))
            {
                foreach (AccountAccessConsentSubtestEnum subTest in
                         AccountAccessConsentSubtest.AccountAccessConsentSubtestsSupported(bankProfile))
                {
                    await AccountAccessConsentSubtest.RunTest(
                        subTest,
                        bankProfile,
                        testData2,
                        bankRegistrationId,
                        bankProfile.AccountAndTransactionApiSettings,
                        requestBuilder,
                        requestBuilderGenerator,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("config"),
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("aisp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        bankUserList,
                        AccountAccessConsentOptions.TestConsent,
                        apiClient);
                }
            }

            // Run domestic payment consent subtests
            if (testData1.RegistrationScope.HasFlag(RegistrationScopeEnum.PaymentInitiation))
            {
                foreach (DomesticPaymentSubtestEnum subTest in
                         DomesticPaymentSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
                {
                    await DomesticPaymentSubtest.RunTest(
                        subTest,
                        bankProfile,
                        bankRegistrationId,
                        bankProfile.PaymentInitiationApiSettings,
                        requestBuilder,
                        requestBuilderGenerator,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("config"),
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("pisp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        bankUserList,
                        apiClient);
                }

                // Run domestic VRP consent subtests
                foreach (DomesticVrpSubtestEnum subTest in
                         DomesticVrpSubtest.DomesticVrpFunctionalSubtestsSupported(bankProfile))
                {
                    await DomesticVrpSubtest.RunTest(
                        subTest,
                        bankProfile,
                        bankRegistrationId,
                        bankProfile.VariableRecurringPaymentsApiSettings,
                        requestBuilder,
                        requestBuilderGenerator,
                        testNameUnique,
                        modifiedBy,
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("config"),
                        testDataProcessorFluentRequestLogging
                            .AppendToPath("vrp")
                            .AppendToPath($"{subTest.ToString()}"),
                        consentAuth,
                        bankUserList);
                }
            }
        }


        // Delete bank configuration objects
        await BankConfigurationSubtests.DeleteObjects(
            testData2,
            requestBuilder,
            modifiedBy,
            bankRegistrationId,
            bankProfile,
            testType);
    }

    protected void SetTestLogging()
    {
        _appContextFixture.OutputHelper = _outputHelper;
    }

    protected void UnsetTestLogging()
    {
        _appContextFixture.OutputHelper = null;
    }
}
