// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security;

public static class CertificateFactories
{
    public static void ImportPrivateKey(string privateKey, ref RSA rsaContainer)
    {
        string cleanedPrivateKey = CleanPem(privateKey);
        rsaContainer.ImportFromPem(cleanedPrivateKey);
    }

    public static X509Certificate2 CreateCertWithKey(string certPem, string keyPem)
    {
        string cleanedKeyPem = CleanPem(keyPem);
        string cleanedCertPem = CleanPem(certPem);
        var cert = X509Certificate2.CreateFromPem(cleanedCertPem, cleanedKeyPem);
        return MakeTlsCompatible(cert);
    }

    /// <summary>
    ///     Ensures a certificate with private key can be used for TLS (as client or server cert). Takes ownership of
    ///     <paramref name="certWithKey" />: use the returned certificate instead (it may be the same instance).
    /// </summary>
    internal static X509Certificate2 MakeTlsCompatible(X509Certificate2 certWithKey)
    {
        // Linux/macOS: ephemeral private keys (as created by CreateFromPem, CreateSelfSigned, etc.) work fine.
        if (!OperatingSystem.IsWindows())
        {
            return certWithKey;
        }

        // Windows only: SChannel can't use ephemeral private keys, so round-trip via PKCS#12 to get a
        // persisted key (temporary key file, deleted when the returned certificate is disposed).
        // https://github.com/dotnet/runtime/issues/45680
        using (certWithKey)
        {
            byte[] pkcs12 = certWithKey.Export(X509ContentType.Pkcs12);
            try
            {
                // null password: workaround from https://support.microsoft.com/en-gb/topic/kb5025823-change-in-how-net-applications-import-x-509-certificates-bf81c936-af2b-446e-9f7a-016f4713b46b
                return X509CertificateLoader.LoadPkcs12(pkcs12, null);
            }
            finally
            {
                CryptographicOperations.ZeroMemory(pkcs12); // wipe plaintext key copy
            }
        }
    }

    private static string CleanPem(string pem)
    {
        // In Bash environment variables (not sure about Windows), "\n" does not represent a newline (it's not an escape sequence) and so
        // gets converted to "\\n" in C#. We convert these back. This problem does not occur with .NET secrets where values
        // contain "\n".
        string cleanedPem = Regex.Replace(pem, @"\\n", "\n");
        return cleanedPem;
    }
}
