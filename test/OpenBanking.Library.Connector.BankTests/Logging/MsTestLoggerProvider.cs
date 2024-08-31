// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.Logging;

public class MsTestLoggerProvider(Func<TestContext?> getTestContext) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, MsTestLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName) => _loggers.GetOrAdd(
        categoryName,
        name => new MsTestLogger(name, getTestContext));

    public void Dispose() => _loggers.Clear();
}
