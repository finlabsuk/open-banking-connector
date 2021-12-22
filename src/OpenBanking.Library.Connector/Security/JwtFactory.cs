﻿// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Security.Cryptography;
using Jose;
using Newtonsoft.Json;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security
{
    public static class JwtFactory
    {
        public static string CreateJwt<TClaims>(
            Dictionary<string, object> headers,
            TClaims claims,
            string signingKey,
            string signingCertificate,
            JsonSerializerSettings? jsonSerializerSettingsOverride = null)
            where TClaims : class
        {
            claims.ArgNotNull(nameof(claims));
            signingKey.ArgNotNull(nameof(signingKey));
            signingCertificate.ArgNotNull(nameof(signingCertificate));

            // Create JSON serialiser settings
            JsonSerializerSettings jsonSerializerSettings =
                jsonSerializerSettingsOverride ?? new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            string payloadJson = JsonConvert.SerializeObject(
                claims,
                jsonSerializerSettings);

            // X509Certificate2 privateKey = CertificateFactories.GetCertificate2FromPem(
            //     signingKey,
            //     signingCertificate) ?? throw new InvalidOperationException();
            // RSA privateKeyRsa = privateKey.GetRSAPrivateKey();

            var rsa = RSA.Create();
            try
            {
                CertificateFactories.ImportPrivateKey(signingKey, ref rsa);

                string result = JWT.Encode(
                    payloadJson,
                    rsa,
                    JwsAlgorithm.PS256,
                    headers);
                return result;
            }
            finally
            {
                rsa.Dispose();
            }
        }

        public static Dictionary<string, object> DefaultJwtHeadersExcludingTyp(string signingId) =>
            new Dictionary<string, object>
            {
                { "kid", signingId.ArgNotNull(nameof(signingId)) }
            };

        public static Dictionary<string, object> DefaultJwtHeadersIncludingTyp(string signingId) =>
            new Dictionary<string, object>
            {
                { "typ", "JWT" },
                { "kid", signingId.ArgNotNull(nameof(signingId)) }
            };
    }
}
