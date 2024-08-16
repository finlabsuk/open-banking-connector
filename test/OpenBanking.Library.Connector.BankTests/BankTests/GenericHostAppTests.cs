// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

[Collection("App context collection")]
public class GenericHostAppTests : AppTests, IClassFixture<BankTestingFixture>
{
    public GenericHostAppTests(
        ITestOutputHelper outputHelper,
        AppContextFixture appContextFixture,
        BankTestingFixture bankTestingFixture) : base(
        outputHelper,
        appContextFixture,
        bankTestingFixture) { }

    [Theory]
    [MemberData(
        nameof(TestedSkippedBanksById),
        true,
        Skip = "Bank skipped due to setting of" +
               nameof(BankProfile.BankConfigurationApiSettings.RegistrationScopeIsValid) + "in bank profile")]
    [MemberData(
        nameof(TestedUnskippedBanksById),
        true)]
    public async Task TestAll(
        BankTestData1 testGroup, // name chosen to customise label in test runner
        BankTestData2 bankProfile) // name chosen to customise label in test runner
    {
        // Connect output to logging
        SetTestLogging();

        await TestAllInner(testGroup, bankProfile, true);

        UnsetTestLogging();
    }
}
