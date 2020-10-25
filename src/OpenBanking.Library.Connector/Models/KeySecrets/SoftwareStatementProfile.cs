// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets
{
    /// <summary>
    ///     Class that captures a software statement and associated keys and certificates.
    ///     This class is a KeySecret item - i.e. properties are stored as key secrets in advance
    ///     of running OBC.
    /// </summary>
    public class SoftwareStatementProfile : IKeySecretItem
    {
        public SoftwareStatementProfile(
            string softwareStatement,
            string signingKeyId,
            string signingKey,
            string signingCertificate,
            string transportKey,
            string transportCertificate,
            string defaultFragmentRedirectUrl,
            string id)
        {
            SoftwareStatement = softwareStatement ?? throw new ArgumentNullException(nameof(softwareStatement));
            SigningKeyId = signingKeyId ?? throw new ArgumentNullException(nameof(signingKeyId));
            SigningKey = signingKey ?? throw new ArgumentNullException(nameof(signingKey));
            SigningCertificate = signingCertificate ?? throw new ArgumentNullException(nameof(signingCertificate));
            TransportKey = transportKey ?? throw new ArgumentNullException(nameof(transportKey));
            TransportCertificate =
                transportCertificate ?? throw new ArgumentNullException(nameof(transportCertificate));
            DefaultFragmentRedirectUrl = defaultFragmentRedirectUrl ??
                                         throw new ArgumentNullException(nameof(defaultFragmentRedirectUrl));
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public SoftwareStatementProfile() { }

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement { get; set; } = null!;

        /// Open Banking Signing Key ID as string, e.g. "ABC"
        public string SigningKeyId { get; set; } = null!;

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        public string SigningKey { get; set; } = null!;

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string SigningCertificate { get; set; } = null!;

        /// Open Banking Transport Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        public string TransportKey { get; set; } = null!;

        /// Open Banking Transport Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string TransportCertificate { get; set; } = null!;

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; } = null!;

        /// Software statement profile ID as string, e.g. "DevPispSoftwareStatement"
        /// This is your choice; a meaningful name should help debugging throughout OBC.
        public string Id { get; set; } = null!;

        public string HttpClientName() => $"software-statement-profile:{Id}";
    }
}
