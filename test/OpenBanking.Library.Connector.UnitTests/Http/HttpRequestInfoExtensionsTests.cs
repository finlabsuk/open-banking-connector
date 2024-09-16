// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Http;

public class HttpRequestInfoExtensionsTests
{
    [Theory]
    [InlineData("text/html")]
    [InlineData("application/pdf")]
    public void Create_ContentTypesPreserved(string contentType)
    {
        HttpRequestMessage result = new HttpRequestBuilder()
            .SetUri(new Uri("http://tests"))
            .SetTextContent("abcdef", contentType)
            .CreateHttpRequestMessage();

        MediaTypeHeaderValue? resultContentTypes =
            result.Content?.Headers.ContentType;

        resultContentTypes?.MediaType.Should().Be(contentType);
    }
}
