// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.NetGenericHost.Instrumentation
{
    internal class LoggerInstrumentationClient : IInstrumentationClient
    {
        private readonly ILogger<object> _logger;

        public LoggerInstrumentationClient(ILogger<object> logger)
        {
            _logger = logger;
        }

        public void StartTrace(TraceInfo info) => LogTrace("Start", info);

        public void EndTrace(TraceInfo info) => LogTrace("End", info);

        public void Info(string message) => _logger.LogInformation(message);

        public void Warning(string message) => _logger.LogWarning(message);

        public void Error(string message) => _logger.LogError(message);

        public void Exception(Exception exception) => _logger.LogError(exception, exception.Message);

        public void Exception(Exception exception, string message) => _logger.LogError(exception, message);

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
                if (httpInfo.Url != null)
                {
                    sb.AppendLine($"Url: {httpInfo.Url}");
                }

                if (httpInfo.StatusCode.HasValue)
                {
                    sb.AppendLine($"Status: {httpInfo.StatusCode.Value}");
                }
            }

            foreach (var keyValuePair in info.Values)
            {
                sb.AppendLine($"{keyValuePair.Key}: {keyValuePair.Value}");
            }

            _logger.LogDebug(sb.ToString());
        }
    }
}
