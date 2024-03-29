﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security;

public class PemParsingCertificateReader : ICertificateReader
{
    public Task<X509Certificate2?> GetCertificateAsync(string fileName) =>
        ((X509Certificate2?) CertificateFactories.CreateCert(fileName)).ToTaskResult();

    public Task<X509Certificate2?> GetCertificateAsync(string value, SecureString password) =>
        throw new NotImplementedException();
}
