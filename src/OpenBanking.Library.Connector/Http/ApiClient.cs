// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Microsoft.Extensions.Http.Logging;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class ApiClient(
    IHttpClient httpClient,
    IInstrumentationClient instrumentationClient,
    TppReportingMetrics? tppReportingMetrics)
    : IApiClient, IDisposable
{
    private readonly IHttpClient _httpClient = httpClient.ArgNotNull(nameof(httpClient));

    private readonly IHttpClientLogger _httpRequestLogger = new HttpRequestLogger(instrumentationClient);

    private readonly IInstrumentationClient _instrumentation =
        instrumentationClient.ArgNotNull(nameof(instrumentationClient));

    /// <summary>
    ///     Constructor used for external (bank) API requests
    /// </summary>
    /// <param name="instrumentationClient"></param>
    /// <param name="pooledConnectionLifetimeSeconds"></param>
    /// <param name="tppReportingMetrics"></param>
    /// <param name="clientCertificates"></param>
    /// <param name="serverCertificateValidator"></param>
    public ApiClient(
        IInstrumentationClient instrumentationClient,
        int pooledConnectionLifetimeSeconds,
        TppReportingMetrics tppReportingMetrics,
        IList<X509Certificate2>? clientCertificates = null,
        IServerCertificateValidator? serverCertificateValidator = null) : this(
        new HttpClientWrapper(
            new HttpClient(
                CreatePrimaryHandler(
                    pooledConnectionLifetimeSeconds,
                    clientCertificates,
                    serverCertificateValidator))),
        instrumentationClient,
        tppReportingMetrics) { }

    public ApiClient(IInstrumentationClient instrumentationClient, HttpClient unwrappedHttpClient) : this(
        new HttpClientWrapper(unwrappedHttpClient),
        instrumentationClient,
        null) { }

    public static JsonSerializerSettings GetDefaultJsonSerializerSettings => new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateParseHandling = DateParseHandling.None
    };

    public async Task<(T response, string? xFapiInteractionId)> SendExpectingJsonResponseAsync<T>(
        HttpRequestMessage request,
        string? requestContentForLog,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(request);

        (int statusCode, string? responseBody, string? xFapiInteractionId) =
            await SendInnerAsync(request, requestContentForLog, tppReportingRequestInfo);

        // Check body not null
        if (responseBody is null)
        {
            throw new HttpRequestException("Received null HTTP body when configured to receive non-null type.");
        }

        T? responseBodyTyped;
        try
        {
            // De-serialise body
            responseBodyTyped = JsonConvert.DeserializeObject<T>(responseBody, jsonSerializerSettings);
        }
        catch (Exception ex)
        {
            throw new ExternalApiResponseDeserialisationException(
                statusCode,
                $"{request.Method}",
                $"{request.RequestUri}",
                responseBody,
                xFapiInteractionId,
                ex.Message);
        }

        if (responseBodyTyped is null)
        {
            throw new HttpRequestException("Could not de-serialise HTTP body");
        }

        return (responseBodyTyped, xFapiInteractionId);
    }

    public async Task SendExpectingNoResponseAsync(
        HttpRequestMessage request,
        string? requestContentForLog,
        TppReportingRequestInfo? tppReportingRequestInfo)
    {
        ArgumentNullException.ThrowIfNull(request);

        (int statusCode, string? responseBody, string? xFapiInteractionId) =
            await SendInnerAsync(request, requestContentForLog, tppReportingRequestInfo);

        // Check body null
        if (!string.IsNullOrEmpty(responseBody))
        {
            throw new HttpRequestException("Received non-null HTTP body when configured to receive null type.");
        }
    }

    public async Task<string> SendExpectingStringResponseAsync(
        HttpRequestMessage request,
        string? requestContentForLog,
        TppReportingRequestInfo? tppReportingRequestInfo)
    {
        ArgumentNullException.ThrowIfNull(request);

        (int statusCode, string? responseBody, string? xFapiInteractionId) =
            await SendInnerAsync(request, requestContentForLog, tppReportingRequestInfo);

        // Check body not null
        if (string.IsNullOrEmpty(responseBody))
        {
            throw new HttpRequestException("Received null HTTP body when configured to receive non-null type.");
        }

        return responseBody;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private static SocketsHttpHandler CreatePrimaryHandler(
        int pooledConnectionLifetimeSeconds,
        IList<X509Certificate2>? clientCertificates,
        IServerCertificateValidator? serverCertificateValidator)
    {
        var clientHandler = new SocketsHttpHandler
        {
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            ActivityHeadersPropagator =
                DistributedContextPropagator
                    .CreateNoOutputPropagator(), // ensures no traceparent HTTP header when ActivityHeadersPropagator used 
            PooledConnectionLifetime = TimeSpan.FromSeconds(pooledConnectionLifetimeSeconds)
        };

        const int maxRedirects = 50;
        clientHandler.AllowAutoRedirect = true;
        clientHandler.MaxAutomaticRedirections = maxRedirects;

        // SSL settings
        var sslClientAuthenticationOptions = new SslClientAuthenticationOptions
        {
            EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
        };

        // Limit cipher suites on Linux
        var tls12CipherSuites = new[]
        {
            TlsCipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
            TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
            TlsCipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384, TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384
        };
        var tls13CipherSuites = new[]
        {
            TlsCipherSuite.TLS_AES_256_GCM_SHA384, TlsCipherSuite.TLS_AES_128_GCM_SHA256,
            TlsCipherSuite.TLS_CHACHA20_POLY1305_SHA256
        };
        TlsCipherSuite[] allCipherSuites = tls13CipherSuites.Concat(tls12CipherSuites).ToArray();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            sslClientAuthenticationOptions.CipherSuitesPolicy = new CipherSuitesPolicy(allCipherSuites);
        }

        // Add client certs
        if (clientCertificates is not null &&
            clientCertificates.Count > 0)
        {
            X509Certificate2[] y = clientCertificates.ToArray();
            sslClientAuthenticationOptions.ClientCertificates = new X509Certificate2Collection(y);
        }

        // Add custom remote cert validation
        if (serverCertificateValidator is not null)
        {
            sslClientAuthenticationOptions.RemoteCertificateValidationCallback = serverCertificateValidator.IsOk;
        }

        clientHandler.SslOptions = sslClientAuthenticationOptions;
        return clientHandler;
    }

    private async Task<(int statusCode, string? responseBody, string? xFapiInteractionId)> SendInnerAsync(
        HttpRequestMessage request,
        string? requestContentForLog,
        TppReportingRequestInfo? tppReportingRequestInfo)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Make request and retrieve response body.
        // Since logging requires request and response body, done at this level rather than in wrapping  DelegatingHandler as previously intended.
        _httpRequestLogger.LogRequestStart(request);
        HttpResponseMessage? response = null;
        string? responseBody = null;
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            // Make request
            response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseContentRead,
                CancellationToken.None);
            stopWatch.Stop();

            // Get request and response bodies
            responseBody = await response.Content.ReadAsStringAsync();

            // Log request
            _httpRequestLogger.LogRequestStop(
                new HttpRequestLoggerAdditionalData
                {
                    RequestBody = requestContentForLog,
                    ResponseBody = responseBody
                },
                request,
                response,
                stopWatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopWatch.Stop(); // no-op if not required

            // Update TPP reporting metric
            if (tppReportingRequestInfo is not null) // removed ex is TaskCanceledException { InnerException: TimeoutException })
            {
                if (response is null)
                {
                    tppReportingMetrics?.RequestNoResponseCount.Add(
                        1,
                        new KeyValuePair<string, object?>("bank_profile", tppReportingRequestInfo.BankProfile),
                        new KeyValuePair<string, object?>(
                            "external_api_endpoint",
                            tppReportingRequestInfo.EndpointDescription));
                }
                else
                {
                    UpdateTppReportingMetrics(tppReportingRequestInfo, (int) response.StatusCode);
                }
            }

            // Log request
            _httpRequestLogger.LogRequestFailed(
                new HttpRequestLoggerAdditionalData
                {
                    RequestBody = requestContentForLog,
                    ResponseBody = responseBody
                },
                request,
                response,
                ex,
                stopWatch.Elapsed);
            throw;
        }
        finally
        {
            response?.Dispose();
        }

        // Update TPP reporting metric
        if (tppReportingRequestInfo is not null)
        {
            UpdateTppReportingMetrics(tppReportingRequestInfo, (int) response.StatusCode);
        }

        // Get selected response headers
        string? xFapiInteractionId = null;
        if (response.Headers.TryGetValues("x-fapi-interaction-id", out IEnumerable<string>? values))
        {
            xFapiInteractionId = values.First();
        }

        // Check HTTP status code
        var statusCode = (int) response.StatusCode;
        if (!response.IsSuccessStatusCode)
        {
            throw new ExternalApiHttpErrorException(
                statusCode,
                $"{request.Method}",
                $"{request.RequestUri}",
                responseBody,
                xFapiInteractionId);
        }

        return (statusCode, responseBody, xFapiInteractionId);
    }

    private void UpdateTppReportingMetrics(
        TppReportingRequestInfo tppReportingRequestInfo,
        int statusCodeInt)
    {
        if (tppReportingMetrics is null)
        {
            return;
        }
        Counter<int>? count = statusCodeInt switch
        {
            (>= 200) and (<= 299) => tppReportingMetrics.Request2xxResponseCount,
            (>= 400) and (<= 499) => tppReportingMetrics.Request4xxResponseCount,
            (>= 500) and (<= 599) => tppReportingMetrics.Request5xxResponseCount,
            _ => null
        };
        count?.Add(
            1,
            new KeyValuePair<string, object?>("bank_profile", tppReportingRequestInfo.BankProfile),
            new KeyValuePair<string, object?>(
                "external_api_endpoint",
                tppReportingRequestInfo.EndpointDescription));
    }

    public async Task<HttpResponseMessage> LowLevelSendAsync(HttpRequestMessage request)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseContentRead,
                CancellationToken.None);

            return response;
        }
        catch (Exception ex)
        {
            _instrumentation.Exception(ex, request.RequestUri!.ToString());

            throw;
        }
    }

    private static async Task<string?> GetStringRequestBodyAsync(HttpRequestMessage request)
    {
        using HttpContent? content = request.Content;
        if (content is null)
        {
            return null;
        }
        return await content.ReadAsStringAsync();
    }
}
