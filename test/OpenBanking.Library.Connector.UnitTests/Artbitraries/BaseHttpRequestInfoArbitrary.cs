// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FsCheck;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries
{
    public static class BaseHttpRequestInfoArbitrary
    {
        public static Arbitrary<HttpRequestInfo> GetArbitrary()
        {
            var elements = new[] { new HttpRequestInfo() };

            return Gen.Elements(elements).ToArbitrary();
        }
    }
}
