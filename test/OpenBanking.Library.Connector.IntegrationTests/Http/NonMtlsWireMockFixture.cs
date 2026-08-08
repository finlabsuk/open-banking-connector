// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Http;

// Local HTTPS WireMock.Net server, no client-certificate requirement - mirrors the
// well-known/openid-configuration code path. Server certificate is self-signed and generated in
// memory (TestCertificates); see ApiClientNonMtlsTests for how each test handles that trust gap.
public sealed class NonMtlsWireMockFixture : IAsyncLifetime
{
    public WireMockServer Server { get; private set; } = null!;

    public ValueTask InitializeAsync()
    {
        Server = WireMockServer.Start(
            new WireMockServerSettings
            {
                UseSSL = true,
                CertificateSettings = new WireMockCertificateSettings
                {
                    X509Certificate = TestCertificates.CreateSelfSignedServerCertificate()
                }
            });
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        Server.Stop();
        Server.Dispose();
        return ValueTask.CompletedTask;
    }
}
