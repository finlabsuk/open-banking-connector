// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Http;

// Local HTTPS WireMock.Net server with a self-signed server certificate generated in memory
// (TestCertificates), optionally requiring a client certificate (mutual TLS).
public abstract class HttpsWireMockFixture : IDisposable
{
    private readonly X509Certificate2 _serverCertificate;

    protected HttpsWireMockFixture(bool requireClientCertificate)
    {
        _serverCertificate = TestCertificates.CreateSelfSignedServerCertificate();
        try
        {
            var settings = new WireMockServerSettings
            {
                UseSSL = true,
                CertificateSettings = new WireMockCertificateSettings { X509Certificate = _serverCertificate }
            };
            if (requireClientCertificate)
            {
                settings.ClientCertificateMode = ClientCertificateMode.RequireCertificate;
                settings.AcceptAnyClientCertificate = true;
            }
            Server = WireMockServer.Start(settings);
        }
        catch
        {
            _serverCertificate.Dispose();
            throw;
        }
    }

    public WireMockServer Server { get; }

    public void Dispose()
    {
        try
        {
            Server.Stop();
            Server.Dispose();
        }
        finally
        {
            _serverCertificate.Dispose();
        }
    }
}
