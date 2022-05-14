﻿// Licensed to Finnovation Labs Limited under one or more agreements.
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
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.BankConfiguration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    public class BankData
    {
        public BankProfile BankProfile { get; set; } = null!;

        public string? OverrideCase { get; set; }
    }

    public abstract class AppTests
    {
        protected readonly ITestOutputHelper _outputHelper;
        protected readonly IServiceProvider _serviceProvider;

        protected AppTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture)
        {
            _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            _serviceProvider = appContextFixture.Host.Services;
        }

        public static TheoryData<BankProfileEnum, SoftwareStatementProfileData, RegistrationScopeEnum>
            TestedSkippedBanksById(bool genericAppNotPlainAppTest) =>
            TestedBanksById(true, genericAppNotPlainAppTest);

        public static TheoryData<BankProfileEnum, SoftwareStatementProfileData, RegistrationScopeEnum>
            TestedUnskippedBanksById(bool genericAppNotPlainAppTest) =>
            TestedBanksById(false, genericAppNotPlainAppTest);

        public static TheoryData<BankProfileEnum, SoftwareStatementProfileData, RegistrationScopeEnum> TestedBanksById(
            bool skippedNotUnskipped,
            bool genericAppNotPlainAppTest)
        {
            // Get bank test settings
            var bankTestSettings = AppConfiguration.GetSettings<BankTestSettings>();
            bankTestSettings.Validate();
            //var env = AppConfiguration.EnvironmentName;

            // Get bank profile definitions
            var bankProfilesSettings = AppConfiguration.GetSettings<BankProfilesSettings>();
            bankProfilesSettings.Validate();
            BankProfileHiddenPropertiesDictionary bankProfileHiddenProperties =
                DataFile.ReadFile<BankProfileHiddenPropertiesDictionary>(
                        bankProfilesSettings.HiddenPropertiesFile,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        }).GetAwaiter()
                    .GetResult();
            var bankProfileDefinitions =
                new BankProfileDefinitions(bankProfileHiddenProperties);

            var data =
                new TheoryData<BankProfileEnum, SoftwareStatementProfileData, RegistrationScopeEnum>();

            // Assemble bank list including override cases
            Dictionary<BankProfileEnum, string> overridesDict = bankTestSettings
                .SoftwareStatementAndCertificateProfileOverrides
                .ToDictionary(x => x.Bank, x => x.OverrideCase);
            List<BankData> bankList = bankTestSettings
                .TestedBanks
                .Select(
                    bankProfileEnum => new BankData
                    {
                        BankProfile = bankProfileDefinitions.GetBankProfile(bankProfileEnum),
                        OverrideCase = overridesDict.TryGetValue(bankProfileEnum, out string? value) ? value : null
                    })
                .ToList();

            // Loop through test case entries
            List<TestGroup> testCases = genericAppNotPlainAppTest
                ? bankTestSettings.GenericHostAppTests
                : bankTestSettings.PlainAppTests;
            foreach (TestGroup testCaseGroup in testCases)
            {
                // Loop through bank profiles
                foreach (BankData bankData in bankList)
                {
                    // Go no further for bank profiles not satisfying included/excluded filters
                    List<BankProfileEnum> includedBanks = testCaseGroup.IncludedBanks;
                    if (!includedBanks.Any())
                    {
                        List<BankProfileEnum> excludedBanks = testCaseGroup.ExcludedBanks;
                        if (excludedBanks.Contains(bankData.BankProfile.BankProfileEnum))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!includedBanks.Contains(bankData.BankProfile.BankProfileEnum))
                        {
                            continue;
                        }
                    }

                    // Determine skip status based on registration scope and add to Theory if matches skippedNotUnskipped
                    bool testCaseSkipped =
                        !bankData.BankProfile.BankConfigurationApiSettings.UseRegistrationScope(
                            testCaseGroup.RegistrationScope);
                    if (testCaseSkipped == skippedNotUnskipped)
                    {
                        data.Add(
                            bankData.BankProfile.BankProfileEnum,
                            new SoftwareStatementProfileData
                            {
                                SoftwareStatementProfileId = testCaseGroup.SoftwareStatementProfileId,
                                OverrideCase = bankData.OverrideCase
                            },
                            testCaseGroup.RegistrationScope);
                    }
                }
            }

            return data;
        }

        protected async Task TestAllInner(
            BankProfileEnum bankProfileEnum,
            SoftwareStatementProfileData softwareStatementProfile,
            RegistrationScopeEnum registrationScope,
            Func<IRequestBuilderContainer> requestBuilderGenerator,
            bool genericNotPlainAppTest)
        {
            // Test name
            string testName =
                $"{bankProfileEnum}_{softwareStatementProfile.SoftwareStatementProfileId}_{registrationScope.AbbreviatedName()}";
            string testNameUnique = $"{testName}_{Guid.NewGuid()}";

            // Get bank test settings
            BankTestSettings bankTestSettings =
                _serviceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

            // Get bank profile definitions
            var bankProfileDefinitions =
                _serviceProvider.GetRequiredService<BankProfileDefinitions>();

            // Get bank users
            List<BankUser> bankUserList =
                _serviceProvider.GetRequiredService<BankUserStore>()
                    .GetRequiredBankUserList(bankProfileEnum);

            // Get consent authoriser inputs
            var nodeJsService = _serviceProvider.GetRequiredService<INodeJSService>();
            // NodeJSProcessOptions nodeJSProcessOptions =
            //     services.GetRequiredService<IOptions<NodeJSProcessOptions>>().Value;

            // Get API client
            var processedSoftwareStatementProfileStore =
                _serviceProvider.GetRequiredService<IProcessedSoftwareStatementProfileStore>();
            ProcessedSoftwareStatementProfile processedSoftwareStatementProfile =
                await processedSoftwareStatementProfileStore.GetAsync(
                    softwareStatementProfile.SoftwareStatementProfileId,
                    softwareStatementProfile.OverrideCase);
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
            BankProfile bankProfile = bankProfileDefinitions.GetBankProfile(bankProfileEnum);

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
                    softwareStatementProfile.SoftwareStatementProfileId,
                    softwareStatementProfile.OverrideCase,
                    registrationScope,
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
                requestBuilder,
                bankRegistrationId,
                bankId,
                bankProfile.BankConfigurationApiSettings);
        }

        public class SoftwareStatementProfileData : IXunitSerializable
        {
            public string SoftwareStatementProfileId { get; set; } = null!;

            public string? OverrideCase { get; set; }

            public void Deserialize(IXunitSerializationInfo info)
            {
                SoftwareStatementProfileId = info.GetValue<string>(nameof(SoftwareStatementProfileId));
                OverrideCase = info.GetValue<string>(nameof(OverrideCase));
            }

            public void Serialize(IXunitSerializationInfo info)
            {
                info.AddValue(nameof(SoftwareStatementProfileId), SoftwareStatementProfileId);
                info.AddValue(nameof(OverrideCase), OverrideCase);
            }

            public override string ToString()
            {
                return $"{SoftwareStatementProfileId}" + (OverrideCase is null ? "" : $" (Override: {OverrideCase})");
            }
        }
    }
}
