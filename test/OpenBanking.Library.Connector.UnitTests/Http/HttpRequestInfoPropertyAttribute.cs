// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;
using FsCheck.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Http;

public class HttpRequestInfoPropertyAttribute : PropertyAttribute
{
    public HttpRequestInfoPropertyAttribute()
    {
        Verbose = PropertyTests.VerboseTests;
        Arbitrary = new[]
        {
            typeof(BaseUrlArbitrary), typeof(BaseMockArbitrary<ICredentials>), typeof(BaseArbitrary<Cookie>),
            typeof(BaseArbitrary<HttpHeader>), typeof(BaseArbitrary<X509Certificate>),
            typeof(BaseMockArbitrary<IWebProxy>), typeof(BaseMockArbitrary<IServerCertificateValidator>)
        };
    }
}
