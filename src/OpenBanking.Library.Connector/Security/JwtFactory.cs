// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using Jose;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public class JwtFactory
    {
        public string CreateJwt<TClaims>(
            SoftwareStatementProfile profile,
            TClaims claims,
            bool useOpenBankingJwtHeaders) where TClaims : class
        {
            profile.ArgNotNull(nameof(profile));
            claims.ArgNotNull(nameof(claims));

            Dictionary<string, object> headers = useOpenBankingJwtHeaders
                ? CreateOpenBankingJwtHeaders(
                    signingId: profile.SigningKeyId,
                    orgId: profile.SoftwareStatementPayload.OrgId,
                    softwareId: profile.SoftwareStatementPayload.SoftwareId)
                : CreateJwtHeaders(profile.SigningKeyId);


            string payloadJson = JsonConvert.SerializeObject(
                value: claims,
                settings: new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

            X509Certificate2 privateKey = CertificateFactories.GetCertificate2FromPem(
                privateKey: profile.SigningKey,
                pem: profile.SigningCertificate);
            RSA privateKeyRsa = privateKey.GetRSAPrivateKey();

            string result = JWT.Encode(
                payload: payloadJson,
                key: privateKeyRsa,
                algorithm: JwsAlgorithm.PS256,
                extraHeaders: headers);

            return result;
        }

        public Dictionary<string, object> CreateJwtHeaders(string signingId)
        {
            signingId.ArgNotNull(nameof(signingId));

            return new Dictionary<string, object>
            {
                { "typ", "JWT" },
                { "kid", signingId }
            };
        }

        public Dictionary<string, object> CreateOpenBankingJwtHeaders(string signingId, string orgId, string softwareId)
        {
            signingId.ArgNotNull(nameof(signingId));
            orgId.ArgNotNull(nameof(orgId));
            softwareId.ArgNotNull(nameof(softwareId));

            return new Dictionary<string, object>
            {
                { "typ", "JOSE" },
                { "kid", signingId },
                {
                    "crit",
                    new[]
                    {
                        "b64", "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss",
                        "http://openbanking.org.uk/tan"
                    }
                },
                { "b64", false },
                { "obIat", DateTimeOffset.UtcNow },
                { "obIss", $"{orgId}/{softwareId}" },
                { "obTan", "openbanking.org.uk" }
            };
        }

        public Dictionary<string, object> CreateJoseHeaders(string signingId)
        {
            signingId.ArgNotNull(nameof(signingId));

            return new Dictionary<string, object>
            {
                { "typ", "JOSE" },
                { "kid", signingId }
            };
        }
    }
}
