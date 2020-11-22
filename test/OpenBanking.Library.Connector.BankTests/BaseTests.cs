// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;

namespace FinnovationLabs.OpenBanking.Library.Connector.BankTests
{
    [Collection("App context collection")]
    public class BaseTests
    {
        protected readonly IHost _host;

        protected BaseTests(ITestOutputHelper outputHelper, AppContextFixture appContextFixture)
        {
            _host = appContextFixture.Host;
            OutputHelper = outputHelper ?? throw new ArgumentNullException(nameof(outputHelper));
            AppContextFixture = appContextFixture ?? throw new ArgumentNullException(nameof(appContextFixture));
        }

        public ITestOutputHelper OutputHelper { get; }
        public AppContextFixture AppContextFixture { get; }

        protected void SetTestLogging()
        {
            AppContextFixture.OutputHelper = OutputHelper;
        }

        protected void UnsetTestLogging()
        {
            AppContextFixture.OutputHelper = null;
        }
    }
}
