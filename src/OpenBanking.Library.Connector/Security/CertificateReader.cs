// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    internal sealed class CertificateReader : ICertificateReader
    {
        private readonly ICertificateReader _fileCertReader;
        private readonly ICertificateReader _pemReader;

        public CertificateReader(ICertificateReader fileCertReader, ICertificateReader pemReader)
        {
            _fileCertReader = fileCertReader.ArgNotNull(nameof(fileCertReader));
            _pemReader = pemReader.ArgNotNull(nameof(pemReader));
        }

        public async Task<X509Certificate2> GetCertificateAsync(string value)
        {
            value.ArgNotNull(nameof(value));

            X509Certificate2 result;
            if (value.IsPemThumbprint())
            {
                result = await _pemReader.GetCertificateAsync(value);
            }
            else
            {
                result = await _fileCertReader.GetCertificateAsync(value);
            }

            return result;
        }

        public async Task<X509Certificate2> GetCertificateAsync(string value, SecureString password)
        {
            value.ArgNotNull(nameof(value));

            X509Certificate2 result;
            if (value.IsPemThumbprint())
            {
                result = await _pemReader.GetCertificateAsync(value: value, password: password);
            }
            else
            {
                result = await _fileCertReader.GetCertificateAsync(value: value, password: password);
            }

            return result;
        }
    }
}
