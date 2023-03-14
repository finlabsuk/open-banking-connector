// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security;

internal class FileCertificateReader : ICertificateReader
{
    private readonly IIoFacade _ioFacade;

    public FileCertificateReader() : this(new IoFacade()) { }

    public FileCertificateReader(string rootFolder) : this(new IoFacade(() => rootFolder)) { }

    internal FileCertificateReader(IIoFacade ioFacade)
    {
        _ioFacade = ioFacade.ArgNotNull(nameof(ioFacade));
    }

    public Task<X509Certificate2?> GetCertificateAsync(string fileName)
    {
        X509Certificate2? result = GetX509Certificate2(fileName, new SecureString());

        return result.ToTaskResult();
    }


    public Task<X509Certificate2?> GetCertificateAsync(string fileName, SecureString password)
    {
        X509Certificate2? result = GetX509Certificate2(fileName, password);

        return result.ToTaskResult();
    }

    private X509Certificate2? GetX509Certificate2(string fileName, SecureString password)
    {
        X509Certificate2? result = null;
        if (!string.IsNullOrEmpty(fileName))
        {
            string appdata = _ioFacade.GetContentPath();

            string? certFile = _ioFacade.GetDirectoryFiles(appdata, fileName).FirstOrDefault();
            if (!string.IsNullOrEmpty(certFile))
            {
                result = GetCertificateFromFileAsync(certFile, password);
            }
        }

        return result;
    }


    private X509Certificate2 GetCertificateFromFileAsync(string certFile, SecureString password)
    {
        return new X509Certificate2(
            certFile,
            password,
            X509KeyStorageFlags.MachineKeySet |
            X509KeyStorageFlags.PersistKeySet |
            X509KeyStorageFlags.Exportable);
    }
}
