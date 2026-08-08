// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.BankProfiles;
using FinnovationLabs.OpenBanking.Library.Connector.Fluent;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using NSubstitute;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Types;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Http;

// Exercises ApiClient's real constructor (the one production code uses via ObWacCertificate /
// ServiceCollectionExtensions), not the plain HttpClient bypass - each test targets something
// CreatePrimaryHandler configures that a mocked HttpMessageHandler (UnitTests) can't exercise.
//
// Most tests use DefaultServerCertificateValidator purely to get past the fixture's self-signed
// cert - that's scaffolding, not how a real bank connection validates certs. See
// ApiClient_HttpsRequest_UntrustedCertificate_NoValidatorOverride_Fails for that real path.
[Trait(TraitTypes.TestTarget, TestTypes.LocalMocks)]
public class ApiClientNonMtlsTests : IClassFixture<NonMtlsWireMockFixture>
{
    private readonly WireMockServer _server;

    public ApiClientNonMtlsTests(NonMtlsWireMockFixture fixture)
    {
        _server = fixture.Server;
        _server.Reset();
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_NoClientCertificate_Succeeds()
    {
        _server
            .Given(Request.Create().WithPath("/.well-known/openid-configuration").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("{}"));

        using ApiClient api = CreateApiClient();

        HttpResponseMessage response =
            await api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/.well-known/openid-configuration"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_ConnectionFailure()
    {
        // Real connection failure (nothing listening on port 1), distinct from an HTTP-level
        // error response.
        using ApiClient api = CreateApiClient();

        await Assert.ThrowsAsync<HttpRequestException>(
            () => api.LowLevelSendAsync(CreateGetRequest("https://127.0.0.1:1/")));
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_TimesOut()
    {
        // SendInnerAsync only maps a timeout to ExternalApiHttpRequestTimeout via
        // SendExpecting*Async, not LowLevelSendAsync. Records via RequestNoResponseCount, not the
        // status-code counters used by ApiClient_SendExpectingStringResponse_RecordsTppReportingMetric.
        _server
            .Given(Request.Create().WithPath("/slow").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithDelay(TimeSpan.FromSeconds(3)));

        (string endpointDescription, TppReportingRequestInfo requestInfo) =
            CreateTppReportingRequestInfo(nameof(ApiClient_HttpsRequest_TimesOut));
        using var capture = new MetricsCapture(endpointDescription);
        using ApiClient api = CreateApiClient(1);

        var exception = await Assert.ThrowsAsync<HttpResponseException>(
            () => api.SendExpectingStringResponseAsync(CreateGetRequest(_server.Url + "/slow"), null, requestInfo));

        Assert.Equal(ServerErrorType.ExternalApiHttpRequestTimeout, exception.ServerError.ServerErrorType);
        Assert.Equal(1, capture.TotalFor("http.client.request.no_response_count"));
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_TruncatedResponse_MapsToIoError()
    {
        // HttpIOException (a real protocol-level failure) maps to ExternalApiHttpRequestIoError.
        // WireMock's fault injection couldn't reproduce a genuine truncated response, so a raw
        // TcpListener sends a Content-Length longer than what it actually writes, then closes.
        await AssertTruncatedResponseMapsToIoErrorAsync(
            nameof(ApiClient_HttpsRequest_TruncatedResponse_MapsToIoError),
            "HTTP/1.1 200 OK\r\nContent-Length: 100\r\n\r\nshort"u8.ToArray());
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_FollowsRedirect()
    {
        // AllowAutoRedirect only takes effect over a real transport - MockHttp can't exercise it.
        _server
            .Given(Request.Create().WithPath("/start").UsingGet())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(302)
                    .WithHeader("Location", _server.Url + "/final"));
        _server
            .Given(Request.Create().WithPath("/final").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("redirected-ok"));

        using ApiClient api = CreateApiClient();

        HttpResponseMessage response = await api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/start"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(
            "redirected-ok",
            await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_DecompressesGzipResponse()
    {
        // AutomaticDecompression only takes effect over a real transport - MockHttp can't
        // exercise it, and a regression here would look like corrupt JSON, not "decompression
        // broke".
        const string body = "just a test";
        _server
            .Given(Request.Create().WithPath("/compressed").UsingGet())
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Encoding", "gzip")
                    .WithBody(GzipCompress(body)));

        using ApiClient api = CreateApiClient();

        HttpResponseMessage response = await api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/compressed"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(body, await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_DoesNotLeakTraceparentHeader()
    {
        // ActivityHeadersPropagator only has effect through the real diagnostics pipeline. A
        // control request (same ambient Activity, no override) proves the header would otherwise
        // be sent, so the assertion isn't vacuous.
        _server
            .Given(Request.Create().WithPath("/traced").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200));
        _server
            .Given(Request.Create().WithPath("/traced-control").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200));

        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref _) => ActivitySamplingResult.AllData
        };
        ActivitySource.AddActivityListener(listener);
        using var activitySource = new ActivitySource(nameof(ApiClient_HttpsRequest_DoesNotLeakTraceparentHeader));
        using Activity? activity = activitySource.StartActivity("test-activity");
        Assert.NotNull(activity);

        using ApiClient api = CreateApiClient();
        await api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/traced"));

        // No ActivityHeadersPropagator override, so this uses .NET's default header-injecting one.
        var controlHandler = new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true
            }
        };
        using var controlHttpClient = new HttpClient(controlHandler);
        await controlHttpClient.GetAsync(_server.Url + "/traced-control", TestContext.Current.CancellationToken);

