// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FsCheck;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;

public static class BaseUrlArbitrary
{
    public static Arbitrary<Uri> GetArbitrary()
    {
        string[] urls = { "http://test.com", "https://test.com" };

        return Gen.Elements(urls).Select(s => new Uri(s)).ToArbitrary();
    }
}
