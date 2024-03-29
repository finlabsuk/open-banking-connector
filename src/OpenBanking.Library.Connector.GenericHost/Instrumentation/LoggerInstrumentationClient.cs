﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.GenericHost.Instrumentation;

public class LoggerInstrumentationClient<TSource> : IInstrumentationClient
    where TSource : class
{
    private readonly ILogger<TSource> _logger;

    public LoggerInstrumentationClient(ILogger<TSource> logger)
    {
        _logger = logger;
    }

    public void Info(string message) => _logger.LogInformation(message);

    public void Warning(string message) => _logger.LogWarning(message);

    public void Error(string message) => _logger.LogError(message);
    public void Trace(string message) => _logger.LogTrace(message);

    public void Exception(Exception exception) =>
        _logger.LogError(exception, exception.Message);

    public void Exception(Exception exception, string message) =>
        _logger.LogError(exception, message);

    public void StartTrace(TraceInfo info) => LogTrace("Start", info);

    public void EndTrace(TraceInfo info) => LogTrace("End", info);

    private void LogTrace(string prefix, TraceInfo info)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(prefix))
        {
            sb.Append($"{prefix}: ");
        }

        sb.AppendLine(info.Message);

        if (info is HttpTraceInfo httpInfo)
        {
            sb.AppendLine($"Url: {httpInfo.Url}");

            if (httpInfo.StatusCode.HasValue)
            {
                sb.AppendLine($"Status: {httpInfo.StatusCode.Value}");
            }
        }

        foreach (KeyValuePair<string, string> keyValuePair in info.Values)
        {
            sb.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value}");
        }

        _logger.LogTrace(sb.ToString());
    }
}
