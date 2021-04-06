// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Configuration;
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
            //var env = AppConfiguration.EnvironmentName;

            TheoryData<BankProfileEnum, BankRegistrationType> data =
                new TheoryData<BankProfileEnum, BankRegistrationType>();

            foreach (var bankRegistrationType in bankTestSettings.TestedBankRegistrationTypes)
            {
                // Get bank whitelist if available
                bool bankWhitelistAvailable;
                List<BankProfileEnum>? bankWhitelist;
                if (genericAppNotPlainAppTest)
                {
                    bankWhitelistAvailable =
                        bankTestSettings.BankWhitelists.GenericHostAppTests.TryGetValue(
                            bankRegistrationType.SoftwareStatementProfileId,
                            out bankWhitelist);
                }
                else
                {
                    bankWhitelistAvailable =
                        bankTestSettings.BankWhitelists.PlainAppTests.TryGetValue(
                            bankRegistrationType.SoftwareStatementProfileId,
                            out bankWhitelist);
                }

                foreach (BankProfileEnum bankEnum in BankProfileEnumHelper.AllBanks)
                {
                    BankProfile bankProfile = BankProfileEnumHelper.GetBank(bankEnum);

                    // If whitelist available, only test banks on whitelist
                    if (bankWhitelistAvailable && !bankWhitelist!.Contains(bankProfile.BankProfileEnum))
                    {
                        continue;
                    }

                    // Determine skip status based on bank support and add to Theory if matches skippedNotUnskipped
                    bool testCaseSkipped =
                        !bankProfile.RegistrationScopeSupported(bankRegistrationType.RegistrationScope);
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
    }
}
