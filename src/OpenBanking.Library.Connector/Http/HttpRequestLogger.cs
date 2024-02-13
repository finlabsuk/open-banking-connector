// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using Microsoft.Extensions.Http.Logging;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http;

public class HttpRequestLoggerAdditionalData
{
    public required string? RequestBody { get; init; }

    public required string? ResponseBody { get; init; }
}

public class HttpRequestLogger(IInstrumentationClient instrumentationClient) : IHttpClientLogger
{
    private readonly IInstrumentationClient _instrumentationClient =
        instrumentationClient ?? throw new ArgumentNullException(nameof(instrumentationClient));

    public object? LogRequestStart(HttpRequestMessage request) => null;

    public void LogRequestStop(
        object? context,
        HttpRequestMessage request,
        HttpResponseMessage response,
        TimeSpan elapsed) =>
        LogRequest(_instrumentationClient, request, response, context as HttpRequestLoggerAdditionalData, elapsed);

    public void LogRequestFailed(
        object? context,
        HttpRequestMessage request,
        HttpResponseMessage? response,
        Exception exception,
        TimeSpan elapsed)
        => LogRequest(_instrumentationClient, request, response, context as HttpRequestLoggerAdditionalData, elapsed);

    private static void LogRequest(
        IInstrumentationClient instrumentationClient,
        HttpRequestMessage request,
        HttpResponseMessage? response,
        HttpRequestLoggerAdditionalData? additionalData,
        TimeSpan elapsed)
    {
        string jsonFormatted;

        // Generate HTTP request info trace
        StringBuilder requestTraceSb = new StringBuilder()
            .AppendLine("#### HTTP REQUEST")
            .AppendLine("######## REQUEST")
            .AppendLine($"{request}")
            .AppendLine("######## REQUEST BODY");

        // Log request body
        string? requestBody = additionalData?.RequestBody;
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

        // Log response
        requestTraceSb.AppendLine("######## RESPONSE");
        if (response is null)
        {
            requestTraceSb.AppendLine("<No Response>");
        }
        else
        {
            // Log response body
            requestTraceSb
                .AppendLine($"{response}")
                .AppendLine("######## RESPONSE BODY");
            string? responseBody = additionalData?.ResponseBody;
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

        requestTraceSb.AppendLine($"######## ELAPSED TIME: {double.Round(elapsed.TotalMilliseconds)} ms");

        requestTraceSb.AppendLine("####");
        instrumentationClient.Trace(requestTraceSb.ToString());
    }
}
