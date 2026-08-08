// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Http;

// Local HTTPS WireMock.Net server requiring a client certificate - mirrors the real bank
// registration/API-call path (ObWacCertificate.ApiClient). Both certs are generated in memory
// (TestCertificates). Chain trust is deliberately not checked on either side here -
// AcceptAnyClientCertificate because these tests only care that a cert was presented, and the
// server cert's own trust is covered separately in ApiClientNonMtlsTests.
public sealed class MtlsWireMockFixture : IAsyncLifetime
{
    public WireMockServer Server { get; private set; } = null!;

    public ValueTask InitializeAsync()
    {
        Server = WireMockServer.Start(
            new WireMockServerSettings
            {
                UseSSL = true,
                CertificateSettings =
                    new WireMockCertificateSettings
                    {
                        X509Certificate = TestCertificates.CreateSelfSignedServerCertificate()
                    },
                ClientCertificateMode = ClientCertificateMode.RequireCertificate,
                AcceptAnyClientCertificate = true
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
