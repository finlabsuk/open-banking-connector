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
                    
                    // Handle included and excluded banks
                    List<BankProfileEnum> includedBanks = bankRegistrationType.IncludedBanks;
                    if (!includedBanks.Any())
                    {
                        List<BankProfileEnum> excludedBanks = bankRegistrationType.ExcludedBanks;
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
            Func<IRequestBuilderContainer> requestBuilderGenerator,
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
