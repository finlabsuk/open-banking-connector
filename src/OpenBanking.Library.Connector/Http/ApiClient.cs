// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using Microsoft.Extensions.Http.Logging;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpClientLogger _httpRequestLogger;
    private readonly IInstrumentationClient _instrumentation;

    public ApiClient(
        IInstrumentationClient instrumentationClient,
        int pooledConnectionLifetimeSeconds,
        IList<X509Certificate2>? clientCertificates = null,
        IServerCertificateValidator? serverCertificateValidator = null)
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
            EnabledSslProtocols = SslProtocols.Tls12
        };

        // Limit cipher suites on Linux
        var cipherSuites = new[]
        {
            TlsCipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
            TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
            TlsCipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384, TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384
        };
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            sslClientAuthenticationOptions.CipherSuitesPolicy = new CipherSuitesPolicy(cipherSuites);
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

        _instrumentation = instrumentationClient.ArgNotNull(nameof(instrumentationClient));
        _httpRequestLogger = new HttpRequestLogger(instrumentationClient);
        _httpClient = new HttpClient(clientHandler);
    }

    public ApiClient(IInstrumentationClient instrumentationClient, HttpClient httpClient)
    {
        _instrumentation = instrumentationClient.ArgNotNull(nameof(instrumentationClient));
        _httpRequestLogger = new HttpRequestLogger(instrumentationClient);
        _httpClient = httpClient.ArgNotNull(nameof(httpClient));
    }

    public static JsonSerializerSettings GetDefaultJsonSerializerSettings => new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        DateParseHandling = DateParseHandling.None
    };

    public async Task<T> SendExpectingJsonResponseAsync<T>(
        HttpRequestMessage request,
        JsonSerializerSettings? jsonSerializerSettings)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(request);

        (int statusCode, string? responseBody, string? xFapiInteractionId) = await SendInnerAsync(request);

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

        return responseBodyTyped;
    }

    public async Task SendExpectingNoResponseAsync(HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request);

        (int statusCode, string? responseBody, string? xFapiInteractionId) = await SendInnerAsync(request);

        // Check body null
        if (!string.IsNullOrEmpty(responseBody))
        {
            throw new HttpRequestException("Received non-null HTTP body when configured to receive null type.");
        }
    }

    public async Task<string> SendExpectingStringResponseAsync(HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request);

        (int statusCode, string? responseBody, string? xFapiInteractionId) = await SendInnerAsync(request);

        // Check body not null
        if (string.IsNullOrEmpty(responseBody))
        {
            throw new HttpRequestException("Received null HTTP body when configured to receive non-null type.");
        }

        return responseBody;
    }

    private async Task<(int statusCode, string? responseBody, string? xFapiInteractionId)> SendInnerAsync(
        HttpRequestMessage request)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Make request and retrieve response body.
        // Since logging requires request and response body, done at this level rather than in wrapping  DelegatingHandler as previously intended.
        _httpRequestLogger.LogRequestStart(request);
        HttpResponseMessage? response = null;
        string? requestBody = null;
        string? responseBody = null;
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        try
        {
            // Make request
            response = await _httpClient.SendAsync(request);
            stopWatch.Stop();

            // Get request and response bodies
            requestBody = await GetStringRequestBodyAsync(request); // don't read before making request
            responseBody = await response.Content.ReadAsStringAsync();

            // Log request
            _httpRequestLogger.LogRequestStop(
                new HttpRequestLoggerAdditionalData
                {
                    RequestBody = requestBody,
                    ResponseBody = responseBody
                },
                request,
                response,
                stopWatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopWatch.Stop(); // no-op if not required

            // Log request
            _httpRequestLogger.LogRequestFailed(
                new HttpRequestLoggerAdditionalData
                {
                    RequestBody = requestBody,
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

    public async Task<HttpResponseMessage> LowLevelSendAsync(HttpRequestMessage request)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.SendAsync(request);

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
