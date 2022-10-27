// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using Org.BouncyCastle.Crypto;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security
{
    public class PemParsingCertificateReaderTests
    {
        [Fact]
        public void GetCertificateAsync_NullThumbprint_ExceptionThrown()
        {
            var rdr = new PemParsingCertificateReader();

            Action a = () => rdr.GetCertificateAsync(null!);

            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetCertificateAsync_EmptyThumbprint_NullReturned()
        {
            var rdr = new PemParsingCertificateReader();

            Func<Task<X509Certificate2?>> a = () => rdr.GetCertificateAsync("");

            await a.Should().ThrowAsync<CryptographicException>();
        }

        [Fact]
        public async Task GetCertificateAsync__KeyPair_NullReturned()
        {
            AsymmetricCipherKeyPair keyPair = TestCertificateGenerator.GenerateKeyPair();
            using (X509Certificate originCert = TestCertificateGenerator.GenerateCertificate(
                       keyPair,
                       "test",
                       null))
            {
                string pem = TestCertificateGenerator.GetPemTextFromPublicKey(keyPair);

                var rdr = new PemParsingCertificateReader();

                Func<Task<X509Certificate2?>> a = () => rdr.GetCertificateAsync(pem);

                await a.Should().ThrowAsync<CryptographicException>();
            }
        }

        [Fact]
        public async Task GetCertificateAsync_CertInPemThumbprint_NonNullReturned()
        {
            AsymmetricCipherKeyPair keyPair = TestCertificateGenerator.GenerateKeyPair();
            using (X509Certificate originCert = TestCertificateGenerator.GenerateCertificate(
                       keyPair,
                       "test",
                       null))
            {
                X509Certificate cert = TestCertificateGenerator.ToX509V2Cert(originCert);

                string pem = TestCertificateGenerator.GetPemTextFromCertificate(cert);

                var rdr = new PemParsingCertificateReader();

                X509Certificate2? result = await rdr.GetCertificateAsync(pem);

                result.Should().NotBeNull();
            }
        }
    }
}
