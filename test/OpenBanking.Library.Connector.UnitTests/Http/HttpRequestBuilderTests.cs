// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Artbitraries;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Http
{
    public class HttpRequestBuilderTests
    {
        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(UriArbitrary) })]
        public bool SetUri(Uri value)
        {
            var b = new HttpRequestBuilder().SetUri(value) as HttpRequestBuilder;

            return b.RequestInfo.RequestUri == value;
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public bool SetUseDefaultCredentials(bool value)
        {
            var b = new HttpRequestBuilder().SetUseDefaultCredentials(value) as HttpRequestBuilder;

            return b.RequestInfo.UseDefaultCredentials == value;
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public bool SetPreAuthenticate(bool value)
        {
            var b = new HttpRequestBuilder().SetPreAuthenticate(value) as HttpRequestBuilder;

            return b.RequestInfo.PreAuthenticate == value;
        }

        [Property(Verbose = PropertyTests.VerboseTests, Arbitrary = new[] { typeof(HttpMethodArbitrary) })]
        public bool SetMethod(HttpMethod value)
        {
            var b = new HttpRequestBuilder().SetMethod(value) as HttpRequestBuilder;

            return b.RequestInfo.Method == value.ToString();
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public bool SetHeaders(IList<string> values)
        {
            var headers = values.NullToEmpty().Select(v => new HttpHeader(name: "hdr", value: v)).ToList();

            var b = new HttpRequestBuilder().SetHeaders(headers) as HttpRequestBuilder;

            return b.RequestInfo.Headers.SequenceEqual(headers);
        }

        [Property(Verbose = PropertyTests.VerboseTests)]
        public bool SetTimeeout(TimeSpan value)
        {
            var b = new HttpRequestBuilder().SetTimeout(value) as HttpRequestBuilder;

            return b.RequestInfo.Timeout == value;
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public bool SetReadWriteTimeeout(TimeSpan value)
        {
            var b = new HttpRequestBuilder().SetReadWriteTimeout(value) as HttpRequestBuilder;

            return b.RequestInfo.ReadWriteTimeout == value;
        }


        [Property(Verbose = PropertyTests.VerboseTests)]
        public bool SetMaxRedirects(int value)
        {
            var b = new HttpRequestBuilder().SetMaxRedirects(value) as HttpRequestBuilder;

            return b.RequestInfo.MaxRedirects == value;
        }

        [Fact]
        public void SetCredentials()
        {
            var value = Substitute.For<ICredentials>();
            var b = new HttpRequestBuilder().SetCredentials(value) as HttpRequestBuilder;

            b.RequestInfo.Credentials.Should().Be(value);
        }
    }
}
