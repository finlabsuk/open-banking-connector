// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public static class CertificateFactories
    {
        public static void ImportPrivateKey(string privateKey, ref RSA rsaContainer)
        {
            string cleanedPrivateKey;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cleanedPrivateKey = Regex.Replace(privateKey, @"\\n", Environment.NewLine);
            }
            else
            {
                cleanedPrivateKey = privateKey;
            }

            string[] privateKeyBlocks = cleanedPrivateKey.Split(
                "-",
                StringSplitOptions.RemoveEmptyEntries);
            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);

            if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
            {
                rsaContainer.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
            {
                rsaContainer.ImportRSAPrivateKey(privateKeyBytes, out _);
            }
        }

        public static X509Certificate2? GetCertificate2FromPem(string privateKey, string pem)
        {
            string cleanedPrivateKey;
            string cleanedPem;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                cleanedPrivateKey = Regex.Replace(privateKey, @"\\n", Environment.NewLine);
                cleanedPem = Regex.Replace(pem, @"\\n", Environment.NewLine);
            }
            else
            {
                cleanedPrivateKey = privateKey;
                cleanedPem = pem;
            }
            //instrumentationClient.Info(cleanedPrivateKey);
            //instrumentationClient.Info(cleanedPem);

            string[] privateKeyBlocks = cleanedPrivateKey.Split(
                "-",
                StringSplitOptions.RemoveEmptyEntries);

            byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
            using var rsa = RSA.Create();

            if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            }

            using X509Certificate2? publicKey = GetCertificate2FromPem(cleanedPem);
            using X509Certificate2? certWithKey = publicKey?.CopyWithPrivateKey(rsa);

            return certWithKey is null
                ? null
                : new X509Certificate2(certWithKey.Export(X509ContentType.Pkcs12));
        }

        public static X509Certificate2? GetCertificate2FromPem(string pem)
        {
            if (pem.ArgNotNull(nameof(pem)).IsPemThumbprint())
            {
                string[] publicKeyBlocks = pem.Split("-", StringSplitOptions.RemoveEmptyEntries);
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
