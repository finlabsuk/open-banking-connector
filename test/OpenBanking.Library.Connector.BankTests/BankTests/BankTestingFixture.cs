// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using FinnovationLabs.OpenBanking.Library.Connector.BankTests.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class BankTestingFixture : WebApplicationFactory<Program>
{
    private readonly AsyncLocal<TestContext?>
        _asyncLocalTestContext = new();

    public TestContext? TestContext
    {
        get => _asyncLocalTestContext.Value;
        set => _asyncLocalTestContext.Value = value;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // builder.ConfigureLogging(
        //     logging =>
        //     {
        //         logging.AddProvider(new MsTestLoggerProvider(() => TestContext));
        //     });
        builder.ConfigureServices(
            services => { services.AddSingleton<ILoggerProvider>(new MsTestLoggerProvider(() => TestContext)); });
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            builder.UseContentRoot("");
        }
    }
}
