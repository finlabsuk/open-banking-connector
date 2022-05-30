// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IInstrumentationClient _instrumentation;

        public ApiClient(
            IInstrumentationClient instrumentationClient,
            IList<X509Certificate2>? clientCertificates = null,
            IServerCertificateValidator? serverCertificateValidator = null)
        {
            var clientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
            };

            const int maxRedirects = 50;
            clientHandler.AllowAutoRedirect = true;
            clientHandler.MaxAutomaticRedirections = maxRedirects;

            if (clientCertificates is not null &&
                clientCertificates.Count > 0)
            {
                clientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                clientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;

                foreach (X509Certificate2 certificate in clientCertificates)
                {
                    clientHandler.ClientCertificates.Add(certificate);
                }
            }

            if (serverCertificateValidator is not null)
            {
                clientHandler.ServerCertificateCustomValidationCallback = serverCertificateValidator.IsOk;
            }

            _instrumentation = instrumentationClient.ArgNotNull(nameof(instrumentationClient));
            _httpClient = new HttpClient(new ErrorAndLoggingHandler(clientHandler));
        }

        public ApiClient(IInstrumentationClient instrumentation, HttpClient httpClient)
        {
            _instrumentation = instrumentation.ArgNotNull(nameof(instrumentation));
            _httpClient = httpClient.ArgNotNull(nameof(httpClient));
        }


        public async Task<T> RequestJsonAsync<T>(
            HttpRequestMessage request,
            JsonSerializerSettings? jsonSerializerSettings)
            where T : class
        {
            request.ArgNotNull(nameof(request));

            HttpResponseMessage? response = null;
            string? responseBody = null;
            try
            {
                // Make HTTP call
                response = await _httpClient.SendAsync(request);
                responseBody = await GetStringResponseAsync(response);

                // Check HTTP status code
                HttpResponseMessage _ = response.EnsureSuccessStatusCode();

                // Check body not null
                if (responseBody is null)
                {
                    throw new HttpRequestException("Received null HTTP body when configured to receive non-null type.");
                }

                // De-serialise body
                T responseBodyTyped =
                    JsonConvert.DeserializeObject<T>(responseBody, jsonSerializerSettings) ??
                    throw new HttpRequestException("Could not de-serialise HTTP body");

                return responseBodyTyped;
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
        }

        public async Task SendAsync(HttpRequestMessage request)
        {
            request.ArgNotNull(nameof(request));

            HttpResponseMessage? response = null;
            string? responseBody = null;
            try
            {
                // Make HTTP call
                response = await _httpClient.SendAsync(request);
                responseBody = await GetStringResponseAsync(response);

                // Check HTTP status code
                HttpResponseMessage _ = response.EnsureSuccessStatusCode();

                // Check body not null
                if (!string.IsNullOrEmpty(responseBody))
                {
                    throw new HttpRequestException("Received non-null HTTP body when configured to receive null type.");
                }
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
        }

        public async Task<HttpResponseMessage> LowLevelSendAsync(HttpRequestMessage request)
        {
            try
            {
                _instrumentation.StartTrace(new HttpTraceInfo("Starting request", request.RequestUri!));
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                _instrumentation.EndTrace(
                    new HttpTraceInfo(
                        "Ended request",
                        request.RequestUri!,
                        response.StatusCode));

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
            if (response.Content is null)
            {
                return null;
            }

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
}
