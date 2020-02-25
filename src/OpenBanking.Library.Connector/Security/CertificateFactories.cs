// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public static class CertificateFactories
    {
        public static IEnumerable<X509Certificate2> GetCertificates(IKeySecretProvider secrets)
        {
            secrets.ArgNotNull(nameof(secrets));

            var certificates = new List<X509Certificate2>();

            foreach (var x in Enumerable.Range(1, KeySecrets.MaxSoftwareStatements))
            {
                var certName = KeySecrets.GetName(x.ToString(), KeySecrets.TransportCertificate);
                var cert = secrets.GetKeySecretAsync(certName).Result;
                var keyName = KeySecrets.GetName(x.ToString(), KeySecrets.TransportCertificateKey);
                var key = secrets.GetKeySecretAsync(keyName).Result;

                if (cert != null && key != null)
                {
                    var certificate = GetCertificate2FromPem(key.Value, cert.Value);

                    if (certificate != null)
                    {
                        certificates.Add(certificate);
                    }
                }
            }

            return certificates;
        }

        public static X509Certificate2 GetCertificate2FromPem(string privateKey, string pem)
        {
            using var publicKey = GetCertificate2FromPem(pem);
            var privateKeyBlocks = privateKey.Split("-", StringSplitOptions.RemoveEmptyEntries);
            var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
            using var rsa = RSA.Create();

            if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            }

            using (var certWithKey = publicKey.CopyWithPrivateKey(rsa))
            {
                return new X509Certificate2(certWithKey.Export(X509ContentType.Pkcs12));
            }
        }

        public static X509Certificate2 GetCertificate2FromPem(string pem)
        {
            if (pem.ArgNotNull(nameof(pem)).IsPemThumbprint())
            {
                var publicKeyBlocks = pem.Split("-", StringSplitOptions.RemoveEmptyEntries);
                if (publicKeyBlocks.Length == 4)
                {
                    var publicKeyBytes = Convert.FromBase64String(publicKeyBlocks[1]);
                    return new X509Certificate2(publicKeyBytes);
                }
            }

            return null;
        }
    }
}
