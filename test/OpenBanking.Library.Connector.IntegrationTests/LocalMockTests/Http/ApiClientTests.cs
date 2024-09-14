// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.LocalMockTests.Http;

public class ApiClientTests
{
    [Theory]
    [InlineData("https://www.google.com/")]
    public async Task ApiClient_SendAsync_Success(string url)
    {
        HttpRequestBuilder b = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url);

        HttpRequestMessage req = b.Create();

        using (var http = new HttpClient())
        {
            var api = new ApiClient(
                Substitute.For<IInstrumentationClient>(),
                http);

            HttpResponseMessage r = await api.LowLevelSendAsync(req);

            r.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    [Theory]
    [InlineData("https://github.com/a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69")]
    public async Task ApiClient_SendAsync_NotFound(string url)
    {
        HttpRequestBuilder b = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url);

        HttpRequestMessage req = b.Create();

        using (var http = new HttpClient())
        {
            var api = new ApiClient(
                Substitute.For<IInstrumentationClient>(),
                http);

            HttpResponseMessage r = await api.LowLevelSendAsync(req);

            r.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public void ApiClient_SendAsync_Failure(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .Create();

        using var http = new HttpClient();
        Func<Task> a = async () =>
        {
            var api = new ApiClient(
                Substitute.For<IInstrumentationClient>(),
                http);
            HttpResponseMessage r = await api.LowLevelSendAsync(req);
        };

        a.Should().ThrowAsync<HttpRequestException>();
    }
}
