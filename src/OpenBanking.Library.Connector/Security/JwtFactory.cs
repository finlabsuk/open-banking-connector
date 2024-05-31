// Licensed to Finnovation Labs Limited under one or more agreements.
// Finnovation Labs Limited licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using Jose;

namespace FinnovationLabs.OpenBanking.Library.Connector.Security;

public static class JwtFactory
{
    public static string CreateJwt(
        Dictionary<string, object> headers,
        string payloadJson,
        string signingKey,
        JwsAlgorithm? jwsAlgorithm)
    {
        signingKey.ArgNotNull(nameof(signingKey));

        var rsa = RSA.Create();
        try
        {
            CertificateFactories.ImportPrivateKey(signingKey, ref rsa);

            string result = JWT.Encode(
                payloadJson,
                rsa,
                jwsAlgorithm ?? JwsAlgorithm.PS256,
                headers);
            return result;
        }
        finally
        {
            rsa.Dispose();
        }
    }

    public static Dictionary<string, object> DefaultJwtHeadersExcludingTyp(string signingId) =>
        new() { { "kid", signingId.ArgNotNull(nameof(signingId)) } };

    public static Dictionary<string, object> DefaultJwtHeadersIncludingTyp(string signingId) =>
        new()
        {
            { "typ", "JWT" },
            { "kid", signingId.ArgNotNull(nameof(signingId)) }
        };

    public static Dictionary<string, object> JwtHeaders(string? signingId, string? type)
    {
        var jwtHeaders = new Dictionary<string, object>();
        if (signingId is not null)
        {
            jwtHeaders.Add("kid", signingId);
        }
        if (type is not null)
        {
            jwtHeaders.Add("typ", type);
        }
        return jwtHeaders;
    }
}
