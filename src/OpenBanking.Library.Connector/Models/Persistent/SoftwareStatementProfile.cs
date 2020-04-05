// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    public class SoftwareStatementProfile: IEntity
    {
        public string Id { get; set; }

        public string State { get; set; }

        public string SoftwareStatementHeaderBase64 { get; set; }

        public string SoftwareStatementPayloadBase64 { get; set; }

        public SoftwareStatementPayload SoftwareStatementPayload { get; set; }

        public string SoftwwareStatementSignatureBase64 { get; set; }

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement =>
            new[]
            {
                SoftwareStatementHeaderBase64,
                SoftwareStatementPayloadBase64,
                SoftwwareStatementSignatureBase64
            }.JoinString(".");

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
        public string DefaultFragmentRedirectUrl { get; set; }
    }
}
