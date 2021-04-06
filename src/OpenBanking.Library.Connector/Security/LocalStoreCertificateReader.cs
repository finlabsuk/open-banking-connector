﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    [ExcludeFromCodeCoverage]
    public class LocalStoreCertificateReader : ICertificateReader
    {
        public Task<X509Certificate2?> GetCertificateAsync(string thumbprint)
        {
            return GetCertificateInner(thumbprint, null).ToTaskResult();
        }

        public Task<X509Certificate2?> GetCertificateAsync(string thumbprint, SecureString password)
        {
            return GetCertificateInner(thumbprint, password).ToTaskResult();
        }

        private X509Certificate2? GetCertificateInner(string value, SecureString? password)
        {
            using X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);

            return store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    value,
                    false)
                .OfType<X509Certificate2>()
                .FirstOrDefault();
        }
    }
}
