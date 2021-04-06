// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.FunctionalSubtests.PaymentInitiation.DomesticPayment;
using FinnovationLabs.OpenBanking.Library.Connector.Configuration;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests
{
    [Collection("App context collection")]
    public partial class GenericHostAppTests : AppTests
    {
        private readonly AppContextFixture _appContextFixture;

        public GenericHostAppTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture) : base(
            outputHelper,
            appContextFixture)
        {
            _appContextFixture = appContextFixture ?? throw new ArgumentNullException(nameof(appContextFixture));
        }

        [Theory]
        [MemberData(
            nameof(TestedSkippedBanksById),
            true,
            Skip = "Bank skipped due to skip instruction in " + nameof(SkipSettings))]
        [MemberData(
            nameof(TestedUnskippedBanksById),
            true)]
        public async Task GenericHostAppTests2(
            BankProfileEnum bank,
            BankRegistrationType bankRegistrationType)
        {
            await TestAll(bank, bankRegistrationType);
        }

        private void SetTestLogging()
        {
            _appContextFixture.OutputHelper = _outputHelper;
        }

        private void UnsetTestLogging()
        {
            _appContextFixture.OutputHelper = null;
        }

        public async Task TestAll(
            BankProfileEnum bank,
            BankRegistrationType bankRegistrationType)
        {
            // Create test data processor
            TestDataProcessor testDataProcessor = new TestDataProcessor(
                _dataFolder,
                $"{bankRegistrationType.SoftwareStatementProfileId}_{RegistrationScopeApiSetHelper.AbbreviatedName(bankRegistrationType.RegistrationScope)}_{bank}");

            // Connect output to logging
            SetTestLogging();

            // Dereference bank
            BankProfile bankProfile = BankProfileEnumHelper.GetBank(bank);

            // Get services scope
            using IServiceScope serviceScope1 = _serviceProvider.CreateScope();
            IServiceProvider services = serviceScope1.ServiceProvider;
            IRequestBuilder requestBuilder = services.GetRequiredService<IRequestBuilder>();
            INodeJSService nodeJsService = services.GetRequiredService<INodeJSService>();
            // OutOfProcessNodeJSServiceOptions outOfProcessNodeJSServiceOptions =
            //     services.GetRequiredService<IOptions<OutOfProcessNodeJSServiceOptions>>().Value;
            // NodeJSProcessOptions nodeJSProcessOptions =
            //     services.GetRequiredService<IOptions<NodeJSProcessOptions>>().Value;
            BankTestSettings bankTestSettings =
                services.GetRequiredService<ISettingsProvider<BankTestSettings>>().GetSettings();

            (Guid bankId, Guid bankRegistrationId, Guid bankApiInformationId) =
                await new ClientReg().ClientRegistrationTest(
                    bankRegistrationType.SoftwareStatementProfileId,
                    bankRegistrationType.RegistrationScope,
                    requestBuilder,
                    bankProfile,
                    testDataProcessor);

            // Run domestic payment subtests
            foreach (DomesticPaymentFunctionalSubtestEnum subTest in
                DomesticPaymentFunctionalSubtest.DomesticPaymentFunctionalSubtestsSupported(bankProfile))
            {
                bool subtestSkipped = !SkipSettings.DomesticPaymentSubtestUnskippedFilter(
                    subTest,
                    bankProfile,
                    bankRegistrationType.RegistrationScope);
                if (subtestSkipped)
                {
                    continue;
                }

                TestDataProcessor testDataProcessorSubtest =
                    testDataProcessor.AddTestSubfolder($"PI_{subTest.ToString()}");
                await DomesticPaymentFunctionalSubtest.RunTest(
                    subTest,
                    bankProfile,
                    bankRegistrationId,
                    bankApiInformationId,
                    requestBuilder,
                    testDataProcessorSubtest,
                    true,
                    nodeJsService,
                    bankTestSettings.ConsentAuthoriser.GetProcessedPuppeteerLaunch());
            }

            UnsetTestLogging();
        }
    }
}