        IDictionary<string, WireMockList<string>> apiClientRequestHeaders =
            Assert.Single(_server.LogEntries, e => e.RequestMessage!.Path == "/traced").RequestMessage!.Headers!;
        IDictionary<string, WireMockList<string>> controlRequestHeaders =
            Assert.Single(_server.LogEntries, e => e.RequestMessage!.Path == "/traced-control")
                .RequestMessage!.Headers!;

        Assert.DoesNotContain(
            apiClientRequestHeaders.Keys,
            k => k.Equals("traceparent", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            controlRequestHeaders.Keys,
            k => k.Equals("traceparent", StringComparison.OrdinalIgnoreCase));
    }

    [Theory]
    [InlineData(200, "http.client.request.status2xx_responses")]
    [InlineData(404, "http.client.request.status4xx_responses")]
    [InlineData(500, "http.client.request.status5xx_responses")]
    public async Task ApiClient_SendExpectingStringResponse_RecordsTppReportingMetric(
        int statusCode,
        string expectedInstrumentName)
    {
        // Metrics recording only happens via SendExpecting*Async with a real TppReportingMetrics -
        // unreachable from UnitTests, since that constructor always builds a real handler.
        _server
            .Given(Request.Create().WithPath("/metrics").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(statusCode).WithBody("body"));

        (string endpointDescription, TppReportingRequestInfo requestInfo) =
            CreateTppReportingRequestInfo(
                $"{nameof(ApiClient_SendExpectingStringResponse_RecordsTppReportingMetric)}-{statusCode}");
        using var capture = new MetricsCapture(endpointDescription);
        using ApiClient api = CreateApiClient();

        HttpRequestMessage req = CreateGetRequest(_server.Url + "/metrics");
        if (statusCode is >= 200 and <= 299)
        {
            await api.SendExpectingStringResponseAsync(req, null, requestInfo);
        }
        else
        {
            await Assert.ThrowsAsync<HttpResponseException>(
                () => api.SendExpectingStringResponseAsync(req, null, requestInfo));
        }

        Assert.Equal(1, capture.TotalFor(expectedInstrumentName));
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_UntrustedCertificate_NoValidatorOverride_Fails()
    {
        // The only test with no serverCertificateValidator override - the real default-bank
        // connection path. Confirms default cert validation still runs and rejects our
        // self-signed cert; a regression here wouldn't be caught by end-to-end bank tests either,
        // since real banks present CA-trusted certs regardless.
        _server
            .Given(Request.Create().WithPath("/stuff").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200));

        using var api = new ApiClient(
            Substitute.For<IInstrumentationClient>(),
            60,
            ApiClientTestHelpers.CreateTppReportingMetrics());

        await Assert.ThrowsAnyAsync<HttpRequestException>(
            () => api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/stuff")));
    }

    [Fact]
    public async Task ApiClient_CustomTimeout_EnforcedCorrectly()
    {
        // Demonstrates that custom timeoutSeconds parameter configures per-request timeout.
        _server
            .Given(Request.Create().WithPath("/custom-timeout").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200).WithBody("ok").WithDelay(TimeSpan.FromSeconds(2)));

        (string endpointDescription, TppReportingRequestInfo requestInfo) =
            CreateTppReportingRequestInfo(nameof(ApiClient_CustomTimeout_EnforcedCorrectly));

        // With 1s timeout, a 2s response delay causes a timeout.
        using (ApiClient apiShort = CreateApiClient(1))
        {
            var exception = await Assert.ThrowsAsync<HttpResponseException>(
                () => apiShort.SendExpectingStringResponseAsync(
                    CreateGetRequest(_server.Url + "/custom-timeout"),
                    null,
                    requestInfo));
            Assert.Equal(ServerErrorType.ExternalApiHttpRequestTimeout, exception.ServerError.ServerErrorType);
        }

        // With 5s timeout, a 2s response delay completes successfully.
        using (ApiClient apiLong = CreateApiClient(5))
        {
            string result = await apiLong.SendExpectingStringResponseAsync(
                CreateGetRequest(_server.Url + "/custom-timeout"),
                null,
                requestInfo);
            Assert.NotNull(result);
        }
    }

