// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    /// <summary>
    ///     Disable verification of external TLS certificates. Not for production use but
    ///     helpful when testing against sandboxes using self-signed certificates.
    /// </summary>
    public class DefaultServerCertificateValidator : IServerCertificateValidator
    {
        public bool IsOk(object stateInfo, X509Certificate? cert, X509Chain? chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
