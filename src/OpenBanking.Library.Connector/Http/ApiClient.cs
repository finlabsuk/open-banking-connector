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
            : this(new ConsoleInstrumentationClient(), httpClient)
        {
        }

        public ApiClient(IInstrumentationClient instrumentation, HttpClient httpClient)
        {
            _instrumentation = instrumentation.ArgNotNull(nameof(instrumentation));
            _httpClient = httpClient.ArgNotNull(nameof(httpClient));
        }


        public async Task<T> RequestJsonAsync<T>(HttpRequestMessage request, bool requestContentIsJson)
            where T : class
        {
            request.ArgNotNull(nameof(request));

            try
            {
                // Make HTTP call
                using var response = await _httpClient.SendAsync(request);
                var json = await GetStringResponseAsync(response);

                // Generate HTTP request info trace
                var requestTraceSb = new StringBuilder()
                    .AppendLine("#### HTTP REQUEST")
                    .AppendLine("######## REQUEST")
                    .Append(request.ToString());
                if (request.Content != null)
                {
                    var body = await request.Content.ReadAsStringAsync();
                    requestTraceSb
                        .AppendLine(",")
                        .AppendLine("Body:");
                    if (requestContentIsJson)
                    {
                        dynamic parsedJson = JsonConvert.DeserializeObject(body);
                        var jsonFormatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                        requestTraceSb.AppendLine(jsonFormatted);
                    }
                    else
                    {
                        requestTraceSb
                            .AppendLine(body);
                    }
                }
                else
                {
                    requestTraceSb.AppendLine();
                }
                requestTraceSb
                    .AppendLine("######## RESPONSE")
                    .AppendLine($"{response.ToString()},");
                if (!string.IsNullOrEmpty(json))
                {
                    dynamic parsedJson = JsonConvert.DeserializeObject(json);
                    var jsonFormatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
                    requestTraceSb.AppendLine("Body:");
                    requestTraceSb.AppendLine(jsonFormatted);
                }
                _instrumentation.Info(requestTraceSb.ToString());

                // Check HTTP status code
                var _ = response.EnsureSuccessStatusCode();

                return json != null ? JsonConvert.DeserializeObject<T>(json) : null;
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
                var response = await _httpClient.SendAsync(request);
                _instrumentation.EndTrace(new HttpTraceInfo("Ended request", request.RequestUri, response.StatusCode));

                return response;
            }
            catch (Exception ex)
            {
                _instrumentation.Exception(ex, request.RequestUri.ToString());

                throw;
            }
        }

        private static async Task<string> GetStringResponseAsync(HttpResponseMessage response)
        {
            string json = null;
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
