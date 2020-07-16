// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    internal static class CertificateExtensions
    {
        public static bool IsPemThumbprint(this string thumbprint)
        {
            return thumbprint != null &&
                   thumbprint.IndexOf(value: "-----BEGIN CERTIFICATE-----", comparisonType: StringComparison.Ordinal) ==
                   0;
        }


        public static bool IsPemKey(this string value)
        {
            return value != null && value.IndexOf(
                value: "-----BEGIN PRIVATE KEY-----",
                comparisonType: StringComparison.Ordinal) == 0;
        }
    }
}
