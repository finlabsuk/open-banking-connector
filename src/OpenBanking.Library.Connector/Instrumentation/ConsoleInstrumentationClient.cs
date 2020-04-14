// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace FinnovationLabs.OpenBanking.Library.Connector.Instrumentation
{
    public class ConsoleInstrumentationClient : IInstrumentationClient
    {
        private readonly TextWriter _outWriter;

        [ExcludeFromCodeCoverage]
        public ConsoleInstrumentationClient()
            : this(Console.Out)
        {
        }

        public ConsoleInstrumentationClient(TextWriter outWriter)
        {
            _outWriter = outWriter.ArgNotNull(nameof(outWriter));
        }

        public void StartTrace(TraceInfo info)
        {
            var msg = GetTraceInfoMessage(info);

            Write(_outWriter, msg.ToString(), ConsoleColor.Gray);
        }


        public void EndTrace(TraceInfo info)
        {
            var msg = GetTraceInfoMessage(info);

            Write(_outWriter, msg.ToString(), ConsoleColor.Gray);
        }

        public void Info(string message)
        {
            Write(_outWriter, message, ConsoleColor.White);
        }

        public void Warning(string message)
        {
            Write(_outWriter, message, ConsoleColor.Yellow);
        }

        public void Error(string message)
        {
            Write(_outWriter, message, ConsoleColor.Red);
        }

        public void Exception(Exception exception)
        {
            Exception(exception, "");
        }

        public void Exception(Exception exception, string message)
        {
            var lines = exception.WalkRecursive(e => e.InnerException)
                .Select(e => e.Message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                lines = new[] { message }.Concat(lines);
            }

            var msg = lines.JoinString(Environment.NewLine);

            Write(_outWriter, msg, ConsoleColor.Red);
        }

        private void Write(TextWriter target, string message, ConsoleColor? color)
        {
            lock (Console.Out)
            {
                if (color is { } value)
                {
                    Console.ForegroundColor = value; 
                }
                target.WriteLine(message);
                Console.ResetColor();
            }
        }


        private StringBuilder GetTraceInfoMessage(TraceInfo info)
        {
            var sb = new StringBuilder().AppendLine(info.Message);

            foreach (var kvp in info.Values)
            {
                sb.AppendLine($"{kvp.Key} - {kvp.Value}");
            }

            if (info is HttpTraceInfo httpInfo)
            {
                sb.AppendLine($"Url: {httpInfo.Url}");
                if (httpInfo.StatusCode.HasValue)
                {
                    sb.Append($"Status: {httpInfo.StatusCode}");
                }
            }

            return sb;
        }
    }
}
