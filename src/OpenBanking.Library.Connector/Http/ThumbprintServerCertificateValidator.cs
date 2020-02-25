// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    public class ThumbprintServerCertificateValidator : IServerCertificateValidator
    {
        private readonly IKeySecretProvider _keySecrets;
        private readonly Lazy<HashSet<string>> _thirdPartyThumbprints;

        public ThumbprintServerCertificateValidator(IKeySecretProvider keySecrets)
        {
            _keySecrets = keySecrets.ArgNotNull(nameof(keySecrets));
            _thirdPartyThumbprints = new Lazy<HashSet<string>>(GetThumbprints(GetServerCertificatesAsync().Result));
        }

        public bool IsOk(HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors)
        {
            var tp = cert.Thumbprint;

            return _thirdPartyThumbprints.Value.Contains(tp);
        }

        private async Task<IEnumerable<X509Certificate2>> GetServerCertificatesAsync()
        {
            var pems = await KeySecrets.GetThirdPartyCertificateNames()
                .Select(_keySecrets.GetKeySecretAsync)
                .ToArray()
                .WaitAll();

            var result = new List<X509Certificate2>();
            var rdr = new PemParsingCertificateReader();

            foreach (var secret in pems.Where(ks => ks != null))
            {
                var cert = await rdr.GetCertificateAsync(secret.Value, new SecureString());
                if (cert != null)
                {
                    result.Add(cert);
                }
            }

            return result;
        }

        private HashSet<string> GetThumbprints(IEnumerable<X509Certificate2> certificates)
        {
            return certificates.Select(c => c.Thumbprint).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
