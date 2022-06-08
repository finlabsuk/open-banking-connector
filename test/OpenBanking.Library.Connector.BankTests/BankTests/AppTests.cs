// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.AccountAndTransaction.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration.Request;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    public abstract class AppTests
    {
        protected readonly ITestOutputHelper _outputHelper;
        protected readonly IServiceProvider _serviceProvider;

        protected AppTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture)
        {
            _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            _serviceProvider = appContextFixture.Host.Services;
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
            bankTestSettings.Validate();
            //var env = AppConfiguration.EnvironmentName;

            // Get bank profile definitions
            var bankProfilesSettings = AppConfiguration.GetSettings<BankProfilesSettings>();
            var bankProfilesSettingsProvider =
                new DefaultSettingsProvider<BankProfilesSettings>(bankProfilesSettings);
            var bankProfileDefinitions =
                new BankProfileDefinitions(bankProfilesSettingsProvider);
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
                    string? overrideCase =
                        testGroup
                            .SoftwareStatementAndCertificateProfileOverrides
                            .TryGetValue(bankProfileEnum, out string? value1)
                            ? value1
                            : null;

                    // Get BankRegistration
                    BankRegistration? bankRegistrationObject =
                        testGroup
                            .BankRegistrationObjects
                            .TryGetValue(bankProfileEnum, out BankRegistration? value2)
                            ? value2
                            : null;

                    // Get BankRegistration ID
                    Guid? bankRegistrationId =
                        testGroup
                            .BankRegistrationIds
                            .TryGetValue(bankProfileEnum, out Guid value3)
                            ? value3
                            : null;

                    // Get AccountAccessConsent
                    AccountAccessConsent? accountAccessConsentObject =
                        testGroup
                            .AccountAccessConsentObjects
                            .TryGetValue(bankProfileEnum, out AccountAccessConsent? value4)
                            ? value4
                            : null;

                    // Get AccountAccessConsent ID
                    Guid? accountAccessConsentId =
                        testGroup
                            .AccountAccessConsentIds
                            .TryGetValue(bankProfileEnum, out Guid value5)
                            ? value5
                            : null;

                    // Get external API BankRegistration ID
                    string? externalApiBankRegistrationId =
                        testGroup
                            .BankRegistrationExternalApiIds
                            .TryGetValue(bankProfileEnum, out string? value6)
                            ? value6
                            : null;

                    // Get external API AccountAccessConsent ID
                    string? externalApiAccountAccessConsentId =
                        testGroup
                            .AccountAccessConsentExternalApiIds
                            .TryGetValue(bankProfileEnum, out string? value7)
                            ? value7
                            : null;

                    // Get external API AccountAccessConsent refresh token
                    string? externalApiAccountAccessConsentRefreshToken =
                        testGroup
                            .AccountAccessConsentRefreshTokens
                            .TryGetValue(bankProfileEnum, out string? value8)
                            ? value8
                            : null;

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
                                BankRegistrationObject = bankRegistrationObject,
                                BankRegistrationId = bankRegistrationId,
                                AccountAccessConsentId = accountAccessConsentId,
                                BankRegistrationExternalApiId = externalApiBankRegistrationId,
                                AccountAccessConsentExternalApiId = externalApiAccountAccessConsentId,
                                AccountAccessConsentRefreshToken = externalApiAccountAccessConsentRefreshToken
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
            // Test name
            string testName =
                $"{testData2.BankProfileEnum}_{testData1.SoftwareStatementProfileId}_{testData1.RegistrationScope.AbbreviatedName()}";
            string testNameUnique = $"{testName}_{Guid.NewGuid()}";

            // Get bank test settings
            BankTestSettings bankTestSettings =
                _serviceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

            // Get bank profile definitions
            var bankProfileDefinitions =
                _serviceProvider.GetRequiredService<IBankProfileDefinitions>();

            // Get bank users
            List<BankUser> bankUserList =
                _serviceProvider.GetRequiredService<BankUserStore>()
                    .GetRequiredBankUserList(testData2.BankProfileEnum);

            // Get consent authoriser inputs
            var nodeJsService = _serviceProvider.GetRequiredService<INodeJSService>();
            // NodeJSProcessOptions nodeJSProcessOptions =
            //     services.GetRequiredService<IOptions<NodeJSProcessOptions>>().Value;

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

            var testDataProcessorApiOverrides = new FilePathBuilder(
                Path.Combine(
                    bankTestSettings.GetDataDirectoryForCurrentOs(),
                    $"{topLevelFolderName}/apiOverrides"),
                testName,
                ".json");

            // Dereference bank
            BankProfile bankProfile = bankProfileDefinitions.GetBankProfile(testData2.BankProfileEnum);

            // Create consent auth if in use
            ConsentAuth? consentAuth;
            if (genericNotPlainAppTest)
            {
                PuppeteerLaunchOptionsJavaScript puppeteerLaunchOptions =
                    bankTestSettings.ConsentAuthoriser.PuppeteerLaunch.ToJavaScript();

                OutOfProcessNodeJSServiceOptions outOfProcessNodeJSServiceOptions =
                    _serviceProvider.GetRequiredService<IOptions<OutOfProcessNodeJSServiceOptions>>().Value;

                if (puppeteerLaunchOptions is null ||
                    nodeJsService is null)
                {
                    throw new ArgumentNullException($"{nameof(puppeteerLaunchOptions)} or {nameof(nodeJsService)}");
                }

                var launchOptions = new BrowserTypeLaunchOptions
                {
                    Args = puppeteerLaunchOptions.Args,
                    Devtools = puppeteerLaunchOptions.Devtools,
                    ExecutablePath = puppeteerLaunchOptions.ExecutablePath,
                    Headless = puppeteerLaunchOptions.Headless,
                    SlowMo = (float?) puppeteerLaunchOptions.SlowMo,
                    Timeout = outOfProcessNodeJSServiceOptions.TimeoutMS
                };
                consentAuth = new ConsentAuth(launchOptions);
            }
            else
            {
                consentAuth = null;
            }

            // Create bank configuration objects
            (Guid bankId, Guid bankRegistrationId) =
                await BankConfigurationSubtests.PostAndGetObjects(
                    testData1,
                    testData2,
                    requestBuilder,
                    bankProfile,
                    testNameUnique,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("config"),
                    testDataProcessorApiLogging?
                        .AppendToPath("config"),
                    testDataProcessorApiOverrides
                        .AppendToPath("config"));

            // Run account access consent subtests
            foreach (AccountAccessConsentSubtestEnum subTest in
                     AccountAccessConsentSubtest.AccountAccessConsentSubtestsSupported(bankProfile))
            {
                await AccountAccessConsentSubtest.RunTest(
                    subTest,
                    bankProfile,
                    testData2,
                    bankId,
                    bankRegistrationId,
                    bankProfile.AccountAndTransactionApiSettings,
                    requestBuilder,
                    requestBuilderGenerator,
                    testNameUnique,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("config"),
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("aisp")
                        .AppendToPath($"{subTest.ToString()}"),
                    consentAuth,
                    bankUserList,
                    apiClient);
            }

            // Run domestic payment consent subtests
            foreach (DomesticPaymentSubtestEnum subTest in
                     DomesticPaymentSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
            {
                await DomesticPaymentSubtest.RunTest(
                    subTest,
                    bankProfile,
                    bankId,
                    bankRegistrationId,
                    bankProfile.PaymentInitiationApiSettings,
                    requestBuilder,
                    requestBuilderGenerator,
                    testNameUnique,
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
                    bankId,
                    bankRegistrationId,
                    bankProfile.VariableRecurringPaymentsApiSettings,
                    requestBuilder,
                    requestBuilderGenerator,
                    testNameUnique,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("config"),
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("vrp")
                        .AppendToPath($"{subTest.ToString()}"),
                    consentAuth,
                    bankUserList);
            }

            // Delete bank configuration objects
            await BankConfigurationSubtests.DeleteObjects(
                testData2,
                requestBuilder,
                bankRegistrationId,
                bankId,
                bankProfile.BankConfigurationApiSettings);
        }
    }
}
