// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class HttpRequestBuilder
{
    public HttpRequestBuilder()
    {
        HttpRequestMessage = new HttpRequestMessage();
        HttpRequestMessage.Headers.UserAgent.ParseAdd("OpenBankingConnector");
    }

    protected HttpRequestMessage HttpRequestMessage { get; }

    public HttpRequestBuilder SetUri(Uri value)
    {
        HttpRequestMessage.RequestUri = value.ArgNotNull(nameof(value));
        return this;
    }

    public HttpRequestBuilder SetUri(string value) => SetUri(new Uri(value));

    public HttpRequestBuilder SetMethod(HttpMethod method)
    {
        HttpRequestMessage.Method = method.ArgNotNull(nameof(method));
        return this;
    }

    public HttpRequestBuilder SetHeaders(IEnumerable<HttpHeader> values)
    {
        IList<HttpHeader> infoHeaders = values.ArgNotNull(nameof(values)).ToList();
        if (infoHeaders.Count > 0)
        {
            IEnumerable<IGrouping<string, HttpHeader>> headers = infoHeaders.GroupBy(h => h.Name);
            foreach (IGrouping<string, HttpHeader> headerGroup in headers)
            {
                IEnumerable<string> values1 = headerGroup.Select(h => h.Value);
                HttpRequestMessage.Headers.Add(headerGroup.Key, values1);
            }
        }
        return this;
    }

    public HttpRequestBuilder SetJsonContent<TRequest>(
        TRequest request,
        JsonSerializerSettings? requestJsonSerializerSettings)
    {
        JsonSerializerSettings jsonSerializerSettings =
            requestJsonSerializerSettings ?? new JsonSerializerSettings();
        jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        string content = JsonConvert.SerializeObject(
            request,
            jsonSerializerSettings);
        HttpRequestMessage.Content = new StringContent(
            content,
            Encoding.UTF8,
            new MediaTypeWithQualityHeaderValue("application/json"));
        return this;
    }

    public HttpRequestBuilder SetTextContent(string content, string contentType)
    {
        if (!string.IsNullOrWhiteSpace(content))
        {
            HttpRequestMessage.Content = new StringContent(
                content,
                Encoding.UTF8,
                new MediaTypeWithQualityHeaderValue(contentType));
        }
        return this;
    }

    public HttpRequestMessage Create()
    {
        if (!HttpRequestMessage.Headers.Accept.Any())
        {
            HttpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        return HttpRequestMessage;
    }
}
