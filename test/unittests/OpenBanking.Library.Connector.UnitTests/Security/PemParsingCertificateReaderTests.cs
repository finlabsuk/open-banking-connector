// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security
{
    public class PemParsingCertificateReaderTests
    {
        [Fact]
        public void GetCertificateAsync_NullThumbprint_ExceptionThrown()
        {
            var rdr = new PemParsingCertificateReader();

            Action a = () => rdr.GetCertificateAsync(null);

            a.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetCertificateAsync_EmptyThumbprint_NullReturned()
        {
            var rdr = new PemParsingCertificateReader();

            var result = await rdr.GetCertificateAsync("");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCertificateAsync__KeyPair_NullReturned()
        {
            var keyPair = TestCertificateGenerator.GenerateKeyPair();
            using (var originCert = TestCertificateGenerator.GenerateCertificate(keyPair, "test", null))
            {
                var pem = TestCertificateGenerator.GetPemTextFromPublicKey(keyPair);

                var rdr = new PemParsingCertificateReader();

                var result = await rdr.GetCertificateAsync(pem);

                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task GetCertificateAsync_CertInPemThumbprint_NonNullReturned()
        {
            var keyPair = TestCertificateGenerator.GenerateKeyPair();
            using (var originCert = TestCertificateGenerator.GenerateCertificate(keyPair, "test", null))
            {
                var cert = TestCertificateGenerator.ToX509V2Cert(originCert);

                var pem = TestCertificateGenerator.GetPemTextFromCertificate(cert);

                var rdr = new PemParsingCertificateReader();

                var result = await rdr.GetCertificateAsync(pem);

                result.Should().NotBeNull();
            }
        }
    }
}
