// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Instrumentation
{
    public class ConsoleInstrumentationClientTests
    {
        [Theory]
        [InlineData("just a test")]
        [InlineData(" just a test ")]
        public void Logging_Info_Recorded(string message)
        {
            using (var ms = new MemoryStream())
            {
                var w = new StreamWriter(ms);
                var logger = new ConsoleInstrumentationClient(w);

                logger.Info(message);

                w.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                string result = new StreamReader(ms).ReadToEnd();

                result.Should().Contain(message);
            }
        }

        [Theory]
        [InlineData("just a test")]
        [InlineData(" just a test ")]
        public void Logging_Warning_Recorded(string message)
        {
            using (var ms = new MemoryStream())
            {
                var w = new StreamWriter(ms);
                var logger = new ConsoleInstrumentationClient(w);

                logger.Warning(message);

                w.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                string result = new StreamReader(ms).ReadToEnd();

                result.Should().Contain(message);
            }
        }


        [Theory]
        [InlineData("just a test")]
        [InlineData(" just a test ")]
        public void Logging_Error_Recorded(string message)
        {
            using (var ms = new MemoryStream())
            {
                var w = new StreamWriter(ms);
                var logger = new ConsoleInstrumentationClient(w);

                logger.Error(message);

                w.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                string result = new StreamReader(ms).ReadToEnd();

                result.Should().Contain(message);
            }
        }


        [Theory]
        [InlineData("just a test", " exception message ")]
        [InlineData("just a test", "ex msg")]
        [InlineData(" just a test ", " ex msg ")]
        public void Logging_Exception_Recorded(string message, string exceptionMessage)
        {
            var ex = new Exception(exceptionMessage);
            using (var ms = new MemoryStream())
            {
                var w = new StreamWriter(ms);
                var logger = new ConsoleInstrumentationClient(w);

                logger.Exception(ex, message);

                w.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                string result = new StreamReader(ms).ReadToEnd();


                result.Should().Contain(exceptionMessage);
                result.Should().Contain(message);
            }
        }


        [Theory]
        [InlineData("just a test", "end of test")]
        [InlineData(" just a test ", " end of test ")]
        public void Logging_StartTrace_EndTrace_Recorded(string startMsg, string endMsg)
        {
            using (var ms = new MemoryStream())
            {
                var w = new StreamWriter(ms);
                var logger = new ConsoleInstrumentationClient(w);


                logger.StartTrace(new TraceInfo(startMsg));
                logger.EndTrace(new TraceInfo(endMsg));

                w.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                string result = new StreamReader(ms).ReadToEnd();

                result = result.Trim(Environment.NewLine.ToCharArray());

                result.Should().StartWith(startMsg);
                result.Should().EndWith(endMsg);
            }
        }

        [Theory]
        [InlineData("msg1", "just a test", "end of test")]
        [InlineData(" msg2 ", " just a test ", " end of test ")]
        public void Logging_StartTrace_TraceRecorded(string msg, params string[] msgs)
        {
            var trace = new TraceInfo(msg);
            for (var x = 0; x < msgs.Length; x++)
            {
                trace.Add($"stuff{x}", msgs[x]);
            }


            using (var ms = new MemoryStream())
            {
                var w = new StreamWriter(ms);
                var logger = new ConsoleInstrumentationClient(w);

                logger.StartTrace(trace);


                w.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                string result = new StreamReader(ms).ReadToEnd();

                result = result.Trim(Environment.NewLine.ToCharArray());

                result.Should().StartWith(msg);
                foreach (string m in msgs)
                {
                    result.Should().Contain(m);
                }
            }
        }
    }
}
