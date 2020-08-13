// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Persistence;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent
{
    public class SoftwareStatementProfile : IEntity
    {
        public string State { get; set; }

        public string SoftwareStatementHeaderBase64 { get; set; }

        // TODO: Remove this once SoftwareStatementPayload stores all fields, this duplicates info in SoftwareStatementPayload
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
        public string SigningKey { get; set; }

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string SigningCertificate { get; set; }

        /// Open Banking Transport Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        /// TODO: This will be replaced by a secret name
        public string TransportKey { get; set; }

        /// Open Banking Transport Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string TransportCertificate { get; set; }

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; }

        public string Id { get; set; }

        public string SoftwareStatementPayloadToBase64(SoftwareStatementPayload payload)
        {
            string jsonData = JsonConvert.SerializeObject(payload);
            return Base64UrlEncoder.Encode(jsonData);
        }

        public SoftwareStatementPayload SoftwareStatementPayloadFromBase64(string payloadBase64)
        {
            // Perform conversion
            string payloadString = Base64UrlEncoder.Decode(payloadBase64);
            SoftwareStatementPayload newObject = JsonConvert.DeserializeObject<SoftwareStatementPayload>(payloadString);

            // Check reverse conversion works or throw
            // (If reverse conversion fails, we can never re-generate base64 correctly)
            // if (payloadBase64 != SoftwareStatementPayloadToBase64(newObject))
            // {
            //     throw new ArgumentException("Please update SoftwareStatementPayload type to support your software statement");
            // }

            return newObject;
        }
    }
}