    [Fact]
    public async Task ApiClient_HttpsRequest_TruncatedChunkedResponse_MapsToIoError()
    {
        // Raw TcpListener sends a chunked response header followed by an incomplete chunk data
        // stream (chunk header declares 100 bytes / 0x64, but only 10 are sent), then closes the
        // socket abruptly.
        await AssertTruncatedResponseMapsToIoErrorAsync(
            nameof(ApiClient_HttpsRequest_TruncatedChunkedResponse_MapsToIoError),
            "HTTP/1.1 200 OK\r\nTransfer-Encoding: chunked\r\n\r\n64\r\nincomplete"u8.ToArray());
    }

    // Shared by the two truncated-response tests above: starts a raw TcpListener, drains the
    // request, writes the given (deliberately truncated) response bytes, then asserts the client
    // maps the resulting connection failure to ExternalApiHttpRequestIoError.
    private async Task AssertTruncatedResponseMapsToIoErrorAsync(string testName, byte[] responseBytes)
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint) listener.LocalEndpoint).Port;

        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        Task serverTask = Task.Run(
            async () =>
            {
                using TcpClient client = await listener.AcceptTcpClientAsync(cancellationToken);
                await using NetworkStream stream = client.GetStream();
                // Drain the request first: closing with unread bytes still buffered triggers a
                // hard RST instead of a graceful FIN, which throws HttpRequestException instead of
                // the HttpIOException this test targets. A short read is fine, so the count is
                // discarded.
                var requestBuffer = new byte[4096];
                _ = await stream.ReadAsync(requestBuffer, cancellationToken);
                await stream.WriteAsync(responseBytes, cancellationToken);
            },
            cancellationToken);

        (string endpointDescription, TppReportingRequestInfo requestInfo) = CreateTppReportingRequestInfo(testName);
        using var capture = new MetricsCapture(endpointDescription);
        using ApiClient api = CreateApiClient();

        var exception = await Assert.ThrowsAsync<HttpResponseException>(
            () => api.SendExpectingStringResponseAsync(
                CreateGetRequest($"http://127.0.0.1:{port}/"),
                null,
                requestInfo));

        Assert.Equal(ServerErrorType.ExternalApiHttpRequestIoError, exception.ServerError.ServerErrorType);
        Assert.Equal(1, capture.TotalFor("http.client.request.no_response_count"));

        await serverTask;
    }

    [Fact]
    public async Task ApiClient_Disposal_DisposesHttpClientAndPreventsFurtherRequests()
    {
        ApiClient api = CreateApiClient();
        api.Dispose();

        // Disposing multiple times should not throw
        api.Dispose();

        // Attempts to send requests after disposal should throw ObjectDisposedException
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/stuff")));
    }

    private static ApiClient CreateApiClient(int timeoutSeconds = 100) =>
        new(
            Substitute.For<IInstrumentationClient>(),
            60,
            ApiClientTestHelpers.CreateTppReportingMetrics(),
            null,
            new DefaultServerCertificateValidator(),
            timeoutSeconds);

    private static HttpRequestMessage CreateGetRequest(string uri) =>
        new HttpRequestBuilder().SetMethod(HttpMethod.Get).SetUri(uri).CreateHttpRequestMessage();

    // Shared description keeps MetricsCapture's filter and the request info's tag from drifting apart.
    private static (string EndpointDescription, TppReportingRequestInfo RequestInfo) CreateTppReportingRequestInfo(
        string testName)
    {
        var endpointDescription = $"{testName}-{Guid.NewGuid()}";
        return (
            endpointDescription,
            new TppReportingRequestInfo
            {
                EndpointDescription = endpointDescription,
                BankProfile = BankProfileEnum.NatWest_NatWestSandbox // arbitrary; only used as a metric tag
            });
    }

    private static byte[] GzipCompress(string content)
    {
        using var output = new MemoryStream();
        using (var gzip = new GZipStream(output, CompressionMode.Compress, true))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            gzip.Write(bytes, 0, bytes.Length);
        }
        return output.ToArray();
    }
}
