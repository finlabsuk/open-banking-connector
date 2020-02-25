// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
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


        public async Task<T> RequestJsonAsync<T>(HttpRequestMessage request)
            where T : class
        {
            request.ArgNotNull(nameof(request));

            try
            {
                _instrumentation.StartTrace(new HttpTraceInfo("Starting request", request.RequestUri));

                using (var response = await _httpClient.SendAsync(request))
                {
                    var json = await GetStringResponseAsync(response);

                    var traceInfo = new HttpTraceInfo("Ended request", request.RequestUri, response.StatusCode);
                    if (!string.IsNullOrEmpty(json))
                    {
                        traceInfo.Add("Response", json);
                    }

                    _instrumentation.EndTrace(traceInfo);
                    var _ = response.EnsureSuccessStatusCode();


                    return json != null ? JsonConvert.DeserializeObject<T>(json) : null;
                }
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
