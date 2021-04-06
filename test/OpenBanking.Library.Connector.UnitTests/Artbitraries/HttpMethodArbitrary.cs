// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http;
using FsCheck;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries
{
    public static class HttpMethodArbitrary
    {
        public static Arbitrary<HttpMethod> GetArbitrary()
        {
            HttpMethod[] elements =
            {
                HttpMethod.Get, HttpMethod.Post, HttpMethod.Put, HttpMethod.Delete, HttpMethod.Head, HttpMethod.Options,
                HttpMethod.Trace
            };

            return Gen.Elements(elements).ToArbitrary();
        }
    }
}
