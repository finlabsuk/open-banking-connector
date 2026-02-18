// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FsCheck;
using FsCheck.Fluent;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;

public static class NullWhitespaceStringArbitrary
{
    public static Arbitrary<string> GetArbitrary() =>
        ArbMap.Default.ArbFor<string>().Generator
            .Where(string.IsNullOrWhiteSpace)
            .ToArbitrary();
}
