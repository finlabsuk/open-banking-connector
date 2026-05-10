// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Instrumentation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class BankTestingFixture : WebApplicationFactory<Program>
{
    public BankTestingFixture()
    {
        Server.PreserveExecutionContext = true; // ensure correct propagation of AsyncLocal<T>
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            builder.UseContentRoot("");
        }

        // Use TestOutputLoggerProvider which routes logs to MSTest test context ensuring
        // logs end up in correct test's output during parallel testing.
        builder.ConfigureLogging(
            logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(new TestOutputLoggerProvider());
            });
    }
}
