﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.GenericHost;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public;
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
            Skip = "Bank skipped due to setting of" +
                   nameof(BankProfile.ClientRegistrationApiSettings.UseRegistrationScope) + "in bank profile")]
        [MemberData(
            nameof(TestedUnskippedBanksById),
            true)]
        public async Task TestAll(
            BankProfileEnum bank,
            SoftwareStatementProfileData softwareStatementProfile,
            RegistrationScope registrationScope)
        {
            // Connect output to logging
            SetTestLogging();

            await TestAllInner(
                bank,
                softwareStatementProfile,
                registrationScope,
                () => new ScopedRequestBuilderContainer(_serviceProvider),
                true);

            UnsetTestLogging();
        }

        private void SetTestLogging()
        {
            _appContextFixture.OutputHelper = _outputHelper;
        }

        private void UnsetTestLogging()
        {
            _appContextFixture.OutputHelper = null;
        }
    }
}
