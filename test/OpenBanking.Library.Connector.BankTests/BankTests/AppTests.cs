// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.VariableRecurringPayments.DomesticVrp;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Models.Repository;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Repositories;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

        public static TheoryData<BankProfileEnum, string, RegistrationScope> TestedSkippedBanksById(
            bool genericAppNotPlainAppTest) =>
            TestedBanksById(true, genericAppNotPlainAppTest);

        public static TheoryData<BankProfileEnum, string, RegistrationScope> TestedUnskippedBanksById(
            bool genericAppNotPlainAppTest) =>
            TestedBanksById(false, genericAppNotPlainAppTest);

        public static TheoryData<BankProfileEnum, string, RegistrationScope> TestedBanksById(
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
            Dictionary<string, Dictionary<string, BankProfileHiddenProperties>> bankProfileHiddenProperties =
                DataFile.ReadFile<Dictionary<string, Dictionary<string, BankProfileHiddenProperties>>>(
                    bankProfilesSettings.HiddenPropertiesFile,
                    new JsonSerializerSettings()).GetAwaiter().GetResult();
            var bankProfileDefinitions =
                new BankProfileDefinitions(bankProfileHiddenProperties);

            var data =
                new TheoryData<BankProfileEnum, string, RegistrationScope>();

            List<TestCaseGroup> testCases = genericAppNotPlainAppTest
                ? bankTestSettings.GenericHostAppTestCases
                : bankTestSettings.PlainAppTestCases;

            // Loop through test case entries
            foreach (TestCaseGroup testCaseGroup in testCases)
            {
                // Loop through bank profiles
                List<BankProfileEnum> testedBanks = bankTestSettings.TestedBankProfiles;
                foreach (BankProfileEnum bankEnum in testedBanks)
                {
                    BankProfile bankProfile = BankProfileEnumHelper.GetBank(
                        bankEnum,
                        bankProfileDefinitions);

                    // Go no further for bank profiles not satisfying included/excluded filters
                    List<BankProfileEnum> includedBanks = testCaseGroup.IncludedBanks;
                    if (!includedBanks.Any())
                    {
                        List<BankProfileEnum> excludedBanks = testCaseGroup.ExcludedBanks;
                        if (excludedBanks.Contains(bankProfile.BankProfileEnum))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!includedBanks.Contains(bankProfile.BankProfileEnum))
                        {
                            continue;
                        }
                    }

                    // Determine skip status based on registration scope and add to Theory if matches skippedNotUnskipped
                    bool testCaseSkipped =
                        !bankProfile.ClientRegistrationApiSettings.UseRegistrationScope(
                            testCaseGroup.RegistrationScope);
                    if (testCaseSkipped == skippedNotUnskipped)
                    {
                        data.Add(
                            bankEnum,
                            testCaseGroup.SoftwareStatementProfileId,
                            testCaseGroup.RegistrationScope);
                    }
                }
            }

            return data;
        }

        protected async Task TestAllInner(
            BankProfileEnum bank,
            string softwareStatementProfileId,
            RegistrationScope registrationScope,
            Func<IRequestBuilderContainer> requestBuilderGenerator,
            bool genericNotPlainAppTest)
        {
            // Test name
            var testName =
                $"{bank}_{softwareStatementProfileId}_{registrationScope.AbbreviatedName()}";
            var testNameUnique = $"{testName}_{Guid.NewGuid()}";

            // Get bank test settings
            BankTestSettings bankTestSettings =
                _serviceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

            // Get bank profile definitions
            var bankProfileDefinitions =
                _serviceProvider.GetRequiredService<BankProfileDefinitions>();

            // Get bank users
            List<BankUser> bankUserList =
                _serviceProvider.GetRequiredService<BankUserStore>()
                    .GetRequiredBankUserList(bank);

            // Get consent authoriser inputs
            var nodeJsService = _serviceProvider.GetRequiredService<INodeJSService>();
            // OutOfProcessNodeJSServiceOptions outOfProcessNodeJSServiceOptions =
            //     services.GetRequiredService<IOptions<OutOfProcessNodeJSServiceOptions>>().Value;
            // NodeJSProcessOptions nodeJSProcessOptions =
            //     services.GetRequiredService<IOptions<NodeJSProcessOptions>>().Value;
            PuppeteerLaunchOptionsJavaScript puppeteerLaunchOptions =
                bankTestSettings.ConsentAuthoriser.PuppeteerLaunch.ToJavaScript();

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
            BankProfile bankProfile = BankProfileEnumHelper.GetBank(bank, bankProfileDefinitions);

            // Create bank configuration objects
            (Guid bankId, Guid bankRegistrationId, Guid bankApiSetId) =
                await ClientRegistrationSubtests.PostAndGetObjects(
                    softwareStatementProfileId,
                    registrationScope,
                    requestBuilder,
                    bankProfile,
                    testNameUnique,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("clientReg"),
                    testDataProcessorApiLogging?
                        .AppendToPath("clientReg"),
                    testDataProcessorApiOverrides
                        .AppendToPath("clientReg"));

            // Run domestic payment subtests
            foreach (DomesticPaymentSubtestEnum subTest in
                     DomesticPaymentSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
            {
                await DomesticPaymentSubtest.RunTest(
                    subTest,
                    bankProfile,
                    bankRegistrationId,
                    bankApiSetId,
                    bankProfile.PaymentInitiationApiSettings,
                    requestBuilder,
                    requestBuilderGenerator,
                    testNameUnique,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("pisp")
                        .AppendToPath($"{subTest.ToString()}"),
                    genericNotPlainAppTest,
                    nodeJsService,
                    puppeteerLaunchOptions,
                    bankUserList);
            }

            // Run domestic VRP subtests
            foreach (DomesticVrpSubtestEnum subTest in
                     DomesticVrpSubtest.DomesticVrpFunctionalSubtestsSupported(bankProfile))
            {
                await DomesticVrpSubtest.RunTest(
                    subTest,
                    bankProfile,
                    bankRegistrationId,
                    bankApiSetId,
                    bankProfile.VariableRecurringPaymentsApiSettings,
                    requestBuilder,
                    requestBuilderGenerator,
                    testNameUnique,
                    testDataProcessorFluentRequestLogging
                        .AppendToPath("vrp")
                        .AppendToPath($"{subTest.ToString()}"),
                    genericNotPlainAppTest,
                    nodeJsService,
                    puppeteerLaunchOptions,
                    bankUserList);
            }

            // Delete bank configuration objects
            await ClientRegistrationSubtests.DeleteObjects(
                requestBuilder,
                bankApiSetId,
                bankRegistrationId,
                bankId,
                bankProfile.ClientRegistrationApiSettings);
        }
    }
}
