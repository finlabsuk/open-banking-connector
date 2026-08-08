// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Http.Headers;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using Newtonsoft.Json;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Http;

public class ApiClientTests
{
    [Theory]
    [InlineData("https://yadayada.com", "just a test")]
    public async Task SendAsync_ResponseReturned(string url, string content)
    {
        var contentType = "text/plain";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(contentType, content);

        var client = mockHttp.ToHttpClient();

        var apiClient = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            client);

        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        HttpResponseMessage response = await apiClient.LowLevelSendAsync(req);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Equal(content, responseContent);
        Assert.Equal(contentType + "; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Theory]
    [InlineData("https://yadayada.com", "just a test")]
    public async Task SendAsync_TraceStarted(string url, string content)
    {
        var contentType = "text/plain";

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(contentType, content);

        var client = mockHttp.ToHttpClient();

        var instrumentationClient = Substitute.For<IInstrumentationClient>();

        var apiClient = new ApiClient(instrumentationClient, client);

        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        HttpResponseMessage response = await apiClient.LowLevelSendAsync(req);
    }

    [Theory]
    [InlineData("https://yadayada.com")]
    public async Task SendAsync_ExceptionLogged(string url)
    {
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(x => throw new HttpRequestException());

        var client = mockHttp.ToHttpClient();

        var instrumentationClient = Substitute.For<IInstrumentationClient>();

        var apiClient = new ApiClient(instrumentationClient, client);

        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        Func<Task> a = async () => await apiClient.LowLevelSendAsync(req);

        await Assert.ThrowsAsync<HttpRequestException>(a);
    }


    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Success(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var entity = new SerialisedEntity { Message = "test message" };
        string content = JsonConvert.SerializeObject(entity);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond("application/json", content);

        using (var http = mockHttp.ToHttpClient())
        {
            var api = new ApiClient(
                Substitute.For<IInstrumentationClient>(),
                http);
            (SerialisedEntity result, ExternalApiResponseHeaders responseHeaders) =
                await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                    req,
                    "",
                    null,
                    null,
                    true);

            Assert.Equal(entity.Message, result.Message);
        }
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Success_NoRateLimitHeaders_ReturnsNull(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var entity = new SerialisedEntity { Message = "test message" };
        string content = JsonConvert.SerializeObject(entity);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond("application/json", content);

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        (SerialisedEntity _, ExternalApiResponseHeaders responseHeaders) =
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        Assert.Null(responseHeaders.XFapiInteractionId);
        Assert.Null(responseHeaders.RateLimitPolicy);
        Assert.Null(responseHeaders.RateLimit);
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Success_CapturesRateLimitHeaders(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var entity = new SerialisedEntity { Message = "test message" };
        string content = JsonConvert.SerializeObject(entity);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(
            _ =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                response.Headers.Add("x-fapi-interaction-id", "2b68f7eb-a4ff-439e-ad74-be529df0cd69");
                response.Headers.Add("RateLimit-Policy", "\"tier1\";q=481;w=5");
                response.Headers.Add("RateLimit", "\"tier1\";r=49;t=36");
                return response;
            });

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        (SerialisedEntity result, ExternalApiResponseHeaders responseHeaders) =
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        Assert.Equal(entity.Message, result.Message);
        Assert.Equal("2b68f7eb-a4ff-439e-ad74-be529df0cd69", responseHeaders.XFapiInteractionId);
        Assert.Equal(["\"tier1\";q=481;w=5"], responseHeaders.RateLimitPolicy);
        Assert.Equal(["\"tier1\";r=49;t=36"], responseHeaders.RateLimit);
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Success_MultipleRateLimitPolicies_CapturesAll(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var entity = new SerialisedEntity { Message = "test message" };
        string content = JsonConvert.SerializeObject(entity);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(
            _ =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(content, Encoding.UTF8, "application/json")
                };
                // An ASPSP may enforce more than one quota policy at once (e.g. burst + sustained),
                // reported as repeated header instances.
                response.Headers.Add("RateLimit-Policy", "\"platinum\";q=400;w=30");
                response.Headers.Add("RateLimit-Policy", "\"overflow\";q=10;w=1");
                return response;
            });

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        (SerialisedEntity _, ExternalApiResponseHeaders responseHeaders) =
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        Assert.Equal(
            ["\"platinum\";q=400;w=30", "\"overflow\";q=10;w=1"],
            responseHeaders.RateLimitPolicy);
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff", 67, false)]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff", 120, true)]
    public async Task ApiClient_RequestJsonAsync_Failure_CapturesRetryAfterHeader(
        string url,
        int expectedSeconds,
        bool useDateFormat)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(
            _ =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
                response.Headers.RetryAfter = useDateFormat
                    ? new RetryConditionHeaderValue(DateTimeOffset.UtcNow.AddSeconds(expectedSeconds))
                    : new RetryConditionHeaderValue(TimeSpan.FromSeconds(expectedSeconds));
                return response;
            });

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        Func<Task> a = async () =>
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        var exception = await Assert.ThrowsAsync<HttpResponseException>(a);
        var failure = Assert.IsType<ExternalApiHttpRequestFailure>(exception.ServerError);
        Assert.NotNull(failure.RetryAfterSeconds);
        Assert.InRange(failure.RetryAfterSeconds.Value, expectedSeconds - 2, expectedSeconds + 2);
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Failure_CapturesRateLimitHeaders(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(
            _ =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent("{}", Encoding.UTF8, "application/json")
                };
                response.Headers.Add("RateLimit-Policy", "\"tier1\";q=481;w=5");
                response.Headers.Add("RateLimit", "\"tier1\";r=0;t=5");
                return response;
            });

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        Func<Task> a = async () =>
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        var exception = await Assert.ThrowsAsync<HttpResponseException>(a);
        var failure = Assert.IsType<ExternalApiHttpRequestFailure>(exception.ServerError);
        Assert.Equal(["\"tier1\";q=481;w=5"], failure.RateLimitPolicy);
        Assert.Equal(["\"tier1\";r=0;t=5"], failure.RateLimit);
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Failure_NoRetryAfterHeader_RetryAfterSecondsIsNull(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(
            _ => new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        Func<Task> a = async () =>
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        var exception = await Assert.ThrowsAsync<HttpResponseException>(a);
        var failure = Assert.IsType<ExternalApiHttpRequestFailure>(exception.ServerError);
        Assert.Null(failure.RetryAfterSeconds);
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Failure_NoContent(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var entity = new SerialisedEntity { Message = "test message" };
        string content = JsonConvert.SerializeObject(entity);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(r => new HttpResponseMessage(HttpStatusCode.OK));

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        Func<Task> a = async () =>
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        await Assert.ThrowsAsync<HttpRequestException>(a);
    }

    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Failure(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var entity = new SerialisedEntity { Message = "test message" };
        string content = JsonConvert.SerializeObject(entity);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(
            r => new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json")
            });

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            http);

        Func<Task> a = async () =>
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        await Assert.ThrowsAsync<HttpResponseException>(a);
    }


    [Theory]
    [InlineData("https://a5b2a8a9-1220-4aa4-aa83-0036a7bd1e69.com/stuff")]
    public async Task ApiClient_RequestJsonAsync_Failure_StartEndTraceLogged(string url)
    {
        HttpRequestMessage req = new HttpRequestBuilder()
            .SetMethod(HttpMethod.Get)
            .SetUri(url)
            .CreateHttpRequestMessage();

        var entity = new SerialisedEntity { Message = "test message" };
        string content = JsonConvert.SerializeObject(entity);
        var instrumentationClient = Substitute.For<IInstrumentationClient>();

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(HttpMethod.Get, url).Respond(
            r => new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    "application/json")
            });

        using var http = mockHttp.ToHttpClient();
        var api = new ApiClient(instrumentationClient, http);

        Func<Task> a = async () =>
            await api.SendExpectingJsonResponseAsync<SerialisedEntity>(
                req,
                "",
                null,
                null,
                true);

        await Assert.ThrowsAsync<HttpResponseException>(a);
        instrumentationClient.Received(1).Trace(Arg.Any<string>());
    }

    public class SerialisedEntity
    {
        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
