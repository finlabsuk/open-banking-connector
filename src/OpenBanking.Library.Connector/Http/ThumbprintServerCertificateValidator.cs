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
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets.Providers;
using FinnovationLabs.OpenBanking.Library.Connector.Security;

namespace FinnovationLabs.OpenBanking.Library.Connector.Http
{
    public class ThumbprintServerCertificateValidator : IServerCertificateValidator
    {
        private readonly IKeySecretReadOnlyProvider _keySecrets;
        private readonly Lazy<HashSet<string>> _thirdPartyThumbprints;

        public ThumbprintServerCertificateValidator(IKeySecretReadOnlyProvider keySecrets)
        {
            _keySecrets = keySecrets.ArgNotNull(nameof(keySecrets));
            _thirdPartyThumbprints = new Lazy<HashSet<string>>(GetThumbprints(GetServerCertificatesAsync().Result));
        }

        public bool IsOk(HttpRequestMessage msg, X509Certificate2 cert, X509Chain chain, SslPolicyErrors errors)
        {
            string tp = cert.Thumbprint;

            return _thirdPartyThumbprints.Value.Contains(tp);
        }

        public static IEnumerable<string> GetThirdPartyCertificateNames() =>
            Enumerable.Range(start: 1, count: 100).Select(i => $"thirdPartyCertificate.{i}.pem");

        private async Task<IEnumerable<X509Certificate2>> GetServerCertificatesAsync()
        {
            KeySecret[] pems = await GetThirdPartyCertificateNames()
                .Select(_keySecrets.GetKeySecretAsync)
                .ToArray()
                .WaitAll();

            List<X509Certificate2> result = new List<X509Certificate2>();
            PemParsingCertificateReader rdr = new PemParsingCertificateReader();

            foreach (KeySecret secret in pems.Where(ks => ks != null))
            {
                X509Certificate2 cert = await rdr.GetCertificateAsync(
                    value: secret.Value,
                    password: new SecureString());
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
