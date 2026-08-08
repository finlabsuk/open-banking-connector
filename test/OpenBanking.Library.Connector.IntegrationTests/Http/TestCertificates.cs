// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Http;

// Throwaway self-signed certificates generated in memory - no files, CA, config, or secrets.
// Being self-signed, they're untrusted by .NET's default chain validation, so tests that need a
// successful handshake pass DefaultServerCertificateValidator to bypass it (see
// ApiClientNonMtlsTests for the test covering the real, non-bypassed default behaviour).
internal static class TestCertificates
{
    private const string ServerAuthenticationOid = "1.3.6.1.5.5.7.3.1";
    private const string ClientAuthenticationOid = "1.3.6.1.5.5.7.3.2";

    public static X509Certificate2 CreateSelfSignedServerCertificate(string subjectName = "wiremock-test-server") =>
        CreateSelfSigned(subjectName, ServerAuthenticationOid);

    public static X509Certificate2 CreateSelfSignedClientCertificate(string subjectName = "wiremock-test-client") =>
        CreateSelfSigned(subjectName, ClientAuthenticationOid);

    private static X509Certificate2 CreateSelfSigned(string subjectName, string enhancedKeyUsageOid)
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(
            $"CN={subjectName}",
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment,
                false));
        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(new OidCollection { new Oid(enhancedKeyUsageOid) }, false));

        return request.CreateSelfSigned(
            DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddYears(1));
    }
}
