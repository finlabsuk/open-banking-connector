// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;
using FsCheck;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries
{
    public static class BaseCertificateArbitrary
    {
        public static Arbitrary<X509Certificate> GetArbitrary()
        {
            var elements = new[] { new X509Certificate() };

            return Gen.Elements(elements).ToArbitrary();
        }
    }
}
