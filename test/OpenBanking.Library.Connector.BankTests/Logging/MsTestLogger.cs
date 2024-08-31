// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Logging;

public class MsTestLogger(string name, Func<TestContext?> getTestContext) : ILogger
{
    public IDisposable BeginScope<TState>(TState state)
        where TState : notnull => default!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        TestContext? testContext = getTestContext();

        testContext?.WriteLine($"XXXX {name}");
    }
}
