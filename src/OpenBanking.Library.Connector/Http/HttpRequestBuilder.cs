// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using FinnovationLabs.OpenBanking.Library.Connector.Metrics;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class HttpRequestBuilder
{
    protected HttpRequestInfo RequestInfo { get; } = new();

    public HttpRequestBuilder SetUri(Uri value)
    {
        RequestInfo.RequestUri = value.ArgNotNull(nameof(value));
        return this;
    }

    public HttpRequestBuilder SetUri(string value) => SetUri(new Uri(value));

    public HttpRequestBuilder SetMethod(HttpMethod method)
    {
        RequestInfo.Method = method.ArgNotNull(nameof(method));
        return this;
    }

    public HttpRequestBuilder SetHeaders(IEnumerable<HttpHeader> values)
    {
        RequestInfo.Headers = values.ArgNotNull(nameof(values)).ToList();
        return this;
    }

    public HttpRequestBuilder SetJsonContent<TRequest>(
        TRequest request,
        JsonSerializerSettings? requestJsonSerializerSettings)
    {
        RequestInfo.GetContent = () =>
        {
            JsonSerializerSettings jsonSerializerSettings =
                requestJsonSerializerSettings ?? new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            string content = JsonConvert.SerializeObject(
                request,
                jsonSerializerSettings);
            var httpContent = new StringContent(
                content,
                Encoding.UTF8,
                new MediaTypeWithQualityHeaderValue("application/json"));
            using JsonDocument document = JsonDocument.Parse(content);
            string contentForLog = JsonSerializer.Serialize(
                document,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            return (httpContent, contentForLog);
        };
        return this;
    }

    public HttpRequestBuilder SetTextContent(string content, string contentType)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("Text content not specified.");
        }
        RequestInfo.GetContent = () => (new StringContent(
            content,
            Encoding.UTF8,
            new MediaTypeWithQualityHeaderValue(contentType)), content);
        return this;
    }

    public async Task<(T response, string? xFapiInteractionId)> SendExpectingJsonResponseAsync<T>(
        IApiClient client,
        TppReportingRequestInfo? tppReportingRequestInfo,
        JsonSerializerSettings? jsonSerializerSettings = null)
        where T : class
    {
        client.ArgNotNull(nameof(client));

        (HttpRequestMessage Message, string? RequestContentForLog) requestData = RequestInfo.CreateRequestMessage();
        using HttpRequestMessage message = requestData.Message;

        if (!message.Headers.Accept.Any())
        {
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        return await client.SendExpectingJsonResponseAsync<T>(
            message,
            requestData.RequestContentForLog,
            tppReportingRequestInfo,
            jsonSerializerSettings);
    }

    public async Task<string> SendExpectingStringResponseAsync(
        string acceptHeader,
        IApiClient client,
        TppReportingRequestInfo? tppReportingRequestInfo)

    {
        client.ArgNotNull(nameof(client));
        (HttpRequestMessage Message, string? RequestContentForLog) requestData = RequestInfo.CreateRequestMessage();
        using HttpRequestMessage message = requestData.Message;
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(acceptHeader));
        return await client.SendExpectingStringResponseAsync(
            message,
            requestData.RequestContentForLog,
            tppReportingRequestInfo);
    }

    public HttpRequestMessage CreateHttpRequestMessage() => RequestInfo.CreateRequestMessage().Message;

    public async Task SendExpectingNoResponseAsync(
        TppReportingRequestInfo? tppReportingRequestInfo,
        IApiClient client)
    {
        client.ArgNotNull(nameof(client));
        (HttpRequestMessage Message, string? RequestContentForLog) requestData = RequestInfo.CreateRequestMessage();
        using HttpRequestMessage message = requestData.Message;
        if (!message.Headers.Accept.Any())
        {
            message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        await client.SendExpectingNoResponseAsync(message, requestData.RequestContentForLog, tppReportingRequestInfo);
    }
}
