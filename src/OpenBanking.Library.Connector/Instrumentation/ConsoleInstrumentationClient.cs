// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Instrumentation
{
    public class ConsoleInstrumentationClient : IInstrumentationClient
    {
        private readonly TextWriter _outWriter;

        [ExcludeFromCodeCoverage]
        public ConsoleInstrumentationClient()
            : this(Console.Out) { }

        public ConsoleInstrumentationClient(TextWriter outWriter)
        {
            _outWriter = outWriter.ArgNotNull(nameof(outWriter));
        }

        public void StartTrace(TraceInfo info)
        {
            StringBuilder msg = GetTraceInfoMessage(info);

            Write(target: _outWriter, message: msg.ToString(), color: ConsoleColor.Gray);
        }


        public void EndTrace(TraceInfo info)
        {
            StringBuilder msg = GetTraceInfoMessage(info);

            Write(target: _outWriter, message: msg.ToString(), color: ConsoleColor.Gray);
        }

        public void Info(string message)
        {
            Write(target: _outWriter, message: message, color: ConsoleColor.White);
        }

        public void Warning(string message)
        {
            Write(target: _outWriter, message: message, color: ConsoleColor.Yellow);
        }

        public void Error(string message)
        {
            Write(target: _outWriter, message: message, color: ConsoleColor.Red);
        }

        public void Exception(Exception exception)
        {
            Exception(exception: exception, message: "");
        }

        public void Exception(Exception exception, string message)
        {
            IEnumerable<string> lines = exception.WalkRecursive(e => e.InnerException)
                .Select(e => e.Message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                lines = new[] { message }.Concat(lines);
            }

            string msg = lines.JoinString(Environment.NewLine);

            Write(target: _outWriter, message: msg, color: ConsoleColor.Red);
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
            StringBuilder sb = new StringBuilder().AppendLine(info.Message);

            foreach (KeyValuePair<string, string> kvp in info.Values)
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
