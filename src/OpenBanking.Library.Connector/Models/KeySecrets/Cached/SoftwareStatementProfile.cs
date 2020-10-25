// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.KeySecrets;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.KeySecrets.Cached
{
    public class SoftwareStatementProfile : IKeySecretItem
    {
        public SoftwareStatementProfile(KeySecrets.SoftwareStatementProfile profileKeySecrets, ApiClient apiClient)
        {
            // Pass-through properties
            SigningKeyId = profileKeySecrets.SigningKeyId;
            SigningKey = profileKeySecrets.SigningKey;
            SigningCertificate = profileKeySecrets.SigningCertificate;
            DefaultFragmentRedirectUrl = profileKeySecrets.DefaultFragmentRedirectUrl;
            Id = profileKeySecrets.Id;

            // Break software statement into components
            string[] softwareStatementComponentsBase64 = profileKeySecrets.SoftwareStatement.Split(new[] { '.' });
            if (softwareStatementComponentsBase64.Length != 3)
            {
                throw new ArgumentException("softwareStatementComponentsBase64 needs 3 components.");
            }

            SoftwareStatementHeaderBase64 = softwareStatementComponentsBase64[0];
            SoftwareStatementPayloadBase64 = softwareStatementComponentsBase64[1];
            SoftwareStatementPayload =
                SoftwareStatementPayloadFromBase64(softwareStatementComponentsBase64[1]);
            SoftwwareStatementSignatureBase64 = softwareStatementComponentsBase64[2];

            if (SoftwareStatement != profileKeySecrets.SoftwareStatement)
            {
                throw new InvalidOperationException("Can't correctly process software statement");
            }

            // Api client
            ApiClient = apiClient;
        }

        public string SoftwareStatementHeaderBase64 { get; set; } = null!;

        // TODO: Remove this once SoftwareStatementPayload stores all fields, this duplicates info in SoftwareStatementPayload
        public string SoftwareStatementPayloadBase64 { get; set; } = null!;

        public SoftwareStatementPayload SoftwareStatementPayload { get; set; } = null!;

        public string SoftwwareStatementSignatureBase64 { get; set; } = null!;

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement =>
            new[]
            {
                SoftwareStatementHeaderBase64,
                SoftwareStatementPayloadBase64,
                SoftwwareStatementSignatureBase64
            }.JoinString(".");

        /// Open Banking Signing Key ID as string, e.g. "ABC"
        public string SigningKeyId { get; set; } = null!;

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        public string SigningKey { get; set; } = null!;

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string SigningCertificate { get; set; } = null!;

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; set; } = null!;

        public ApiClient ApiClient { get; }

        /// Software statement profile ID as string, e.g. "DevPispSoftwareStatement"
        /// This is your choice; a meaningful name should help debugging throughout OBC.
        public string Id { get; set; } = null!;

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
