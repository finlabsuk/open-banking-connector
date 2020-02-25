// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FsCheck;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries
{
    public static class NonNullWhitespaceStringArbitrary
    {
        public static Arbitrary<string> GetArbitrary()
        {
            return Arb.Default.String().Generator
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArbitrary();
        }
    }
}
