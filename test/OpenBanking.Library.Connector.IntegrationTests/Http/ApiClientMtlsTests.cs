// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Instrumentation;
using NSubstitute;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Http;

// Mirrors the real bank-registration/API-call path (ObWacCertificate.ApiClient): same hardened
// TLS as ApiClientNonMtlsTests, plus a client certificate for mutual TLS. About client-cert
// presentation, not server-cert trust - DefaultServerCertificateValidator is scaffolding here too
// (see ApiClientNonMtlsTests for the test covering real default cert validation).
[Trait(TraitTypes.TestTarget, TestTypes.LocalMocks)]
public class ApiClientMtlsTests : IClassFixture<MtlsWireMockFixture>
{
    private readonly WireMockServer _server;

    public ApiClientMtlsTests(MtlsWireMockFixture fixture)
    {
        _server = fixture.Server;
        _server.Reset();
    }

    [Fact]
    public async Task ApiClient_WithClientCertificate_MtlsHandshakeSucceeds()
    {
        _server
            .Given(Request.Create().WithPath("/stuff").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200));

        using X509Certificate2 clientCert = TestCertificates.CreateSelfSignedClientCertificate();
        using ApiClient api = CreateApiClient(new List<X509Certificate2> { clientCert });

        HttpResponseMessage response = await api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/stuff"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ApiClient_WithoutClientCertificate_MtlsHandshakeFails()
    {
        _server
            .Given(Request.Create().WithPath("/stuff").UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200));

        using ApiClient api = CreateApiClient(null);

        // Proves the previous test's success is down to the certificate, not an accident of
        // WireMock's HTTPS setup.
        await Assert.ThrowsAnyAsync<HttpRequestException>(
            () => api.LowLevelSendAsync(CreateGetRequest(_server.Url + "/stuff")));
    }

    private static ApiClient CreateApiClient(IList<X509Certificate2>? clientCertificates) =>
        new(
            Substitute.For<IInstrumentationClient>(),
            60,
            ApiClientTestHelpers.CreateTppReportingMetrics(),
            clientCertificates,
            new DefaultServerCertificateValidator());

    private static HttpRequestMessage CreateGetRequest(string uri) =>
        new HttpRequestBuilder().SetMethod(HttpMethod.Get).SetUri(uri).CreateHttpRequestMessage();
}
