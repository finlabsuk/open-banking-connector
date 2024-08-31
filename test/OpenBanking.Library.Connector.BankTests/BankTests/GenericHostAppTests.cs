// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

[TestClass]
public class GenericHostAppTests : AppTests
{
    public static IEnumerable<object[]>
        TestedUnskippedBanksById() =>
        TestedBanksById(false, true);

    [DataTestMethod]
    [DynamicData(nameof(TestedUnskippedBanksById), DynamicDataSourceType.Method)]
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
