// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace FinnovationLabs.OpenBanking.Library.Connector.Model.Public
{
    /// Class that captures a software statement and associated keys and certificates
    [PersistenceEquivalent(typeof(Persistent.SoftwareStatementProfile))]
    public class SoftwareStatementProfile
    {
        /// Software statement profile ID as string, e.g. "A.B.C"
        [Required]
        public string Id { get; set; }

        /// Software statement as string, e.g. "A.B.C"
        [Required]
        public string SoftwareStatement { get; set; }

        /// Open Banking Signing Key ID as string, e.g. "ABC"
        [Required]
        public string ObSigningKid { get; set; }

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        [Required]
        public string ObSigningKey { get; set; }

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        [Required]
        public string ObSigningPem { get; set; }

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        [Required]
        public string ObTransportKey { get; set; }

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        [Required]
        public string ObTransportPem { get; set; }

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; }
    }
}
