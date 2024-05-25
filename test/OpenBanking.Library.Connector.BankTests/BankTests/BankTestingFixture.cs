// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests.BankTests;

public class BankTestingFixture : WebApplicationFactory<Program>
{
    public ITestOutputHelper OutputHelper { get; set; } = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(
            x =>
                x.AddXUnit(OutputHelper));
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            builder.UseContentRoot("");
        }
    }
}
