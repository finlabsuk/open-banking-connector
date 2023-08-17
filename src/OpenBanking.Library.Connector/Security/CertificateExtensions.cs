// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Security;

internal static class CertificateExtensions
{
    public static bool IsPemThumbprint(this string? thumbprint) =>
        thumbprint != null &&
        thumbprint.IndexOf("-----BEGIN CERTIFICATE-----", StringComparison.Ordinal) ==
        0;


    public static bool IsPemKey(this string? value) =>
        value != null && value.IndexOf(
            "-----BEGIN PRIVATE KEY-----",
            StringComparison.Ordinal) == 0;
}
