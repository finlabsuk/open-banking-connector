// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

namespace FinnovationLabs.OpenBanking.Library.Connector.KeySecrets
{
    public static class Secrets
    {
        public const int MaxSoftwareStatements = 5;
        public const string SigningKeyId = "signingKeyId";
        public const string SoftwareStatement = "softwareStatement";
        public const string SigningCertificateKey = "signingCertificateKey";
        public const string SigningCertificate = "signingCertificate";
        public const string TransportCertificateKey = "transportCertificateKey";
        public const string TransportCertificate = "transportCertificate";

        public static string GetName(string softwareStatementId, string name) =>
            $"softwareStatementProfile.{softwareStatementId}.{name}";

        public static IEnumerable<string> GetThirdPartyCertificateNames() =>
            Enumerable.Range(start: 1, count: 100).Select(i => $"thirdPartyCertificate.{i}.pem");
    }
}
