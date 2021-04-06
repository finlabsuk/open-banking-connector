// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IInstrumentationClient _instrumentation;

        public ApiClient(HttpClient httpClient)
            : this(new ConsoleInstrumentationClient(), httpClient) { }

        public ApiClient(IInstrumentationClient instrumentation, HttpClient httpClient)
        {
            _instrumentation = instrumentation.ArgNotNull(nameof(instrumentation));
            _httpClient = httpClient.ArgNotNull(nameof(httpClient));
        }


        public async Task<T> RequestJsonAsync<T>(
            HttpRequestMessage request,
            bool requestContentIsJson,
            JsonSerializerSettings? jsonSerializerSettings,
            bool typeTIsNullable = false)
            where T : class?
        {
            request.ArgNotNull(nameof(request));

            try
            {
                // Make HTTP call
                using HttpResponseMessage response = await _httpClient.SendAsync(request);
                string? json = await GetStringResponseAsync(response);

                // Generate HTTP request info trace
                StringBuilder requestTraceSb = new StringBuilder()
                    .AppendLine("#### HTTP REQUEST")
                    .AppendLine("######## REQUEST")
                    .AppendLine($"{request}")
                    .AppendLine("######## REQUEST BODY");
                if (request.Content is null)
                {
                    requestTraceSb.AppendLine("<No Body>");
                }
                else
                {
                    string body = await request.Content.ReadAsStringAsync();
                    if (requestContentIsJson)
                    {
                        dynamic? parsedJson = JsonConvert.DeserializeObject(body);
                        dynamic jsonFormatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                        requestTraceSb.AppendLine(jsonFormatted);
                    }
                    else
                    {
                        requestTraceSb
                            .AppendLine(body);
                    }
                }

                requestTraceSb
                    .AppendLine("######## RESPONSE")
                    .AppendLine($"{response}")
                    .AppendLine("######## RESPONSE BODY");
                if (json is null)
                {
                    requestTraceSb.AppendLine("<No Body>");
                }
                else
                {
                    string jsonFormatted;
                    try
                    {
                        dynamic? parsedJson = JsonConvert.DeserializeObject(json);
                        jsonFormatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                    }
                    catch
                    {
                        jsonFormatted = json;
                    }

                    requestTraceSb.AppendLine(jsonFormatted);
                }

                requestTraceSb.AppendLine("####");
                _instrumentation.Info(requestTraceSb.ToString());

                // Check HTTP status code
                HttpResponseMessage _ = response.EnsureSuccessStatusCode();

                // Check body not null
                T output;
                if (json is null)
                {
                    if (!typeTIsNullable)
                    {
                        throw new HttpRequestException(
                            "Received null HTTP body when configured to receive non-null type.");
                    }

                    output = null;
                }
                else
                {
                    // De-serialise body
                    output = JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings) ??
                             throw new HttpRequestException("Could not de-serialise HTTP body");
                }

                return output!; // Can only be null when typeTIsNullable above is true
            }
            catch (Exception ex)
            {
                _instrumentation.Exception(ex, request.RequestUri.ToString());
                throw;
            }
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            try
            {
                _instrumentation.StartTrace(new HttpTraceInfo("Starting request", request.RequestUri));
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                _instrumentation.EndTrace(
                    new HttpTraceInfo(
                        "Ended request",
                        request.RequestUri,
                        response.StatusCode));

                return response;
            }
            catch (Exception ex)
            {
                _instrumentation.Exception(ex, request.RequestUri.ToString());

                throw;
            }
        }

        private static async Task<string?> GetStringResponseAsync(HttpResponseMessage response)
        {
            string? json = null;
            if (response.Content != null)
            {
                using (response.Content)
                {
                    json = await response.Content.ReadAsStringAsync();
                }
            }

            return json;
        }
    }
}
