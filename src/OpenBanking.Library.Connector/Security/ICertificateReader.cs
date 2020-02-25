// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public interface ICertificateReader
    {
        Task<X509Certificate2> GetCertificateAsync(string value);

        Task<X509Certificate2> GetCertificateAsync(string value, SecureString password);
    }
}
