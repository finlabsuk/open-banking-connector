// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using FinnovationLabs.OpenBanking.Library.Connector.Extensions;
using FinnovationLabs.OpenBanking.Library.Connector.Http;
using FinnovationLabs.OpenBanking.Library.Connector.Repositories;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Models.Repository
{
    public class SoftwareStatementProfile : IRepositoryItem
    {
        public SoftwareStatementProfile(
            string id,
            Configuration.SoftwareStatementProfile inputProfile,
            IApiClient apiClient)
        {
            // Pass-through properties
            SigningKeyId = inputProfile.SigningKeyId;
            SigningKey = inputProfile.SigningKey;
            SigningCertificate = inputProfile.SigningCertificate;
            DefaultFragmentRedirectUrl = inputProfile.DefaultFragmentRedirectUrl;
            Id = id;

            // Break software statement into components
            string[] softwareStatementComponentsBase64 = inputProfile.SoftwareStatement.Split(new[] { '.' });
            if (softwareStatementComponentsBase64.Length != 3)
            {
                throw new ArgumentException("softwareStatementComponentsBase64 needs 3 components.");
            }

            SoftwareStatementHeaderBase64 = softwareStatementComponentsBase64[0];
            SoftwareStatementPayloadBase64 = softwareStatementComponentsBase64[1];
            SoftwareStatementPayload =
                SoftwareStatementPayloadFromBase64(softwareStatementComponentsBase64[1]);
            SoftwwareStatementSignatureBase64 = softwareStatementComponentsBase64[2];

            if (SoftwareStatement != inputProfile.SoftwareStatement)
            {
                throw new InvalidOperationException("Can't correctly process software statement");
            }

            // Api client
            ApiClient = apiClient;
        }

        public string SoftwareStatementHeaderBase64 { get; }

        // TODO: Remove this once SoftwareStatementPayload stores all fields, this duplicates info in SoftwareStatementPayload
        public string SoftwareStatementPayloadBase64 { get; }

        public SoftwareStatementPayload SoftwareStatementPayload { get; }

        public string SoftwwareStatementSignatureBase64 { get; }

        /// Software statement as string, e.g. "A.B.C"
        public string SoftwareStatement =>
            new[]
            {
                SoftwareStatementHeaderBase64,
                SoftwareStatementPayloadBase64,
                SoftwwareStatementSignatureBase64
            }.JoinString(".");

        /// Open Banking Signing Key ID as string, e.g. "ABC"
        public string SigningKeyId { get; }

        /// Open Banking Signing Key as string, e.g. "-----BEGIN PRIVATE KEY-----\nABCD\n-----END PRIVATE KEY-----\n"
        public string SigningKey { get; }

        /// Open Banking Signing Certificate as string, e.g. "-----BEGIN CERTIFICATE-----\nABC\n-----END CERTIFICATE-----\n"
        public string SigningCertificate { get; }

        /// Default redirect URL for OAuth clients with response_mode == fragment.
        public string DefaultFragmentRedirectUrl { get; }

        public IApiClient ApiClient { get; }

        /// Software statement profile ID as string, e.g. "DevPispSoftwareStatement"
        /// This is your choice; a meaningful name should help debugging throughout OBC.
        public string Id { get; }

        public string SoftwareStatementPayloadToBase64(SoftwareStatementPayload payload)
        {
            string jsonData = JsonConvert.SerializeObject(payload);
            return Base64UrlEncoder.Encode(jsonData);
        }

        public SoftwareStatementPayload SoftwareStatementPayloadFromBase64(string payloadBase64)
        {
            // Perform conversion
            string payloadString = Base64UrlEncoder.Decode(payloadBase64);
            SoftwareStatementPayload newObject =
                JsonConvert.DeserializeObject<SoftwareStatementPayload>(payloadString) ??
                throw new ArgumentException("Cannot de-serialise software statement");

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
