// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;
using FsCheck.Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Http;

public class TestHttpRequestBuilder : HttpRequestBuilder
{
    public HttpRequestInfo GetRequestInfo() => RequestInfo;
}

public class HttpRequestBuilderTests
{
    [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = [typeof(UriArbitrary)])]
    public bool SetUri(Uri value)
    {
        var b = (TestHttpRequestBuilder) new TestHttpRequestBuilder().SetUri(value);

        return b.GetRequestInfo().RequestUri == value;
    }

    [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(HttpMethodArbitrary) })]
    public bool SetMethod(HttpMethod value)
    {
        var b = (TestHttpRequestBuilder) new TestHttpRequestBuilder().SetMethod(value);

        return b.GetRequestInfo().Method == value;
    }

    [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = [typeof(HeaderValueArbitrary)])]
    public bool SetHeaders(IList<string> values)
    {
        var headerName = "hdr";
        List<HttpHeader> headers =
            values.NullToEmpty().Select(v => new HttpHeader(headerName, v)).ToList();

        var b = (TestHttpRequestBuilder) new TestHttpRequestBuilder().SetHeaders(headers);

        return b.GetRequestInfo().Headers.SequenceEqual(headers);
    }
}
