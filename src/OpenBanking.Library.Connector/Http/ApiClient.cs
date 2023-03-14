// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Utility;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IInstrumentationClient _instrumentation;

    public ApiClient(
        IInstrumentationClient instrumentationClient,
        IList<X509Certificate2>? clientCertificates = null,
        IServerCertificateValidator? serverCertificateValidator = null)
    {
        var clientHandler = new SocketsHttpHandler
        {
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
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
            TlsCipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,
            TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384
        };
        if (OsPlatformEnumHelper.GetCurrentOsPlatform() is OsPlatformEnum.Linux)
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
        _httpClient = new HttpClient(new LoggingHandler(clientHandler));
    }

    public ApiClient(IInstrumentationClient instrumentation, HttpClient httpClient)
    {
        _instrumentation = instrumentation.ArgNotNull(nameof(instrumentation));
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
        request.ArgNotNull(nameof(request));

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
        request.ArgNotNull(nameof(request));

        (int statusCode, string? responseBody, string? xFapiInteractionId) = await SendInnerAsync(request);

        // Check body not null
        if (!string.IsNullOrEmpty(responseBody))
        {
            throw new HttpRequestException("Received non-null HTTP body when configured to receive null type.");
        }
    }

    private async Task<(int statusCode, string? responseBody, string? xFapiInteractionId)> SendInnerAsync(
        HttpRequestMessage request)
    {
        HttpResponseMessage? response = null;

        int statusCode;
        string? responseBody = null;
        string? xFapiInteractionId = null;
        try
        {
            // Make HTTP call
            response = await _httpClient.SendAsync(request);
            responseBody = await GetStringResponseAsync(response);

            // Get selected headers
            if (response.Headers.TryGetValues("x-fapi-interaction-id", out IEnumerable<string>? values))
            {
                xFapiInteractionId = values.First();
            }

            // Check HTTP status code
            statusCode = (int) response.StatusCode;
            if (statusCode >= 400)
            {
                throw new ExternalApiHttpErrorException(
                    statusCode,
                    $"{request.Method}",
                    $"{request.RequestUri}",
                    responseBody ?? "",
                    xFapiInteractionId);
            }

            HttpResponseMessage _ = response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _instrumentation.Exception(ex, request.RequestUri!.ToString());
            throw;
        }
        finally
        {
            await LogRequest(request, response, responseBody);
            response?.Dispose();
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

    private async Task LogRequest(
        HttpRequestMessage request,
        HttpResponseMessage? response,
        string? responseBody)
    {
        string jsonFormatted;

        // Generate HTTP request info trace
        StringBuilder requestTraceSb = new StringBuilder()
            .AppendLine("#### HTTP REQUEST")
            .AppendLine("######## REQUEST")
            .AppendLine($"{request}")
            .AppendLine("######## REQUEST BODY");

        string? requestBody = await GetStringRequestAsync(request);
        if (string.IsNullOrEmpty(requestBody))
        {
            requestTraceSb.AppendLine("<No Body>");
        }
        else
        {
            try
            {
                dynamic parsedJson =
                    JsonConvert.DeserializeObject(requestBody) ??
                    throw new NullReferenceException();
                jsonFormatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
            }
            catch
            {
                jsonFormatted = requestBody;
            }

            requestTraceSb.AppendLine(jsonFormatted);
        }

        requestTraceSb.AppendLine("######## RESPONSE");
        if (response is null)
        {
            requestTraceSb.AppendLine("<No Response>");
        }
        else
        {
            requestTraceSb
                .AppendLine($"{response}")
                .AppendLine("######## RESPONSE BODY");
            if (string.IsNullOrEmpty(responseBody))
            {
                requestTraceSb.AppendLine("<No Body>");
            }
            else
            {
                try
                {
                    dynamic parsedJson =
                        JsonConvert.DeserializeObject(responseBody) ??
                        throw new NullReferenceException();
                    jsonFormatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                }
                catch
                {
                    jsonFormatted = responseBody;
                }

                requestTraceSb.AppendLine(jsonFormatted);
            }
        }

        requestTraceSb.AppendLine("####");
        _instrumentation.Trace(requestTraceSb.ToString());
    }

    private static async Task<string?> GetStringResponseAsync(HttpResponseMessage response)
    {
        using (response.Content)
        {
            return await response.Content.ReadAsStringAsync();
        }
    }

    private static async Task<string?> GetStringRequestAsync(HttpRequestMessage request)
    {
        if (request.Content is null)
        {
            return null;
        }

        using (request.Content)
        {
            return await request.Content.ReadAsStringAsync();
        }
    }
}
