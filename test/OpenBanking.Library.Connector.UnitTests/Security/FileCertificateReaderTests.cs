// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security;

public class FileCertificateReaderTests
{
    [Fact]
    public void Ctor_NullFacade_ExceptionThrown()
    {
        Func<FileCertificateReader> f = () => new FileCertificateReader((IIoFacade) null!);

        f.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_NonNullFacade_NoExceptionThrown()
    {
        Func<FileCertificateReader> f = () => new FileCertificateReader(Substitute.For<IIoFacade>());

        f.Should().NotThrow<ArgumentNullException>();
    }


    [Fact]
    public async Task GetCertificateAsync_EmptyThumbprint_NullReturned()
    {
        var thumbprint = "";

        var ioFacade = Substitute.For<IIoFacade>();

        var rdr = new FileCertificateReader(ioFacade);
        X509Certificate2? result = await rdr.GetCertificateAsync(thumbprint);

        result.Should().BeNull();
        ioFacade.DidNotReceive().GetContentPath();
        ioFacade.DidNotReceive().GetDirectoryFiles(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task GetCertificateAsync_NoFiles_NullReturned()
    {
        var files = new string[0];
        var contentPath = "";
        var thumbprint = "abc";

        var ioFacade = Substitute.For<IIoFacade>();

        ioFacade.GetContentPath().Returns(contentPath);
        ioFacade.GetDirectoryFiles(Arg.Any<string>(), Arg.Any<string>()).Returns(files);


        var rdr = new FileCertificateReader(ioFacade);
        X509Certificate2? result = await rdr.GetCertificateAsync(thumbprint);

        result.Should().BeNull();
        ioFacade.Received().GetContentPath();
        ioFacade.Received().GetDirectoryFiles(Arg.Any<string>(), Arg.Any<string>());
    }


    [Fact]
    public async Task GetCertificateAsync_DirectoryNamespaceQueried()
    {
        string[] files = { "file.cert" };
        var contentPath = "";
        var thumbprint = "abc";

        var ioFacade = Substitute.For<IIoFacade>();

        ioFacade.GetContentPath().Returns(contentPath);
        ioFacade.GetDirectoryFiles(contentPath, Arg.Any<string>()).Returns(files);


        var rdr = new FileCertificateReader(ioFacade);

        try
        {
            X509Certificate2? _ = await rdr.GetCertificateAsync(thumbprint);

            throw new InvalidOperationException("Test failed");
        }
        catch (Exception ex)
            when (
                ex.Message == "The system cannot find the file specified." || // Windows
                ex.Message.StartsWith("Could not find file") || // macOS
                ex.Message.StartsWith("error:10000080:BIO routines::no such file") // Linux
            )
        {
            ioFacade.Received().GetContentPath();
            ioFacade.Received().GetDirectoryFiles(contentPath, Arg.Any<string>());
        }
    }
}
