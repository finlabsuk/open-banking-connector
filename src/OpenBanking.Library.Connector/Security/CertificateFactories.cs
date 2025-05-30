﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
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
        var initialCert = X509Certificate2.CreateFromPem(cleanedCertPem, cleanedKeyPem);
        byte[]
            exportedCert =
                initialCert.Export(
                    X509ContentType.Pkcs12); // workaround for issue: https://github.com/dotnet/runtime/issues/45680 
        string?
            password = null; // workaround from https://support.microsoft.com/en-gb/topic/kb5025823-change-in-how-net-applications-import-x-509-certificates-bf81c936-af2b-446e-9f7a-016f4713b46b

        X509Certificate2 loadedCert = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? X509CertificateLoader.LoadPkcs12(exportedCert, password, X509KeyStorageFlags.EphemeralKeySet)
            : X509CertificateLoader.LoadPkcs12(exportedCert, password);

        return loadedCert;
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
