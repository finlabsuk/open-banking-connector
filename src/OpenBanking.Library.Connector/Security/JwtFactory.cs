// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Persistent;
using FinnovationLabs.OpenBanking.Library.Connector.Models.Public.PaymentInitiation;
using Jose;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public class JwtFactory
    {
        public string CreateJwt<TClaims>(
            SoftwareStatementProfile profile,
            TClaims claims,
            bool useOpenBankingJwtHeaders,
            ApiVersion paymentInitiationApiVersion) where TClaims : class
        {
            profile.ArgNotNull(nameof(profile));
            claims.ArgNotNull(nameof(claims));

            Dictionary<string, object> headers = useOpenBankingJwtHeaders
                ? CreateOpenBankingJwtHeaders(
                    orgId: profile.SoftwareStatementPayload.OrgId,
                    softwareId: profile.SoftwareStatementPayload.SoftwareId,
                    signingId: profile.SigningKeyId,
                    paymentInitiationApiVersion: paymentInitiationApiVersion)
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

        public Dictionary<string, object> CreateOpenBankingJwtHeaders(
            string orgId,
            string softwareId,
            string signingId,
            ApiVersion paymentInitiationApiVersion)
        {
            signingId.ArgNotNull(nameof(signingId));
            orgId.ArgNotNull(nameof(orgId));
            softwareId.ArgNotNull(nameof(softwareId));

            string[] crit;
            bool? b64;
            if (paymentInitiationApiVersion >= ApiVersion.V3P1P4)
            {
                crit = new[]
                {
                    "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss",
                    "http://openbanking.org.uk/tan"
                };
                b64 = false;
            }
            else
            {
                crit = new[]
                {
                    "http://openbanking.org.uk/iat", "http://openbanking.org.uk/iss",
                    "http://openbanking.org.uk/tan"
                };
                b64 = null;
            }
                
            var dict = new Dictionary<string, object>
            {
                { "typ", "JOSE" },
                { "cty", "application/json" },
                { "kid", signingId },
                { "crit", crit },
                { "http://openbanking.org.uk/iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds() },
                { "http://openbanking.org.uk/iss", $"{orgId}/{softwareId}" },
                { "http://openbanking.org.uk/tan", "openbanking.org.uk" }
            };

            if (!(b64 is null))
            {
                dict.Add("b64", b64.Value);
            }

            return dict;
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
