// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using FinnovationLabs.OpenBanking.Library.Connector.Services;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Instrumentation
{
    internal class TestInstrumentationClient : IInstrumentationClient
    {
        private readonly ITestOutputHelper _outputHelper;
        private readonly ITimeProvider _timeProvider;

        public TestInstrumentationClient(ITestOutputHelper outputHelper, ITimeProvider timeProvider)
        {
            _outputHelper = outputHelper;
            _timeProvider = timeProvider;
        }

        public void StartTrace(TraceInfo info)
        {
            _outputHelper.WriteLine("Start Trace");
        }

        public void EndTrace(TraceInfo info)
        {
            _outputHelper.WriteLine("End Trace");
        }

        public void Info(string message)
        {
            _outputHelper.WriteLine($"[{_timeProvider.GetUtcNow().ToString(CultureInfo.InvariantCulture)}] info:");
            _outputHelper.WriteLine($"{message}");
        }

        public void Warning(string message)
        {
            _outputHelper.WriteLine($"[{_timeProvider.GetUtcNow().ToString(CultureInfo.InvariantCulture)}] warning:");
            _outputHelper.WriteLine($"{message}");
        }

        public void Error(string message)
        {
            _outputHelper.WriteLine($"[{_timeProvider.GetUtcNow().ToString(CultureInfo.InvariantCulture)}] error:");
            _outputHelper.WriteLine($"{message}");
        }

        public void Exception(Exception exception) => Exception(exception, exception.Message);

        public void Exception(Exception exception, string message)
        {
            _outputHelper.WriteLine($"[{_timeProvider.GetUtcNow().ToString(CultureInfo.InvariantCulture)}] exception:");
            _outputHelper.WriteLine($"{message}");
        }
    }
}
