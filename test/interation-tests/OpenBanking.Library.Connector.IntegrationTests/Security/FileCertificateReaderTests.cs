// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.Reflection;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.IntegrationTests.Security
{
    public class FileCertificateReaderTests
    {
        //[Fact]
        // public void GetCertificateAsync_FileFound_CertificateReturned()
        // {
        //     var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //
        //     var fileName = "localtest.pfx";
        //
        //     var password = new NetworkCredential("", "password").SecurePassword;
        //     var certificate = new FileCertificateReader(path).GetCertificateAsync(fileName, password).Result;
        //
        //     certificate.Should().NotBeNull();
        // }

        [Fact]
        public void GetCertificateAsync_FileNotFound_CertificateNotReturned()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var fileName = Guid.NewGuid() + ".pfx";

            var certificate = new FileCertificateReader(path).GetCertificateAsync(fileName).Result;

            certificate.Should().BeNull();
        }
    }
}
