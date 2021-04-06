// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Security;
using FluentAssertions;
using Xunit;

namespace FinnovationLabs.OpenBanking.Library.Connector.UnitTests.Security
{
    public class CertificateExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("THUMBPRINT")]
        [InlineData("PEM")]
        [InlineData("begin CERTIFICATE")]
        [InlineData(" BEGIN CERTIFICATE")]
        public void IsPemThumbprint_False(string tp)
        {
            bool result = tp.IsPemThumbprint();

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("-----BEGIN CERTIFICATE-----")]
        [InlineData("-----BEGIN CERTIFICATE------")]
        public void IsPemThumbprint_True(string tp)
        {
            bool result = tp.IsPemThumbprint();

            result.Should().BeTrue();
        }
    }
}
