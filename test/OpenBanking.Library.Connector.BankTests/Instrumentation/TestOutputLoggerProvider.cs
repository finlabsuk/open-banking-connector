// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable MSTESTEXP // allows use of TestContext.Current
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Instrumentation;

/// <summary>
///     ILoggerProvider that routes all ILogger output to the current test's context
///     via TestContext.Current. Used because console provider does not correctly route logs
///     from BankTestingFixture (DUT) to correct test's output when tests are run in parallel.
/// </summary>
public sealed class TestOutputLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new TestOutputLogger(categoryName);

    public void Dispose() { }
}

file sealed class TestOutputLogger(string categoryName) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        // Try to produce same log output as console provider using "simple" formatter

        string level = logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => "none"
        };

        const string indent = "      ";
        var timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        string body = formatter(state, exception).ReplaceLineEndings($"\n{indent}").TrimEnd();
        var message = $"{timestamp} {level}: {categoryName}[{eventId.Id}]\n{indent}{body}";
        if (exception is not null)
        {
            message += $"\n{indent}{exception.ToString().ReplaceLineEndings($"\n{indent}")}";
        }

        Action<string> write = TestContext.Current is { } ctx ? ctx.WriteLine : Console.WriteLine;
        write(message);
    }
}
