// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles.Sandbox;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
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
        protected readonly string _dataFolder;
        protected readonly ITestOutputHelper _outputHelper;
        protected readonly IServiceProvider _serviceProvider;

        protected AppTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture)
        {
            _outputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            _serviceProvider = appContextFixture.Host.Services;
            _dataFolder = appContextFixture.DataFolder;
        }

        public static TheoryData<BankProfileEnum, BankRegistrationType> TestedSkippedBanksById(
            bool genericAppNotPlainAppTest) =>
            TestedBanksById(true, genericAppNotPlainAppTest);

        public static TheoryData<BankProfileEnum, BankRegistrationType> TestedUnskippedBanksById(
            bool genericAppNotPlainAppTest) =>
            TestedBanksById(false, genericAppNotPlainAppTest);

        public static TheoryData<BankProfileEnum, BankRegistrationType> TestedBanksById(
            bool skippedNotUnskipped,
            bool genericAppNotPlainAppTest)
        {
            // Get bank test settings
            BankTestSettings bankTestSettings = AppConfiguration.GetSettings<BankTestSettings>();
            bankTestSettings.Validate();
            //var env = AppConfiguration.EnvironmentName;

            // Get bank profile definitions
            BankProfileSettings bankProfileSettings = AppConfiguration.GetSettings<BankProfileSettings>();
            bankProfileSettings.Validate();
            Dictionary<string, Dictionary<string, BankProfileHiddenProperties>> bankProfileHiddenProperties =
                DataFile.ReadFile<Dictionary<string, Dictionary<string, BankProfileHiddenProperties>>>(
                    bankProfileSettings.HiddenPropertiesFile,
                    new JsonSerializerSettings()).GetAwaiter().GetResult();
            var bankProfileDefinitions =
                new BankProfileDefinitions(bankProfileHiddenProperties);

            TheoryData<BankProfileEnum, BankRegistrationType> data =
                new TheoryData<BankProfileEnum, BankRegistrationType>();

            // Loop through tested banks
            List<BankProfileEnum> testedBanks = genericAppNotPlainAppTest
                ? bankTestSettings.TestedBanks.GenericHostAppTests
                : bankTestSettings.TestedBanks.PlainAppTests;
            foreach (BankProfileEnum bankEnum in testedBanks)
            {
                BankProfile bankProfile = BankProfileEnumHelper.GetBank(
                    bankEnum,
                    bankProfileDefinitions);
                foreach (BankRegistrationType bankRegistrationType in bankTestSettings.TestedBankRegistrationTypes)
                {
                    // Ignore excluded banks
                    List<BankProfileEnum> excludedBanks = bankRegistrationType.ExcludedBanks;
                    if (excludedBanks.Contains(bankProfile.BankProfileEnum))
                    {
                        continue;
                    }

                    // Determine skip status based on registration scope and add to Theory if matches skippedNotUnskipped
                    bool testCaseSkipped =
                        !bankProfile.ClientRegistrationApiSettings.UseRegistrationScope(
                            bankRegistrationType.RegistrationScope);
                    if (testCaseSkipped == skippedNotUnskipped)
                    {
                        data.Add(
                            bankEnum,
                            bankRegistrationType);
                    }
                }
            }

            return data;
        }

        protected async Task TestAllInner(
            BankProfileEnum bank,
            BankRegistrationType bankRegistrationType,
            IRequestBuilder requestBuilder,
            Func<IScopedRequestBuilder>? requestBuilderGenerator,
            bool genericNotPlainAppTest)
        {
            // Test name
            var testName =
                $"{bank}_{bankRegistrationType.SoftwareStatementProfileId}_{bankRegistrationType.RegistrationScope.AbbreviatedName()}";
            var testNameUnique = $"{testName}_{Guid.NewGuid()}";

            // Get bank test settings
            BankTestSettings bankTestSettings =
                _serviceProvider.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

            // Get bank profile definitions
            BankProfileDefinitions bankProfileDefinitions =
                _serviceProvider.GetRequiredService<BankProfileDefinitions>();

            // Get bank users
            List<BankUser> bankUserList =
                _serviceProvider.GetRequiredService<BankUsers>()
                    .GetRequiredBankUserList(bank);

            // Get consent authoriser inputs
            INodeJSService nodeJsService = _serviceProvider.GetRequiredService<INodeJSService>();
            // OutOfProcessNodeJSServiceOptions outOfProcessNodeJSServiceOptions =
            //     services.GetRequiredService<IOptions<OutOfProcessNodeJSServiceOptions>>().Value;
            // NodeJSProcessOptions nodeJSProcessOptions =
            //     services.GetRequiredService<IOptions<NodeJSProcessOptions>>().Value;
            PuppeteerLaunchOptionsJavaScript puppeteerLaunchOptions =
                bankTestSettings.ConsentAuthoriser.PuppeteerLaunch.ToJavaScript();

            // Create test data writers
            string topLevelFolderName = genericNotPlainAppTest ? "genericAppTests" : "plainAppTests";
            TestDataWriter testDataProcessorFluentRequestLogging = new TestDataWriter(
                Path.Combine(bankTestSettings.GetDataDirectoryForCurrentOs(), $"{topLevelFolderName}/fluent"),
                testName);

            TestDataWriter? testDataProcessorApiLogging = null;
            if (bankTestSettings.LogExternalApiData)
            {
                testDataProcessorApiLogging = new TestDataWriter(
                    Path.Combine(bankTestSettings.GetDataDirectoryForCurrentOs(), $"{topLevelFolderName}/api"),
                    testName);
            }

            TestDataWriter testDataProcessorApiOverrides = new TestDataWriter(
                Path.Combine(
                    bankTestSettings.GetDataDirectoryForCurrentOs(),
                    $"{topLevelFolderName}/apiOverrides"),
                testName);

            // Dereference bank
            BankProfile bankProfile = BankProfileEnumHelper.GetBank(bank, bankProfileDefinitions);

            (Guid bankId, Guid bankRegistrationId, Guid bankApiInformationId) =
                await ClientRegistrationSubtests.PostAndGetObjects(
                    bankRegistrationType.SoftwareStatementProfileId,
                    bankRegistrationType.RegistrationScope,
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
            foreach (DomesticPaymentFunctionalSubtestEnum subTest in
                DomesticPaymentFunctionalSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
            {
                await DomesticPaymentFunctionalSubtest.RunTest(
                    subTest,
                    bankProfile,
                    bankRegistrationId,
                    bankApiInformationId,
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

            await ClientRegistrationSubtests.DeleteObjects(
                requestBuilder,
                bankApiInformationId,
                bankRegistrationId,
                bankId,
                bankProfile.ClientRegistrationApiSettings);
        }
    }
}
