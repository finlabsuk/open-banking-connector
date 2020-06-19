// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public static class CertificateFactories
    {
        public static X509Certificate2 GetCertificate2FromPem(string privateKey, string pem)
        {
            /*
            string cleanedPem = Regex
                    .Replace(pem, @"\r\n|\n\r|\n|\r", Environment.NewLine);
            
            string cleanedPrivateKey =  Regex
                .Replace(privateKey, @"\r\n|\n\r|\n|\r", Environment.NewLine);
                */

            using X509Certificate2 publicKey = GetCertificate2FromPem(pem);
            string[] privateKeyBlocks = privateKey.Split(
                separator: "-",
                options: StringSplitOptions.RemoveEmptyEntries);
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
            using RSA rsa = RSA.Create();

            if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
            {
                rsa.ImportPkcs8PrivateKey(source: privateKeyBytes, bytesRead: out _);
            }
            else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
            {
                rsa.ImportRSAPrivateKey(source: privateKeyBytes, bytesRead: out _);
            }

            using (X509Certificate2 certWithKey = publicKey.CopyWithPrivateKey(rsa))
            {
                return new X509Certificate2(certWithKey.Export(X509ContentType.Pkcs12));
            }
        }

        public static X509Certificate2 GetCertificate2FromPem(string pem)
        {
            if (pem.ArgNotNull(nameof(pem)).IsPemThumbprint())
            {
                string[] publicKeyBlocks = pem.Split(separator: "-", options: StringSplitOptions.RemoveEmptyEntries);
                if (publicKeyBlocks.Length == 4)
                {
                    byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBlocks[1]);
                    return new X509Certificate2(publicKeyBytes);
                }
            }

            return null;
        }
    }
}
