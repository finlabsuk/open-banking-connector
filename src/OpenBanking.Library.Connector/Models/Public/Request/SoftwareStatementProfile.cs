// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Public.Request
{
    /// Class that captures a software statement and associated keys and certificates
    [PersistenceEquivalent(typeof(Persistent.SoftwareStatementProfile))]
    public class SoftwareStatementProfile
    {
        /// Software statement profile ID as string, e.g. "DevPispSoftwareStatement"
        /// This is your choice; a meaningful name should help debugging throughout OBC.
        [Required]
        public string Id { get; set; }

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement { get; set; }

        /// Open Banking Signing Key ID as string, e.g. "ABC"
        public string SigningKeyId { get; set; }

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// TODO: This will be replaced by a secret name
        public string SigningKeySecretName { get; set; }

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string SigningCertificate { get; set; }

        /// Open Banking Transport Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// TODO: This will be replaced by a secret name
        public string TransportKeySecretName { get; set; }

        /// Open Banking Transport Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string TransportCertificate { get; set; }

        /// Default redirect URL for OAuth clients with response_mode == fragment.
       [Required]
        public string DefaultFragmentRedirectUrl { get; set; }
    }
}
